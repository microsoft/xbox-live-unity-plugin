<#
.SUMMARY
  Removes the Unity configuration that proxies HTTP calls through Fiddler.
#>
param([string]$HostName, [int]$Port=8888, [switch]$Force)

$unityPath = 'C:\Program Files\Unity\Editor\Data\Mono\etc\mono\2.0\'
if(!(Test-Path $unityPath))
{
  $unityPath = 'C:\Program Files (x86)\Unity\Editor\Data\Mono\etc\mono\2.0\'
}

$machineConfigPath = Join-Path $unityPath machine.config
$machineConfig = [xml](gc $machineConfigPath)

$hasDefaultProxy = $machineConfig.configuration.'system.net' | Get-Member -Name defaultProxy
if($hasDefaultProxy)
{
  $machineConfig.configuration.'system.net'.RemoveChild($machineConfig.configuration.'system.net'.defaultProxy) | Out-Null
  $machineConfig.Save($machineConfigPath)
}