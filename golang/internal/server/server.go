package server

import (
	"embed"
	"encoding/base64"
	"html/template"
	"io/fs"
	"net/http"
	"sync"
	"time"

	"github.com/google/uuid"
)

// Note stores the content and the creation timestamp
type Note struct {
	Content   string
	CreatedAt time.Time
}

type Server struct {
	assets embed.FS
	data   map[string]Note
	mu     sync.RWMutex
	tmpl   *template.Template
}

func New(assets embed.FS) http.Handler {

	// Parse your template once
	tmpl := template.Must(template.ParseFS(assets, "web/static/index.html"))

	s := &Server{
		assets: assets,
		data:   make(map[string]Note),
		tmpl:   tmpl,
	}

	// Start cleanup goroutine
	s.startCleanup()

	mux := http.NewServeMux()
	mux.HandleFunc("/", s.handleRoot)
	mux.HandleFunc("/v/", s.handleView)
	mux.HandleFunc("/s/", s.handleSave)

	sub, _ := fs.Sub(assets, "web/static")
	mux.Handle("/static/", http.StripPrefix("/static/", http.FileServer(http.FS(sub))))

	return mux
}

// Cleanup goroutine removes expired notes every minute
func (s *Server) startCleanup() {
	ticker := time.NewTicker(time.Minute)
	go func() {
		for range ticker.C {
			s.mu.Lock()
			for k, note := range s.data {
				if time.Since(note.CreatedAt) > 15*time.Minute {
					delete(s.data, k)
				}
			}
			s.mu.Unlock()
		}
	}()
}

// createNote generates a new UUID, stores an empty note, and returns the key
func (s *Server) createNote() string {
	u := uuid.New()
	key := base64.URLEncoding.EncodeToString(u[:])

	s.mu.Lock()
	s.data[key] = Note{
		Content:   "",
		CreatedAt: time.Now(),
	}
	s.mu.Unlock()

	return key
}

// Root: generates a new UUID and redirects
func (s *Server) handleRoot(w http.ResponseWriter, r *http.Request) {
	if r.URL.Path != "/" {
		http.NotFound(w, r)
		return
	}

	key := s.createNote()

	http.Redirect(w, r, "/v/"+key, http.StatusFound)
}

// View: renders the template with content
func (s *Server) handleView(w http.ResponseWriter, r *http.Request) {
	key := r.URL.Path[len("/v/"):]
	s.mu.RLock()
	note, exists := s.data[key]
	s.mu.RUnlock()

	if !exists || time.Since(note.CreatedAt) > 15*time.Minute {
		newKey := s.createNote()
		http.Redirect(w, r, "/v/"+newKey, http.StatusFound)
		return
	}

	err := s.tmpl.Execute(w, map[string]string{
		"Content": note.Content,
		"UUID":    key,
	})
	if err != nil {
		http.Error(w, "template error", http.StatusInternalServerError)
	}
}

func (s *Server) handleSave(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "method not allowed", http.StatusMethodNotAllowed)
		return
	}

	key := r.URL.Path[len("/s/"):]
	body := r.FormValue("content")

	s.mu.Lock()
	note, exists := s.data[key]
	if exists && time.Since(note.CreatedAt) <= 15*time.Minute {
		note.Content = body
		s.data[key] = note
	} else {
		http.Error(w, "note expired", http.StatusNotFound)
	}
	s.mu.Unlock()

	w.WriteHeader(http.StatusOK)
}
