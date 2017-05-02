# Copyright (c) 2016 Microsoft. All Rights Reserved
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#
# Builds the Xbox Live Unity Package
$ErrorActionPreference = "Stop"

$unity = Get-Command Unity -ea SilentlyContinue
if(!$unity)
{
  $unity = Get-Command "C:\Program Files\Unity\Editor\Unity.exe" -ea SilentlyContinue
}

if(!$unity)
{
  Write-Error "Unable to find Unity.exe.  Please make sure that Unity is installed."
  return;
}

$projectPath = Resolve-Path (Join-Path $PSScriptRoot ..)
$packagePath = Join-Path $projectPath XboxLive.unitypackage
Remove-Item $packagePath -ErrorAction SilentlyContinue

$exportAssetPath = "Assets\Xbox Live"
$logFile = Join-Path $PSScriptRoot BuildPackage.log

Write-Host "Exporting Xbox Live Unity Plugin to " -NoNewline
Write-Host $packagePath -ForegroundColor Green
Write-Host "$($unity) -batchmode -logFile '$($logFile)' -projectPath '$($projectPath)' -exportPackage '$($exportAssetPath)' '$($packagePath)' -quit"
. $unity -batchmode -logFile "$logFile" -projectPath "$projectPath" -exportPackage "$exportAssetPath" "$packagePath" -quit
$global:unityProcess = Get-Process Unity | Sort-Object StartTime | Select-Object -Last 1 


# Wait for it to complete exporting the package
$unityProcess | Wait-Process -Timeout 120 -ErrorAction SilentlyContinue

if(!$unityProcess.HasExited)
{
    Write-Warning "Unity (PID $($unityProcess.Id)) seems to be taking a long time to generate the package.  Check the log file for details to see what's happening."
    Write-Warning "Log: $logFile"
}
elseif($unityProcess.ExitCode -ne 0)
{
    Write-Warning "Log: $logFile"
    Write-Error "Unity.exe failed to build package.  Exit Code: $($unityProcess.ExitCode)."
}
else
{
    Write-Host "Package created successfully. "
}