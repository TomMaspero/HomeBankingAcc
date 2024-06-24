# HomeBankingAcc

----

This project seeks to emulate a HomeBanking Web Application made using the ASP.NET Core 8 Framework and Microsoft SQL Server as a database engine.

## Functionalities:

- **Register and log in:** a client can sign up using their name, an email and a password. Upon registration, a first bank account will be created automatically.
- **Managing bank accounts**: A client can create up to two bank accounts, plus the one that's created automatically for a total of three. The client can then keep track of each of the account's balance and their incoming and outgoing transactions.
- **Managing Credit and Debit cards**: A client can create a combination of credit and debit cards and access their info at anytime. Both the cards number and their Security Code are generated randomly.
- **Transactions**: A client can also create transactions between different accounts, sending money to both owned and third party accounts. Upon doing so the corresponding transaction will be created on each account for tracking purposes.
- **Loans**: Finally, clients can apply for, and keep track of, a set of pre-approved loans, which will instantly update the balance on the selected account, generating the corresponding incoming transaction into their account. 

## Structure:

Repository layer: The repository layer is the abstraction layer between the access to the data and the business logic provided by the service layer.

Service layer: The service layer handles the business logic by interacting with the Data granted by the repository layer. 

Controller layer: controlles handle the incoming HTTP requests and call upon the corresponding services to send the correct response to the user.

### Developed by: Tomás Máspero