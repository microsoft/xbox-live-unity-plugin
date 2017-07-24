# Copyright (c) 2016 Microsoft. All Rights Reserved
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

<#
.SYNOPSIS
  Install the Xbox Live SDK into the Unity project
.PARAMETER FromSource
  Build the SDK from the CSharpSource folder and copy the binaries.
.PARAMETER FromNuget
  Download the SDK NuGet package and copy the binaries from there.
.PARAMETER CopyOnly,
  If FromSource is provided then the built binaries will just be copied as opposed
  to performing a rebuild and copying after.
#>
param(
  [switch]$FromSource,
  [switch]$FromNuget,
  [switch]$CopyOnly
)

$ErrorActionPreference = "Stop"

$sdkOutputPath = Join-Path $PSScriptRoot "..\Assets\Xbox Live\Libs\"
mkdir $sdkOutputPath -force | Out-Null

if(!($FromNuget -or $FromSource))
{
  Write-Warning "No switch was specified so we are building the SDK from source (equivalent to passing -FromSource)."
  $FromSource = $true
}

# Check if Nuget is already available somewhere on the path
$nugetCmd = "nuget"
if(!(Get-Command $nugetCmd -ErrorAction SilentlyContinue))
{
    $nugetPath = Join-Path $PSScriptRoot "nuget.exe"

    if(!(Test-Path $nugetPath))
    {
        Write-Host "Downloading nuget.exe..."
        iwr https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $nugetPath
    }
    
    $nugetCmd = $nugetPath
}

Import-Module "$PSScriptRoot\Invoke-MsBuild"

if($FromNuget)
{
  throw "Installing the SDK from NuGet is not currently supported."
  # Folder is named with a . prefix so that it's ignored by Unity, the files that we
  # need out of the package are manually copied out to the appropriate locations.
  
  # TODO: We can probably just dump the packages into the root instead of into the Assets folder.
  $packagesPath = Join-Path $PSScriptRoot "..\Assets\Xbox Live\.packages"
  Write-Host "Installing Microsoft.Xbox.Live.SDK.WinRT.UWP NuGet package into $packagesPath"
  . $nugetCmd install Microsoft.Xbox.Live.SDK.WinRT.UWP -OutputDirectory $packagesPath

  # TODO: Copy some portion of the files out into Unity directories as needed.
  return
}
elseif($FromSource)
{
  $sdkPath = Join-Path $PSScriptRoot "..\CSharpSource"
  $sdkSln = Join-Path $sdkPath "Source\Microsoft.Xbox.Services.CSharp.sln" 

  if(!(Test-Path $sdkSln))
  {
    Write-Error "Unable to find $sdkSln.  Make sure that all submodules are synced."
    return
  }

  if(!$CopyOnly)
  {
    & $nugetCmd restore $sdkSln

    Write-Host "Building (Platform:x86) Xbox Live SDK... "
    $buildResult = Invoke-MsBuild $sdkSln -BuildLogDirectoryPath $PSScriptRoot -ShowBuildOutputInCurrentWindow -Params "/property:Platform=x86"
    
    if(!$buildResult.BuildSucceeded)
    {
       Write-Host "Failed.  See build logs for details."
       Write-Host "Build Log: $($buildResult.BuildLogFilePath)"
       Write-Host "Error Log: $($buildResult.BuildErrorsLogFilePath)"
    }
    else {
      Write-Host "SDK Build (Platform:x86) Succeeded."
    }
    
    Write-Host "Building (Platform:x64) Xbox Live SDK... "
    $buildResult = Invoke-MsBuild $sdkSln -BuildLogDirectoryPath $PSScriptRoot -ShowBuildOutputInCurrentWindow -Params "/property:Platform=x64"
    
    if(!$buildResult.BuildSucceeded)
    {
       Write-Host "Failed.  See build logs for details."
       Write-Host "Build Log: $($buildResult.BuildLogFilePath)"
       Write-Host "Error Log: $($buildResult.BuildErrorsLogFilePath)"
    }
    else {
      Write-Host "SDK Build (Platform:x64) Succeeded."
    }
  }
}