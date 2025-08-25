package main

import (
	"embed"
	"log"
	"net/http"

	"github.com/bwheel/notes/internal/server"
)

//go:embed web/static/*
var staticFiles embed.FS

func main() {
	srv := server.New(staticFiles)
	log.Println("listening on http://localhost:8080")
	log.Fatal(http.ListenAndServe(":8080", srv))
}
