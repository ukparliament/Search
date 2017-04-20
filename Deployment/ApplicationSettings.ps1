<#
.SYNOPSIS
Sets instrumentation key and connection string for use by API app.

.DESCRIPTION
Retrieves value of Application Insights' Instrumentation Key to and adds it to application settings of API app.
It also adds connection string (SparqlEndpoint).

.PARAMETER APIResourceGroupName
Name of the Resource Group where the API app is.

.PARAMETER SearchAPIName
Name of the API.

.PARAMETER ApplicationInsightsName
Name of the Application Insights.

.NOTES
This script is for use as a part of deployment in VSTS only.
#>

Param(
    [Parameter(Mandatory=$true)] [string] $APIResourceGroupName,
    [Parameter(Mandatory=$true)] [string] $SearchAPIName,
    [Parameter(Mandatory=$true)] [string] $ApplicationInsightsName,
    [Parameter(Mandatory=$true)] [string] $GoogleApiKey,
    [Parameter(Mandatory=$true)] [string] $GoogleEngineId
)
$ErrorActionPreference = "Stop"

function Log([Parameter(Mandatory=$true)][string]$LogText){
    Write-Host ("{0} - {1}" -f (Get-Date -Format "HH:mm:ss.fff"), $LogText)
}

Log "Getting Instrumentation Key"
$properties=Get-AzureRmResource -ResourceGroupName $APIResourceGroupName -ResourceName $ApplicationInsightsName -ExpandProperties | Select-Object Properties -ExpandProperty Properties

Log "Gets current settings"
$webApp = Get-AzureRmwebApp -ResourceGroupName $APIResourceGroupName -Name $SearchAPIName
$webAppSettings = $webApp.SiteConfig.AppSettings
$settings=@{}
foreach($set in $webAppSettings){ 
    $settings[$set.Name]=$set.Value
}

Log "Sets new settings values"
$settings["ApplicationInsightsInstrumentationKey"]=$properties.InstrumentationKey
$settings["GoogleApiKey"]=$GoogleApiKey
$settings["GoogleEngineId"]=$GoogleEngineId
Set-AzureRmWebApp -ResourceGroupName $APIResourceGroupName -Name $SearchAPIName -AppSettings $settings

Log "Job wel done!"