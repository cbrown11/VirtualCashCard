# Virtual Cash Card

Development using Outside-in test-driven development, bearing in mind the CQRS architecture (see DDD with GraphQL Example.pdf) and heading in that direction. 

At moment this only covers the command and and in memory Domain repository.

## Running Virtual Cash Card

version .net core 2.2 is required

1. Run CIBuild.bat to Build and test 
2. Run StartAll.bat to start the application


## Future

1. Domain Repository - Use Greg Young’s database for the event sourcing. Instead of In memory EventStore. Switchable via config file.
2. Create a GraphlQL Gateway (with pub/sub). The quick virtual ATM machine could be changed to use this instead. ATDD driven by Specflow. (Including Playground and Voyager)
3. Create a better view or emulator in React/View/Angular, which will use GraphlQL Gateway. Can use Storybook if using react for testing
4. Use a proper Message Transporter (RabbitMQ, SQLServer, MSMQ). Switchable via config file.
5. Create a read model using ElasticSearch. Though could use projections from Greg Young’s database (though not recommended )