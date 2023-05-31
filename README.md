# simple-azure-service-bus-example
This is the simplest Azure Service Bus example I could dream up.

*simple-queue* is just two console apps and a small class library.

* *sender* sends the current time, a random 5 character string and an index to a Azure Service Bus queue.
* *receiver* receives (and removes) the message from the queue.
* *shared* is a library containing Config.cs to which you must add your endpoint _connection string_.

*topic-queue* is also a pair of console apps and a small class library.

* *publisher* sends the current time, a random 5 character string and an index to a Azure Service Bus queue, but also sets a _MessageTopic_ property to either _even_ or _odd_ depending on if the index is an even or an odd number.
* *subscriber* subscribes to a topic with the name _even_ or _odd_ and will receive messages from that topic only.
* *shared* is a library containing Config.cs to which you must add your endpoint _connection string_.

You'll have to configure your Azure Service Bus accordingly in order for this sample application to work correctly, but that should be fairly easy.  I've added PowerShell scripts to create and delete the queue's in Azure.  Just add your Azure Subscription Id in _azure-variables.ps1_ and you should be good to go...

Here are some hints...

1. Create a queue named *demo-asb-queuename*.
2. Create a topic named *demo-asb-topicname*.
3. Create two subscriptions, one named *even* and the other named *odd*.
4. The *even* subscription should have a custom property filter with a key named "messageTopic" and a value "even".
5. The *odd* subscription should have a custom property filter with a key named "messageTopic" and a value "odd".
6. Both subscriptions should have a system property named "contentType" with a value of "text/string".

When your Azure Service Bus is properly set up...

Run the *publisher* and it will begin publishing into the topic queue, sending messages into both the "even" and "odd" subscriptions.

Run the *subscriber* with the argument "even" or "odd" to subscribe to one of the subscriptions.

> For example, in the subscription folder:
>
> **dotnet run --subscribe even** will select the "even" subscription.
>
> **dotnet run --subscribe odd** will select the "odd" subscription.
