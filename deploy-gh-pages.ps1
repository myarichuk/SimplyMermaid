$ErrorActionPreference = "Stop"

$ORIGINAL_DIR = Get-Location

# Store current branch
$CURRENT_BRANCH = git branch --show-current
if ([string]::IsNullOrWhiteSpace($CURRENT_BRANCH)) {
    Write-Host "Not currently on any branch. Please checkout a branch before running." -ForegroundColor Red
    exit 1
}

Write-Host "Deploying from branch: $CURRENT_BRANCH" -ForegroundColor Cyan

$STASHED = $false

try {
    # Check if there are uncommitted changes to stash
    $gitStatus = git status -s
    if (-not [string]::IsNullOrWhiteSpace($gitStatus)) {
        Write-Host "Stashing uncommitted changes..." -ForegroundColor Cyan
        git stash push -u -m "Auto-stash before gh-pages deployment"
        $STASHED = $true
    } else {
        Write-Host "No uncommitted changes to stash." -ForegroundColor Cyan
    }

    # Publish the Blazor app to the "release" folder
    Write-Host "Building the Blazor application..." -ForegroundColor Cyan
    if (Test-Path "release") {
        Remove-Item -Recurse -Force "release"
    }
    dotnet publish src/MermaidEditor/MermaidEditor.csproj -c Release -o release

    # Add .nojekyll to bypass GitHub Pages Jekyll processing
    $noJekyllPath = Join-Path "release" "wwwroot\.nojekyll"
    New-Item -ItemType File -Path $noJekyllPath -Force | Out-Null

    # Copy index.html to 404.html for SPA routing on GitHub Pages
    $indexPath = Join-Path "release" "wwwroot\index.html"
    $notFoundPath = Join-Path "release" "wwwroot\404.html"
    Copy-Item -Path $indexPath -Destination $notFoundPath -Force

    $REMOTE_URL = git config --get remote.origin.url
    if ([string]::IsNullOrWhiteSpace($REMOTE_URL)) {
        Write-Host "Error: remote 'origin' URL not found." -ForegroundColor Red
        exit 1
    }

    Set-Location -Path "release\wwwroot"

    # Initialize a new temporary Git repository here
    git init
    git checkout -b gh-pages

    # Add all published files
    git add .
    git commit -m "Deploy to GitHub Pages from $CURRENT_BRANCH"

    # Push the committed files directly to the gh-pages branch on the remote repository
    # Using force push because we created a new history locally
    Write-Host "Pushing to gh-pages..." -ForegroundColor Cyan
    git push -f $REMOTE_URL gh-pages

    Write-Host "Deployment complete!" -ForegroundColor Green
}
finally {
    # Switch back to the original directory
    Set-Location -Path $ORIGINAL_DIR

    # Pop the stashed changes if we created a stash
    if ($STASHED) {
        Write-Host "Popping stashed changes..." -ForegroundColor Cyan
        git stash pop 2>$null
    }
}
