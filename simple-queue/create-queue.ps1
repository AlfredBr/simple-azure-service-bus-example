try {
    . ../azure-variables.ps1
} catch {
    exit
}

if ($null -eq $azureSubscriptionId)
{
	Write-Output "Please set the 'azure-subscription-id' environment variable."
	exit
}

$subscriptionId = $azureSubscriptionId.Value

# verify that $subscriptionId is set
if ([string]::IsNullOrEmpty($subscriptionId)) {
    Write-Output "Please set the 'azure-subscription-id' environment variable."
    exit
}

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
New-AzResourceGroup -Name $resourceGroupName -Location $location -Force

# Create a new Service Bus namespace
New-AzServiceBusNamespace -Name $namespaceName -ResourceGroupName $resourceGroupName -Location $location

# Create a new Service Bus queue
New-AzServiceBusQueue -ResourceGroupName $resourceGroupName -Namespace $namespaceName -QueueName $queueName

# Create the SAS Policy for the queue
New-AzServiceBusAuthorizationRule -ResourceGroupName $resourceGroupName -Namespace $namespaceName -QueueName $queueName -Name $sasPolicyName -Rights 'Manage', 'Send', 'Listen'

# Get the Service Bus Authorization Rule (not needed for this demo)
Get-AzServiceBusAuthorizationRule -ResourceGroupName $resourceGroupName -Namespace $namespaceName -QueueName $queueName -Name $sasPolicyName

# Get the SAS Policy key
$sasPolicyKey = Get-AzServiceBusKey -ResourceGroupName $resourceGroupName -NamespaceName $namespaceName -QueueName $queueName -Name $sasPolicyName

# Get the Primary Connection String
$primaryConnectionString = $sasPolicyKey.PrimaryConnectionString

# store the connection string in an object
$connectionStringObj = @{connectionString=$primaryConnectionString; queueName=$queueName}

# save the connection string to a file
ConvertTo-Json -InputObject $connectionStringObj | Out-File -FilePath appsettings.json -Encoding ascii

# copy the appsettings.json file to the simple-queue folder
Copy-Item -Path appsettings.json -Destination sender\appsettings.json -Force
Copy-Item -Path appsettings.json -Destination receiver\appsettings.json -Force

# Delete the appsettings.json file
Remove-Item -Path appsettings.json -Force
