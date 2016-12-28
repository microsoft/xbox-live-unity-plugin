# Copyright (c) 2016 Microsoft. All Rights Reserved
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
$ErrorActionPreference = "Stop"

$associationWizardPath = Join-Path $PSScriptRoot "..\External\xbox-live-plugin-shared\AssociationWizard"
$associationWizardSln = Join-Path $associationWizardPath AssociationWizard.sln
if(!(Test-Path $associationWizardSln))
{
  Write-Error "Unable to find $associationWizardSln.  Make sure that all submodules are synced."
  return
}

Write-Host "Rebuilding Xbox Live Association Wizard..."
nuget restore $associationWizardSln
msbuild $associationWizardSln

$unityToolsPath = Join-Path $PSScriptRoot "..\Assets\Xbox Live\Tools\AssociationWizard"
mkdir $unityToolsPath -Force | Out-Null

Write-Host "Copying Assocation Wizard to $unityToolsPath"
copy (Join-Path $associationWizardPath "bin\Debug\*") $unityToolsPath -recurse -force