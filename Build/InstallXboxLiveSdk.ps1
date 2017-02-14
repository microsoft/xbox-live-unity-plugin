# Copyright (c) 2016 Microsoft. All Rights Reserved
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

<#
.SYNOPSIS
  Install the Xbox Live SDK into the Unity project
.PARAMETER FromSource
  Build the SDK from the xbox-live-api-csharp submodule and copy the binaries.
.PARAMETER CopySource
  Copy the raw API source files directly into the Unity directory.
  This is generally useful if you're doing a large amount of back and forth between 
  the SDK and Unity, but requires manually copying changes back.
.PARAMETER FromNuget
  Download the SDK NuGet package and copy the binaries from there.
.PARAMETER CopyOnly,
  If FromSource is provided then the built binaries will just be copied as opposed
  to performing a rebuild and copying after.
#>
param(
  [switch]$FromSource,
  [switch]$CopySource,
  [switch]$FromNuget,
  [switch]$CopyOnly
)

$ErrorActionPreference = "Stop"

$sdkOutputPath = Join-Path $PSScriptRoot "..\Assets\Xbox Live\Libs\"
mkdir $sdkOutputPath -force | Out-Null

if(!($FromNuget -or $FromSource -or $CopySource))
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

if($FromNuget)
{
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
  $sdkPath = Join-Path $PSScriptRoot "..\External\xbox-live-api-csharp"
  $sdkSln = Join-Path $sdkPath "Source\Microsoft.Xbox.Services.CSharp.sln" 

  if(!(Test-Path $sdkSln))
  {
    Write-Error "Unable to find $sdkSln.  Make sure that all submodules are synced."
    return
  }

  if(!$CopyOnly)
  {
    & $nugetCmd restore $sdkSln
    
    Import-Module "$PSScriptRoot\Invoke-MsBuild"

    Write-Host "Building Xbox Live SDK... "
    $buildResult = Invoke-MsBuild $sdkSln -BuildLogDirectoryPath $PSScriptRoot -ShowBuildOutputInCurrentWindow
    
    if(!$buildResult.BuildSucceeded)
    {
       Write-Host "Failed.  See build logs for details."
       Write-Host "Log File: $buildResult.Build$LogFilePath"
       Write-Host "Error Log File: $buildResult.BuildErrorsLogFilePath"
    }
    else {
      Write-Host "SDK Build Succeeded."
    }
  }
}
elseif($CopySource)
{
  # Otherwise just copy the raw source files from the xbox-live-api-csharp submodule directly into the project
  $sdkPath = Join-Path $PSScriptRoot "..\External\xbox-live-api-csharp"
  if(!(Test-Path (Join-Path $sdkPath "Source")))
  {
    Write-Error "Unable to find required files in xbox-live-api-csharp submodule.  Make sure that all submodules are synced."
    return
  }
  
  copy (Join-Path $sdkPath "External\parse-sdk\debug\*") $sdkOutputPath
  copy (Join-Path $sdkPath "External\newtonsoft\9.0.1\*.dll") $sdkOutputPath
  copy (Join-Path $sdkPath "Source\api") $sdkOutputPath -Exclude *.csproj -recurse -force
}