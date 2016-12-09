$associationWizardPath = Join-Path $PSScriptRoot "..\External\xbox-live-plugin-shared\AssociationWizard"
$unityToolsPath = Join-Path $PSScriptRoot "..\Assets\Xbox Live\Tools\AssociationWizard"

Write-Host "Rebuilding Xbox Live Association Wizard..."
Push-Location $associationWizardPath
nuget restore
msbuild AssociationWizard.sln
Pop-Location


if(!(Test-Path $unityToolsPath))
{
  Write-Host "Creating $unityToolsPath"
  mkdir $unityToolsPath | Out-Null
}

Write-Host "Copying Assocation Wizard to Unity tools..."
copy (Join-Path $associationWizardPath "bin\Debug\*") $unityToolsPath -recurse -force