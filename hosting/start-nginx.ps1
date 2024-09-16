# Command options: start, stop, reload
param (
    [string]$command = "help"
)

# Path to NGINX installed via Scoop (adjust if needed)
$nginxPath = "$env:USERPROFILE\scoop\apps\nginx\current"

# Function to start NGINX
function Start-Nginx {
    Write-Host "Starting NGINX..."
    & "$nginxPath\nginx.exe" -p $nginxPath -c "$PSScriptRoot\nginx.conf"
}

# Function to stop NGINX
function Stop-Nginx {
    Write-Output "Stopping NGINX..."
    # Stop NGINX gracefully first
    & $nginxPath -s stop

    # Wait a few seconds to allow graceful shutdown
    Start-Sleep -Seconds 5

    # Forcefully stop NGINX if it is still running
    $nginxProcesses = Get-Process nginx -ErrorAction SilentlyContinue
    if ($nginxProcesses) {
        Write-Output "Forcibly stopping remaining NGINX processes..."
        Stop-Process -Name nginx -Force
    } else {
        Write-Output "NGINX is not running."
    }
}


switch ($command) {
    "start"  { Start-Nginx }
    "stop"   { Stop-Nginx }
    default  { Write-Host "Usage: start-nginx.ps1 -c [start|stop]" }
}
