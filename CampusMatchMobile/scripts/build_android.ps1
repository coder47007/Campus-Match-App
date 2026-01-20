
Write-Host "Starting Android Build Repair..."

# 1. Define Project Path
$projectPath = "d:\College\Projects\Mobile Apps\CampusMatch (2)\CampusMatchMobile"
Write-Host "Project Path: $projectPath"

# 2. Unmap X: (Ignore errors if not mapped)
Write-Host "Cleaning up X: drive..."
try { subst /d X: } catch {}

# 3. Map X: to Project Path
Write-Host "Mapping X: to Project Path..."
cmd /c "subst X: ""$projectPath"""

if (!(Test-Path X:\)) {
    Write-Error "Failed to map X: drive!"
    exit 1
}

# 4. Build
Write-Host "Switching to X:\android..."
Set-Location X:\android

Write-Host "Running Gradle Build..."
.\gradlew.bat assembleDebug > build_log_ps.txt 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build SUCCESS!"
} else {
    Write-Error "Build FAILED with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}
