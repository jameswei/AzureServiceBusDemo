# AzureServiceBusDemo
Demo for azure service bus using [masstransit open source framework](https://masstransit-project.com/)

Solution structure :
- `App` : console application net core 3.1
- `Lib` : class library net std 2.0

Demo will start 2 hosted services :
- `BusHostedService` : responsible of creating/initializing bus
- `FakeSenderHostedService` : responsible of publishing fake message to bus

**`Tools`** : vs19, net core 3.1, masstransit 6, nlog 4.6