<#
.SYNOPSIS
Generates API.

.DESCRIPTION
Creates endpoints to access GraphDB.

.PARAMETER ClusterResourceGroupName
Name of the Resource Group where the cluster is.

.PARAMETER SearchAPIName
Name of the Search API.

.NOTES
This script is for use as a part of deployment in VSTS only.
#>

Param(
    [Parameter(Mandatory=$true)] [string] $ClusterResourceGroupName,
    [Parameter(Mandatory=$true)] [string] $SearchAPIName
)

$ErrorActionPreference = "Stop"

$productTitle="Search"

function Log([Parameter(Mandatory=$true)][string]$LogText){
    Write-Host ("{0} - {1}" -f (Get-Date -Format "HH:mm:ss.fff"), $LogText)
}

Log "Retrives API Management"
$apiManagement=Get-AzureRmApiManagement -ResourceGroupName $ClusterResourceGroupName

Log "Get API Management context"
$management=New-AzureRmApiManagementContext -ResourceGroupName $ClusterResourceGroupName -ServiceName $apiManagement.Name

Log "Check if product already installed"
$product=(Get-AzureRmApiManagementProduct -Context $management | Where-Object Title -Match $productTitle)
if ($product -eq $null){
    Log "Access for Search API"
    $product=New-AzureRmApiManagementProduct -Context $management -Title "Parliament - Search API" -Description "For parliament use only." -ApprovalRequired $true -SubscriptionsLimit 1
    $api=New-AzureRmApiManagementApi -Context $management -Name "Search" -Description "All routes on Search API" -ServiceUrl "https://$SearchAPIName.azurewebsites.net/" -Protocols @("https") -Path "/search"
    New-AzureRmApiManagementOperation -Context $management -ApiId $api.ApiId -Name "Search (description)" -Method "GET" -UrlTemplate "/description"
    
    $request=New-Object -TypeName Microsoft.Azure.Commands.ApiManagement.ServiceManagement.Models.PsApiManagementRequest
    $request.QueryParameters=@(
        New-Object -TypeName Microsoft.Azure.Commands.ApiManagement.ServiceManagement.Models.PsApiManagementParameter -Property @{
            Name="q"
            Type="string"
            Required=$true
        }
        New-Object -TypeName Microsoft.Azure.Commands.ApiManagement.ServiceManagement.Models.PsApiManagementParameter -Property @{
            Name="start"
            Type="number"
            Required=$false
        }
    )
    New-AzureRmApiManagementOperation -Context $management -ApiId $api.ApiId -Name "Search" -Method "GET" -UrlTemplate "/" -Request $request
    
    Add-AzureRmApiManagementApiToProduct -Context $management -ProductId $product.ProductId -ApiId $api.ApiId
}

Log "Retrives subscription"
$subscription=Get-AzureRmApiManagementSubscription -Context $management -ProductId $product.ProductId

Log "Setting variables to use during deployment"
Write-Host "##vso[task.setvariable variable=APIManagementIP]$($apiManagement.StaticIPs[0])"

Log "Job well done!"