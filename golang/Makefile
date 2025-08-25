

.PHONY: build
build: 
	go build -o build/notes ./cmd/notes

.PHONY: run
run: build
	./build/notes

.PHONY: clean
clean:
	rm -f build/notes

.PHONY: dev
dev:
	air
