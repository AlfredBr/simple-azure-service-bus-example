# simple-azure-service-bus-example
This is the simplest Azure Service Bus example I could dream up.

*simple-queue* is just two console apps and a small library.

* *sender* sends the current time, a random 5 character string and an index to a Azure Service Bus queue.
* *receiver* receives (and removes) the message from the queue.
* *shared* is a library containing Config.cs to which you must add your endpoint _connection string_ and _queue name_.

