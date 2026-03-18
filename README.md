# MermaidEditor

A proof-of-concept WYSIWYG Mermaid editor built with .NET 10 Blazor WebAssembly and Mermaid.js.

## Project Vision

MermaidEditor aims to bridge the gap between text-based diagram generation and visual canvas editing. By using Blazor WASM, it allows developers to build robust C# graph models in the browser without needing a backend server, and it utilizes a thin JS interop layer to instantly render the result via Mermaid.js. The goal is to create a fully open-source, easily deployable standard tool for diagram building.

## Quick Start

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

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

### Features
- **Add Nodes:** Click "Add Node" to dynamically create elements.
- **Drag Nodes:** Click and drag nodes around the canvas freely.
- **Connect Nodes:** Hold `Shift` and drag from one node to another to create directed edges.
- **Live Preview:** Instant feedback via Mermaid.js SVG generation from C# state models.

## Development Roadmap

- [x] Establish OSS scaffolding (CI/CD, semantic release).
- [x] Build core Graph/Node/Edge C# data structures.
- [x] Setup thin JS interop layer for Mermaid SVG injection.
- [x] Basic HTML5/SVG canvas for viewing nodes and edges.
- [x] Implement Drag & Drop interactivity.
- [x] Implement Edge creation (Shift + Drag).
- [ ] Add editable labels for Nodes and Edges.
- [ ] Support advanced Mermaid flowchart geometries.
- [ ] Add dark mode support.
- [ ] Export to PNG/SVG feature.

## Contributing

We use [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/). Please ensure your commit messages follow semantic rules (e.g., `feat:`, `fix:`, `chore:`, `docs:`).

This project relies heavily on GitHub actions:
- **.NET CI:** Runs `dotnet build` and `xunit` tests automatically on PRs and commits to `main`.
- **Semantic Release:** Automatically tags, versions, and builds releases based on commit history.
