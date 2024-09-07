Set-Location ./Candlelight.Backend
Write-Host "Launching the application..."
Start-Process powershell -ArgumentList '-NoExit', '-Command', 'dotnet run'

Set-Location ..