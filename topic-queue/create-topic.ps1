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