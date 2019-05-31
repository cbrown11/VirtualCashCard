# Virtual Cash Card



## Running Virtual Cash Card

1. Run CIBuild.bat to Build and test 
2. Run StartAll.bat to start the application (


## Future

1. Create Greg Young EventStore, and allow to be used. Instead of In memory EventStore. Switchable via config file.
2. Create a GraphlQL Gateway (with pub/sub). The quick virtual ATM machine could be changed to use this instead. ATDD driven by Specflow.
3. Create a better view or emulator in React/View/Angular, which will use GraphlQL Gateway. Can use Storybook if using react for testing
4. Create optional Message Transporter (RabbitMQ, SQLServer, MSMQ). Switchable via config file.