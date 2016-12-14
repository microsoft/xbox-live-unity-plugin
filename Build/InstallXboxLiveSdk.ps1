# Copyright (c) 2016 Microsoft. All Rights Reserved
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
param(
  [switch]$FromNuget
)

$ErrorActionPreference = "Stop"


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
}
else
{
  # Otherwise just copy the raw source files from the xbox-live-api-csharp submodule directly into the project
  $sdkPath = Join-Path $PSScriptRoot "..\External\xbox-live-api-csharp"
  if(!(Test-Path (Join-Path $sdkPath "Source")))
  {
    Write-Error "Unable to find required files in xbox-live-api-csharp submodule.  Make sure that all submodules are synced."
    return
  }
  
  $sdkOutputPath = Join-Path $PSScriptRoot "..\Assets\Xbox Live\Libs\"
  mkdir $sdkOutputPath -force | Out-Null
  
  copy (Join-Path $sdkPath "External\parse-sdk\debug\*") $sdkOutputPath
  copy (Join-Path $sdkPath "Source\api") $sdkOutputPath -Exclude *.csproj -recurse -force
}