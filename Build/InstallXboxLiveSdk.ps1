# Copyright (c) 2016 Microsoft. All Rights Reserved
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

$nugetPath = "nuget"

# Check if Nuget is already available somewhere on the path
if(!(Get-Command $nugetPath -ErrorAction SilentlyContinue))
{
    $nugetPath = Join-Path $PSScriptRoot nuget.exe

    if(!(Test-Path $nugetPath))
    {
        Write-Host "Downloading nuget.exe..."
        iwr https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $nugetPath
    }
}

# Folder is named with a . prefix so that it's ignored by Unity, the files that we
# need out of the package are manually copied out to the appropriate locations.
$sdkOutputPath = Join-Path $PSScriptRoot "..\Assets\Xbox Live\.packages"
Write-Host "Installing Microsoft.Xbox.Live.SDK.WinRT.UWP NuGet package into $sdkOutputPath"
. $nugetPath install Microsoft.Xbox.Live.SDK.WinRT.UWP -OutputDirectory $sdkOutputPath

# TODO: Copy some portion of the files out into Unity directories as needed.
