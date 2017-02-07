# Copyright (c) 2016 Microsoft. All Rights Reserved
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#
# There Unity plugin depends on a few 'siblings' for all of it's functionality.
# Some of these are pulled from git submodules so makes sure that everything has been synced beforehand.

Write-Host "Installing the Xbox Live SDK NuGet package" -Foreground Cyan
& "$PSScriptRoot\InstallXboxLiveSdk.ps1" -FromSource