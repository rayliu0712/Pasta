$runtimes = dotnet --list-runtimes 2>$null
if (-not ($runtimes | Select-String "Microsoft.WindowsDesktop.App 8")) {
    Write-Host ".NET 8 Desktop Runtime is not installed."
    Write-Host "Please download and install it from:"
    Write-Host "https://dotnet.microsoft.com/download/dotnet/8.0"
    pause
    exit 1
}

$dest = "$env:LocalAppData\Pasta"
New-Item -ItemType Directory -Path $dest -Force | Out-Null
Copy-Item "$PSScriptRoot\bin\*" -Destination $dest -Force

$ws = New-Object -ComObject WScript.Shell

$sc = $ws.CreateShortcut([Environment]::GetFolderPath("Desktop") + "\Pasta.lnk")
$sc.TargetPath = "$dest\Pasta.exe"
$sc.WorkingDirectory = $dest
$sc.IconLocation = "$dest\Pasta.exe,0"
$sc.Save()

$startMenu = "$env:AppData\Microsoft\Windows\Start Menu\Programs"
$sm = $ws.CreateShortcut("$startMenu\Pasta.lnk")
$sm.TargetPath = "$dest\Pasta.exe"
$sm.WorkingDirectory = $dest
$sm.IconLocation = "$dest\Pasta.exe,0"
$sm.Save()

Write-Host "Installed to $dest"
Write-Host "Shortcuts created on Desktop and Start Menu"
pause
