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
New-AzResourceGroup -Name $resourceGroupName -Location $location

# Create a new Service Bus namespace
New-AzServiceBusNamespace -Name $namespaceName -ResourceGroupName $resourceGroupName -Location $location

# Create a new Service Bus queue
New-AzServiceBusQueue -ResourceGroupName $resourceGroupName -Namespace $namespaceName -QueueName $queueName

# Create the topic
New-AzServiceBusTopic -ResourceGroupName $resourceGroupName -NamespaceName $namespaceName -Name $topicName

# Create the SAS Policy for the topic
New-AzServiceBusAuthorizationRule -ResourceGroupName $resourceGroupName -Namespace $namespaceName -TopicName $topicName -Name $sasPolicyName -Rights 'Manage', 'Send', 'Listen'

# Subscriptions are created at the topic level
New-AzServiceBusSubscription -ResourceGroupName $resourceGroupName -NamespaceName $namespaceName -TopicName $topicName -Name $odd
New-AzServiceBusSubscription -ResourceGroupName $resourceGroupName -NamespaceName $namespaceName -TopicName $topicName -Name $even

# Create the filter rules
New-AzServiceBusRule -ResourceGroupName $resourceGroupName -NamespaceName $namespaceName -TopicName $topicName -SubscriptionName $odd -Name $($odd+$rule) -FilterType CorrelationFilter -ContentType "text/string" -CorrelationFilterProperty @{MessageTopic='odd'}
New-AzServiceBusRule -ResourceGroupName $resourceGroupName -NamespaceName $namespaceName -TopicName $topicName -SubscriptionName $even -Name $($even+$rule) -FilterType CorrelationFilter -ContentType "text/string" -CorrelationFilterProperty @{MessageTopic='even'}

# Get the SAS Policy key
$sasPolicyKey = Get-AzServiceBusKey -ResourceGroupName $resourceGroupName -NamespaceName $namespaceName -TopicName $topicName -Name $sasPolicyName

# Get the Primary Connection String
$primaryConnectionString = $sasPolicyKey.PrimaryConnectionString

# store the connection string in an object
$connectionStringObj = @{connectionString=$primaryConnectionString; queueName=$queueName; topicName=$topicName}

# save the connection string to a file
ConvertTo-Json -InputObject $connectionStringObj | Out-File -FilePath appsettings.json -Encoding ascii

# copy the appsettings.json file to the simple-queue folder
Copy-Item -Path appsettings.json -Destination publisher\appsettings.json -Force
Copy-Item -Path appsettings.json -Destination subscriber\appsettings.json -Force

# Delete the appsettings.json file
Remove-Item -Path appsettings.json -Force
