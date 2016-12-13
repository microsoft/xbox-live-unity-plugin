# Copyright (c) 2016 Microsoft. All Rights Reserved
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#
# Builds the Xbox Live Unity Package

$unity = Get-Command Unity -ea SilentlyContinue
if(!$unity)
{
  Get-Command "C:\Program Files\Unity\Editor\Unity.exe" -ea SilentlyContinue
}

if(!$unity)
{
  Write-Error "Unable to find Unity.exe.  Please make sure that Unity is installed."
  return;
}

$projectPath = Resolve-Path (Join-Path $PSScriptRoot ..)
$exportAssetPath = .\Assets\XboxLive
$packagePath = Resolve-Path (Join-Path $PSScriptRoot ..\XboxLive.unitypackage)
$logFile = Join-Path $PSScriptRoot BuildPackage.log

. $unity -exportPackage $exportAssetPath $packagePath -projectPath $projectPath -logFile $logFile -quit -batchmode
if($LASTEXITCODE -ne 0)
{
  Write-Error "Unity.exe failed to build package.  Exit Code: $LASTEXITCODE."
  Write-Error "See $logFile for details"
}
