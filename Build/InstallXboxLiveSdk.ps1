# Copyright (c) 2016 Microsoft. All Rights Reserved
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

<#
.SUMMARY Install the Xbox Live SDK into the Unity project
.PARAM FromNuget
  Installing the SDK from the NuGet package.
.PARAM FromSource
  Build the SDK from the xbox-live-api-csharp submodule and copy the binaries.
.PARAM RawSource
  Copy the raw API source files directly into the Unity directory.
  This is generally useful if you're doing a large amount of back and forth between 
  the SDK and Unity, but requires manually copying changes back.
#>
param(
  [switch]$FromNuget, 
  [switch]$FromSource,
  [switch]$RawSource
)

$ErrorActionPreference = "Stop"

$sdkOutputPath = Join-Path $PSScriptRoot "..\Assets\Xbox Live\Libs\"
mkdir $sdkOutputPath -force | Out-Null


if($FromNuget)
{
  # Check if Nuget is already available somewhere on the path
  $nugetPath = "nuget.exe"
  if(!(Get-Command $nugetPath -ErrorAction SilentlyContinue))
  {
      $nugetPath = Join-Path $PSScriptRoot $nugetPath

      if(!(Test-Path $nugetPath))
      {
          Write-Host "Downloading nuget.exe..."
          iwr https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $nugetPath
      }
  }

  # Folder is named with a . prefix so that it's ignored by Unity, the files that we
  # need out of the package are manually copied out to the appropriate locations.
  
  # TODO: We can probably just dump the packages into the root instead of into the Assets folder.
  $packagesPath = Join-Path $PSScriptRoot "..\Assets\Xbox Live\.packages"
  Write-Host "Installing Microsoft.Xbox.Live.SDK.WinRT.UWP NuGet package into $packagesPath"
  . $nugetPath install Microsoft.Xbox.Live.SDK.WinRT.UWP -OutputDirectory $packagesPath

  # TODO: Copy some portion of the files out into Unity directories as needed.
  return
}
elseif($FromSource)
{
  $sdkPath = Join-Path $PSScriptRoot "..\External\xbox-live-api-csharp"
  $sdkSln = Join-Path $sdkPath "Build\Microsoft.Xbox.Services.Unity.CSharp\Microsoft.Xbox.Services.Unity.CSharp.sln" 

  if(!(Test-Path $sdkSln))
  {
    Write-Error "Unable to find $sdkSln.  Make sure that all submodules are synced."
    return
  }

  nuget restore $sdkSln
  msbuild $sdkSln

  Write-Host "Copying Xbox Live SDK to $sdkOutputPath"
  copy (Join-Path $sdkPath "Build\Microsoft.Xbox.Services.Unity.CSharp\bin\Debug\*") $sdkOutputPath -recurse -force
}
elseif($RawSource)
{
  # Otherwise just copy the raw source files from the xbox-live-api-csharp submodule directly into the project
  $sdkPath = Join-Path $PSScriptRoot "..\External\xbox-live-api-csharp"
  if(!(Test-Path (Join-Path $sdkPath "Source")))
  {
    Write-Error "Unable to find required files in xbox-live-api-csharp submodule.  Make sure that all submodules are synced."
    return
  }
  
  copy (Join-Path $sdkPath "External\parse-sdk\debug\*") $sdkOutputPath
  copy (Join-Path $sdkPath "Source\api") $sdkOutputPath -Exclude *.csproj -recurse -force
}