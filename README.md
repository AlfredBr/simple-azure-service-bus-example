# simple-azure-service-bus-example
This is the simplest Azure Service Bus example I could dream up.

It is just two console apps.  

* *sender* sends the current time, a random 5 character string and an index to a Azure Service Bus queue.
* *receiver* receives the message from the queue and removes it from the queue.
* *shared* contains Config.cs to which you must add your endpoint _connectionstring_ and _queue name_.

