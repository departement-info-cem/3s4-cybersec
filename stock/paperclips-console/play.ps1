# Script de lancement Universal Paperclips Console
# Usage: .\play.ps1

Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║     UNIVERSAL PAPERCLIPS - Console Edition - Launcher      ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Vérifier .NET
Write-Host "Vérification de .NET..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERREUR: .NET n'est pas installé!" -ForegroundColor Red
    Write-Host "Téléchargez .NET depuis: https://dotnet.microsoft.com/download" -ForegroundColor Red
    Write-Host ""
    Read-Host "Appuyez sur Entrée pour quitter"
    exit 1
}

Write-Host "✓ .NET version $dotnetVersion détecté" -ForegroundColor Green
Write-Host ""

# Compiler si nécessaire
$exePath = "bin\Debug\net8.0\PaperclipsConsole.dll"
if (-not (Test-Path $exePath)) {
    Write-Host "Compilation du jeu..." -ForegroundColor Yellow
    dotnet build --nologo -v quiet
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERREUR lors de la compilation!" -ForegroundColor Red
        Read-Host "Appuyez sur Entrée pour quitter"
        exit 1
    }
    Write-Host "✓ Compilation réussie" -ForegroundColor Green
}

# Afficher l'emplacement de sauvegarde
$saveDir = Join-Path $env:APPDATA "papaclip"
Write-Host "Emplacement de sauvegarde: $saveDir" -ForegroundColor Cyan
Write-Host ""

# Lancer le jeu
Write-Host "Lancement du jeu..." -ForegroundColor Green
Write-Host ""
Start-Sleep -Seconds 1

dotnet run --no-build

Write-Host ""
Write-Host "Merci d'avoir joué!" -ForegroundColor Cyan
