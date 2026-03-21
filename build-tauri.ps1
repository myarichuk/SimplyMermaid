$ErrorActionPreference = "Stop"

# Check if cargo is installed
if (-not (Get-Command cargo -ErrorAction SilentlyContinue)) {
    Write-Host "cargo not found, installing Rust..."
    Invoke-WebRequest -Uri "https://win.rustup.rs/x86_64" -OutFile "rustup-init.exe"
    Write-Host "Running rustup-init.exe..."
    .\rustup-init.exe -y
    Remove-Item "rustup-init.exe" -Force
    # Add cargo to the current session PATH
    $env:Path += ";$env:USERPROFILE\.cargo\bin"
} else {
    Write-Host "Rust is already installed."
}

# Navigate to Tauri project
Set-Location -Path "src\MermaidEditor.Tauri"

# Install npm dependencies
Write-Host "Installing npm dependencies..."
npm install

# Build the Tauri project
Write-Host "Building Tauri project..."
npm run tauri build

Write-Host "Build complete."
