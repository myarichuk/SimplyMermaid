#!/bin/bash
set -e

# Store current branch
CURRENT_BRANCH=$(git branch --show-current)

if [ -z "$CURRENT_BRANCH" ]; then
    echo "Not currently on any branch. Please checkout a branch before running."
    exit 1
fi

echo "Deploying from branch: $CURRENT_BRANCH"

STASHED=0
ORIGINAL_DIR=$(pwd)

# Setup a trap to ensure we always pop stash on exit
cleanup() {
    echo "Cleaning up..."
    # Go back to the original directory
    cd "$ORIGINAL_DIR" || true

    # Pop the stashed changes if we created a stash
    if [ $STASHED -eq 1 ]; then
        echo "Popping stashed changes..."
        git stash pop || true
    fi
    echo "Deployment script finished."
}
trap cleanup EXIT

# Check if there are uncommitted changes to stash
# We stash BEFORE building so we don't stash the build output if we use -u (though we don't need to change branches anymore, we might still want to stash local changes to ensure a clean build state from tracked files).
if [[ -n $(git status -s) ]]; then
    echo "Stashing uncommitted changes..."
    git stash push -u -m "Auto-stash before gh-pages deployment"
    STASHED=1
else
    echo "No uncommitted changes to stash."
fi

# Publish the Blazor app to the "release" folder
echo "Building the Blazor application..."
rm -rf release
dotnet publish src/MermaidEditor/MermaidEditor.csproj -c Release -o release

# Add .nojekyll to bypass GitHub Pages Jekyll processing
touch release/wwwroot/.nojekyll

# Copy index.html to 404.html for SPA routing on GitHub Pages
cp release/wwwroot/index.html release/wwwroot/404.html

# We need the remote URL to push to
REMOTE_URL=$(git config --get remote.origin.url)
if [ -z "$REMOTE_URL" ]; then
    echo "Error: remote 'origin' URL not found."
    exit 1
fi

# Navigate to the publish output directory
cd release/wwwroot

# Initialize a new temporary Git repository here
git init
git checkout -b gh-pages

# Add all published files
git add .
git commit -m "Deploy to GitHub Pages from $CURRENT_BRANCH"

# Push the committed files directly to the gh-pages branch on the remote repository
# Using force push because we created a new history locally
echo "Pushing to gh-pages..."
git push -f "$REMOTE_URL" gh-pages

echo "Deployment complete!"
