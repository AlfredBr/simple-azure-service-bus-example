# simple-azure-service-bus-example
This is the simplest Azure Service Bus example I could dream up.  

_(All code is written in C#.  The Azure config scripts are written in PowerShell.)_

You'll have to configure your Azure Service Bus accordingly in order for this sample application to work correctly, but that should be fairly easy.  I've added PowerShell scripts in each folder to create and delete the queue's in Azure.  

#### Just set the 'azure-subscription-id' to your Azure Subscription Id (it is read in on line #2 of _azure-variables.ps1_).
#### Run the _create-queue.ps1_ script.

Here are some hints...

1. Create a queue named *demo-asb-queuename*.
2. Create a topic named *demo-asb-topicname*.
3. Create two subscriptions, one named *even* and the other named *odd*.
4. The *even* subscription should have a custom property filter with a key named "messageTopic" and a value "even".
5. The *odd* subscription should have a custom property filter with a key named "messageTopic" and a value "odd".
6. Both subscriptions should have a system property named "contentType" with a value of "text/string".

---

#### *simple-queue* is just two console apps and a small class library.
- *sender* sends the current time, a random 5 character string and an index to a Azure Service Bus queue.
- *receiver* receives (and removes) the message from the queue.
- *shared* is a library containing Config.cs to which you must add your endpoint _connection string_.

Run both the **sender** and the **receiver** with "dotnet run"

---

#### *topic-queue* is also a pair of console apps and a small class library.
- *publisher* sends the current time, a random 5 character string and an index to a Azure Service Bus queue, but also sets a _MessageTopic_ property to either _even_ or _odd_ depending on if the index is an even or an odd number.
- *subscriber* subscribes to a topic subscription with the name _even_ or _odd_ and will receive messages from that subscription only.
- *shared* is a library containing Config.cs to which you must add your endpoint _connection string_.

Run the *publisher* and it will begin publishing into the topic queue, sending messages into both the "even" and "odd" subscriptions.

**dotnet run --subscribe even** will select the "even" subscription.

Run the *subscriber* with the argument "even" or "odd" to subscribe to one of the subscriptions.

**dotnet run --subscribe odd** will select the "odd" subscription.



