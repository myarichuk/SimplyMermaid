# SimplyMermaid Editor

**Note: this is a vibe coding experiment**

A proof-of-concept WYSIWYG Mermaid editor built with .NET 10 Blazor WebAssembly and Mermaid.js.

## Project Vision

SimplyMermaid aims to bridge the gap between text-based diagram generation and visual canvas editing. By using Blazor WASM, it allows developers to build robust C# graph models in the browser without needing a backend server, and it utilizes a thin JS interop layer to instantly render the result via Mermaid.js. The goal is to create a fully open-source, easily deployable standard tool for diagram building.

![SimplyMermaid Editor Screenshot 1](placeholder-screenshot1.png)
![SimplyMermaid Editor Screenshot 2](placeholder-screenshot2.png)

## Deployment Options

SimplyMermaid supports two main deployment targets:

1. **Web Hosted Blazor (WASM):** The editor can be served as a set of static files. Since it runs entirely in the browser using WebAssembly, it requires no active backend (e.g., Node.js, PHP, ASP.NET Core). Any simple web server like Nginx or GitHub Pages can host it.
2. **Desktop Application (Tauri):** The exact same Blazor WASM output is bundled into a lightweight, cross-platform desktop application using Tauri.

## Quick Start

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (Required for all options)
- [Docker](https://www.docker.com/) (Required for Docker deployment)
- [Node.js](https://nodejs.org/) & [Rust](https://www.rust-lang.org/) (Required for compiling the Tauri Desktop app)

### Run Locally
1. Clone the repository.
2. Navigate to the UI source code:
   ```bash
   cd src/MermaidEditor
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Open the displayed localhost URL in your browser.

### Docker Deployment (Web)
The project includes a `Dockerfile` to build and serve the application using Nginx.

To build and run via Docker:
1. Run the build script (or use `docker build` directly):
   - On Linux/macOS:
     ```bash
     ./docker-build.sh
     ```
   - On Windows:
     ```powershell
     .\docker-build.ps1
     ```
2. Open `http://localhost:8080` in your browser.

### Desktop Application (Tauri)
You can compile the application as a native desktop app using Tauri.

1. Ensure prerequisites (Node.js and Rust) are installed. The scripts will attempt to install Rust if it is missing.
2. Run the build script from the root of the project:
   - On Linux/macOS:
     ```bash
     ./build-tauri.sh
     ```
   - On Windows:
     ```powershell
     .\build-tauri.ps1
     ```
3. The built application executable will be available in the `src/MermaidEditor.Tauri/src-tauri/target/release/` directory.

## Features

- **Interactive Canvas:** Free-form dragging, zooming, panning, and connecting nodes.
- **Lasso Multi-Select:** Drag over empty space to multi-select and group-drag nodes and edges.
- **Keyboard Shortcuts:** `Ctrl+A` to select all, `Delete/Backspace` to remove, `Escape` to cancel actions, and robust `Ctrl+Z` / `Ctrl+Y` Undo/Redo support.
- **Import & Export:** Export cleanly formatted Mermaid markdown, JSON state, or high-quality transparent PNGs. Import Mermaid text or JSON easily via the side drawer.
- **Position Round-Tripping:** Seamlessly preserves node positions in standard Mermaid comments during serialization.
- **Sequence Diagram Support:** Toggle between Flowchart and Sequence diagrams with native participant and message rendering.
- **Slick MudBlazor UI:** Clean, Excalidraw-inspired dark mode UI with intuitive tooltips, toolbar components, and helpful contextual hints.
- **Throttled Render Queue:** 60fps interaction with debounced background synchronization to Mermaid.js.

## Contributing

We use [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/). Please ensure your commit messages follow semantic rules (e.g., `feat:`, `fix:`, `chore:`, `docs:`).

This project relies heavily on GitHub actions:
- **.NET CI:** Runs `dotnet build` and `xunit` tests automatically on PRs and commits to `main`.
- **Semantic Release:** Automatically tags, versions, and builds releases based on commit history.
