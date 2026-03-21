#!/bin/bash
set -e

# Check if cargo is installed
if ! command -v cargo &> /dev/null
then
    echo "cargo not found, installing Rust..."
    curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh -s -- -y
    # Source the env file to make cargo available in the current session
    source "$HOME/.cargo/env"
else
    echo "Rust is already installed."
fi

# Navigate to Tauri project
cd src/MermaidEditor.Tauri

# Install npm dependencies
echo "Installing npm dependencies..."
npm install

# Build the Tauri project
echo "Building Tauri project..."
npm run tauri build

echo "Build complete."
