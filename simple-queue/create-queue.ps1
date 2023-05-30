. ../azure-variables.ps1

# Install Azure PowerShell module if not already installed
if (-not (Get-Module -ListAvailable Az))
{
    Install-Module -Name Az -AllowClobber -Scope CurrentUser
    Import-Module -Name Az
}

# Connect to your Azure account
Connect-AzAccount

# Set the Azure subscription context
Set-AzContext -SubscriptionId $subscriptionId

# Create a new resource group
New-AzResourceGroup -Name $resourceGroupName -Location $location

# Create a new Service Bus namespace
New-AzServiceBusNamespace -Name $namespaceName -ResourceGroupName $resourceGroupName -Location $location

# Create a new Service Bus queue
New-AzServiceBusQueue -ResourceGroupName $resourceGroupName -Namespace $namespaceName -QueueName $queueName

# Create the SAS Policy for the queue
New-AzServiceBusAuthorizationRule -ResourceGroupName $resourceGroupName -Namespace $namespaceName -QueueName $queueName -Name $sasPolicyName -Rights 'Manage', 'Send', 'Listen'
