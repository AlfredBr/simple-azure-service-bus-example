. ./azure-variables.ps1

# Install Azure PowerShell module if not already installed
if (-not (Get-Module -ListAvailable Az))
{
    Install-Module -Name Az -AllowClobber -Scope CurrentUser
    Import-Module -Name Az
}

# Connect to your Azure account
Connect-AzAccount

# Set the Azure subscription context (if you have multiple subscriptions)
Set-AzContext -SubscriptionId $subscriptionId

# Delete the topics
Remove-AzServiceBusTopic -ResourceGroupName $resourceGroupName -Namespace $namespaceName -Name "even"
Remove-AzServiceBusTopic -ResourceGroupName $resourceGroupName -Namespace $namespaceName -Name "odd"

# Delete the Service Bus namespace
Remove-AzServiceBusNamespace -ResourceGroupName $resourceGroupName -NamespaceName $namespaceName

# Delete a resource group
Remove-AzResourceGroup -Name $resourceGroupName -Force
