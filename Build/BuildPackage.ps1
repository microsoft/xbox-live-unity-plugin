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

$gameSaveAssetsPath = "Assets\Xbox Live\GameSave"
$tempgameSavePackagePath = Join-Path 'Assets\Xbox Live\' GameSave.unitypackage
$gameSavePackagePath = Join-Path 'Assets\Xbox Live\GameSave\' GameSave.unitypackage
Remove-Item $gameSavePackagePath -ErrorAction SilentlyContinue

Write-Host "Moving Game Save Readme.txt into a temporary folder"
$externalFolder = Resolve-Path (Join-Path $projectPath ..)
$tempGameSaveFolder = New-Item (Join-Path $externalFolder 'tempGameSave') -type directory -force
Move-Item (Resolve-Path (Join-Path $gameSaveAssetsPath 'README.txt')) -Destination $tempGameSaveFolder

Write-Host "Exporting Xbox Live Game Save Unity Plugin to " -NoNewline
Write-Host $gameSavePackagePath -ForegroundColor Green
Write-Host "$($unity) -batchmode -logFile '$($logFile)' -projectPath '$($projectPath)' -exportPackage '$($gameSavePackagePath)' '$($gameSavePackagePath)' -quit"
. $unity -batchmode -logFile "$logFile" -projectPath "$projectPath" -exportPackage "$gameSaveAssetsPath" "$tempgameSavePackagePath" -quit
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
    Write-Error "Unity.exe failed to build game save package.  Exit Code: $($unityProcess.ExitCode)."
}
else
{
    Write-Host "Game Save Package created successfully. "
}
Write-Host ""

$packagePath = Join-Path $projectPath XboxLive.unitypackage
Remove-Item $packagePath -ErrorAction SilentlyContinue

Write-Host "Moving Game Save Scripts and Prefabs to a temporary folder."
Copy-Item $gameSaveAssetsPath -Destination $tempGameSaveFolder -ErrorAction SilentlyContinue -recurse
Remove-Item $gameSaveAssetsPath -recurse


Write-Host ""

Write-Host "Moving Game Save plugin into the 'GameSave' folder within Assets."
New-Item $gameSaveAssetsPath -type directory
Copy-Item $tempgameSavePackagePath -Destination $gameSaveAssetsPath
Remove-Item $tempgameSavePackagePath

Write-Host "Moving Readme.txt back into the Game Save folder ..."
Move-Item (Resolve-Path (Join-Path $tempGameSaveFolder 'README.txt')) -Destination $gameSaveAssetsPath 

$exportAssetPath = "Assets\Xbox Live"
$logFile = Join-Path $PSScriptRoot BuildPackage.log

Write-Host ""

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
Write-Host ""

Write-Host "Moving Game Save scripts and prefabs back into the 'GameSave' folder within Assets."
Move-Item (Resolve-Path (Join-Path $tempGameSaveFolder 'GameSave\Scripts')) -Destination $gameSaveAssetsPath
Move-Item (Resolve-Path (Join-Path $tempGameSaveFolder 'GameSave\Prefabs')) -Destination $gameSaveAssetsPath 
Move-Item (Resolve-Path (Join-Path $tempGameSaveFolder 'GameSave\Examples')) -Destination $gameSaveAssetsPath 
Remove-Item $tempGameSaveFolder -recurse

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

Write-Host ""
Write-Host "Done Executing the Script."