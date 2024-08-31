$solutionPath = "./CandlelightModRepo.sln"

function CheckForNuGetPackagesUpdates {
    param (
        [string]$projectPath
    )
    Write-Host "Checking for NuGet package updates for $projectPath..."
    dotnet list $projectPath package --outdated
}

$projects = dotnet sln $solutionPath list | Select-String "\.csproj"

foreach ($project in $projects) {
    $projectPath = $project -replace "^\s+", ""
    $content = Get-Content $projectPath
    if ($content -match "<PackageReference") {
        CheckForNuGetPackagesUpdates -projectPath $projectPath
    } else {
        Write-Host "$projectPath uses packages.config or a different format. Skipping NuGet update."
    }
}

Write-Host "Restoring NuGet packages for the solution..."
dotnet restore $solutionPath

Write-Host "Building the backend..."
dotnet build $solutionPath

Set-Location ./candlelight.client
Write-Host "Installing frontend dependencies..."
npm install

$fixPackages = Read-Host "Do you want to perform audit fix? (y/n)"
if ($fixPackages -eq 'y') {
    npm audit fix
}

Write-Host "Building the frontend..."
ng build

Set-Location ..

if ($?) {
    Write-Host "Build succeeded."
} else {
    Write-Host "Build failed. Please check the error messages above."
}