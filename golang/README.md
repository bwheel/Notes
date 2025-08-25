# Quick Note

Quick Note is a simple web application for creating and sharing temporary notes. Notes expire automatically after a short period, making it ideal for quick, disposable sharing.

## Features

- Create and share notes with unique links
- Notes expire after 15 minutes
- Responsive, minimal interface
- All assets embedded in the Go binary

## Requirements

- Go 1.18 or newer
- [Air](https://github.com/cosmtrek/air) (optional, for hot reloading during development)

## Build

To compile the application:

```sh
make build
```

This will create the binary at `build/notes`.

## Run

To run the compiled binary:

```sh
make run
```

Or directly:

```sh
./build/notes
```

The server will start at [http://localhost:8080](http://localhost:8080).

## Development (Hot Reload)

To run with hot reloading (requires Air):

```sh
make dev
```

Any changes to Go or static files will automatically restart the server.

## Clean

To remove the compiled binary:

```sh
make clean
```

## Project Structure

- `cmd/notes/` — Main application entry point
- `internal/server/` — Server logic
- `cmd/notes/web/static/` — Static assets (HTML, CSS, JS)

## License

&copy; Byron Wheeler
