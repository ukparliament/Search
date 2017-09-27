<#
.SYNOPSIS
Sets settings for API app.

.DESCRIPTION
Sets values of application settings.

.PARAMETER APIResourceGroupName
Name of the Resource Group where the API Management is.

.NOTES
This script is for use as a part of deployment in VSTS only.
#>

Param(
	[Parameter(Mandatory=$true)] [string] $APIResourceGroupName,
	[Parameter(Mandatory=$true)] [string] $APIManagementName,
	[Parameter(Mandatory=$true)] [string] $SearchAPIName,
	[Parameter(Mandatory=$true)] [string] $APIPrefix
)
$ErrorActionPreference = "Stop"

function Log([Parameter(Mandatory=$true)][string]$LogText){
    Write-Host ("{0} - {1}" -f (Get-Date -Format "HH:mm:ss.fff"), $LogText)
}

Log "Gets current settings"
$webApp = Get-AzureRmwebApp -ResourceGroupName $APIResourceGroupName -Name $SearchAPIName
$webAppSettings = $webApp.SiteConfig.AppSettings
$settings=@{}
foreach($set in $webAppSettings){ 
    $settings[$set.Name]=$set.Value
}

Log "Sets new api url"
$settings["ApiManagementServiceUrl"]="https://$APIManagementName.azure-api.net/$APIPrefix/"
Set-AzureRmWebApp -ResourceGroupName $APIResourceGroupName -Name $SearchAPIName -AppSettings $settings


Log "Job well done!"