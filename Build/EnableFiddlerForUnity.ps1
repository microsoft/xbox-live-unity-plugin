<#
.SUMMARY
  Sets up Unity to use Fiddler
.PARAMETER IP
  The host name or IP address of the Fiddler proxy.  Defaults to the current hosts name.
.PARAMETER Port
  The port of the Fiddler proxy.  Defaults to 8888.
#>
param([string]$HostName, [int]$Port=8888, [switch]$Force)

if(!$HostName)
{
  $HostName = [System.Net.Dns]::GetHostName()
}

$unityPath = 'C:\Program Files\Unity\Editor\Data\Mono\etc\mono\2.0\'
if(!(Test-Path $unityPath))
{
  $unityPath = 'C:\Program Files (x86)\Unity\Editor\Data\Mono\etc\mono\2.0\'
}

$machineConfigPath = Join-Path $unityPath machine.config
$machineConfig = [xml](gc $machineConfigPath)

$hasDefaultProxy = $machineConfig.configuration.'system.net' | Get-Member -Name defaultProxy
if($Force -or !$hasDefaultProxy)
{
  if($hasDefaultProxy)
  {
    $machineConfig.configuration.'system.net'.RemoveChild($machineConfig.configuration.'system.net'.defaultProxy)
  }
  $proxy = $machineConfig.CreateElement("proxy")
  $proxy.SetAttribute("proxyaddress", "http://$($HostName):$Port") | out-null
  $defaultProxy = $machineConfig.CreateElement("defaultProxy")
  $defaultProxy.AppendChild($proxy) | out-null
  $machineConfig.configuration.'system.net'.AppendChild($defaultProxy) | out-null
  
  $machineConfig.Save($machineConfigPath)
}