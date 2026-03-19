#!/bin/bash
echo "Building MermaidEditor Docker Image..."
docker build -t mermaideditor:latest .
echo "Running MermaidEditor Docker Container on http://localhost:8080 ..."
docker run -d -p 8080:80 --name mermaideditor-app mermaideditor:latest
echo "Done! The application should be available at http://localhost:8080"