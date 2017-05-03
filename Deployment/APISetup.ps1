<#
.SYNOPSIS
Generates API.

.DESCRIPTION
Creates endpoints to access GraphDB.

.PARAMETER APIResourceGroupName
Name of the Resource Group where the API Management is.

.PARAMETER SearchAPIName
Name of the Search API.

.NOTES
This script is for use as a part of deployment in VSTS only.
#>

Param(
    [Parameter(Mandatory=$true)] [string] $APIResourceGroupName,
    [Parameter(Mandatory=$true)] [string] $SearchAPIName
)

$ErrorActionPreference = "Stop"

$productTitle="Parliament - Search API"

function Log([Parameter(Mandatory=$true)][string]$LogText){
    Write-Host ("{0} - {1}" -f (Get-Date -Format "HH:mm:ss.fff"), $LogText)
}

Log "Retrives API Management"
$apiManagement=Get-AzureRmApiManagement -ResourceGroupName $APIResourceGroupName

Log "Get API Management context"
$management=New-AzureRmApiManagementContext -ResourceGroupName $APIResourceGroupName -ServiceName $apiManagement.Name

Log "Check if product already installed"
$product=(Get-AzureRmApiManagementProduct -Context $management | Where-Object Title -Match $productTitle)
if ($product -eq $null){
    Log "Access for Search API"
    $product=New-AzureRmApiManagementProduct -Context $management -Title $productTitle -Description "For parliament use only." -ApprovalRequired $true -SubscriptionsLimit 1
    $api=New-AzureRmApiManagementApi -Context $management -Name "Search" -Description "All routes on Search API" -ServiceUrl "https://$SearchAPIName.azurewebsites.net/" -Protocols @("https") -Path "/search"
    $operation=New-AzureRmApiManagementOperation -Context $management -ApiId $api.ApiId -Name "Search (description)" -Method "GET" -UrlTemplate "/description"
    Set-AzureRmApiManagementPolicy -Context $management -ApiId $api.ApiId -OperationId $operation.OperationId -Policy ('<policies><inbound><base/></inbound><backend><base/></backend><outbound><base/><find-and-replace from="{0}.azurewebsites.net" to="{1}.azure-api.net"/></outbound></policies>' -f $SearchAPIName, $apiManagement.Name)
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
        New-Object -TypeName Microsoft.Azure.Commands.ApiManagement.ServiceManagement.Models.PsApiManagementParameter -Property @{
            Name="pagesize"
            Type="number"
            Required=$false
        }
    )
    $operation=New-AzureRmApiManagementOperation -Context $management -ApiId $api.ApiId -Name "Search" -Method "GET" -UrlTemplate "/" -Request $request
    Set-AzureRmApiManagementPolicy -Context $management -ApiId $api.ApiId -OperationId $operation.OperationId -Policy '<policies><inbound><base /><rewrite-uri template="search" /></inbound><backend><base /></backend><outbound><base /></outbound><on-error><base /></on-error></policies>'
    Add-AzureRmApiManagementApiToProduct -Context $management -ProductId $product.ProductId -ApiId $api.ApiId
}

Write-Host "##vso[task.setvariable variable=APIManagementIP]$($apiManagement.StaticIPs[0])"

Log "Job well done!"