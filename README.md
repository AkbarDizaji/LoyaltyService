# LoyaltyService

## UsageUsage
Start with this command in the root of the project(where docker-compos is exist):

```
docker-compose up --build

```

Now go to `http://localhost:8080/` and login with credential which is exist in .env file.
In there create a realms: `public`.
Then create a new client : `Loyal`. Turn *Client authentication* and *Authorization* on and select Standard flow.

Now you can use service form:
`http://localhost:8081/swagger/index.html`

first you should get token from `Get /Users/get-token` and then copy that with `Bearer ` keyword in the Authorize button. Then you can use other endpoints. 

 ## What I did?
This project has been implemented with the following:

- Clean architecture
- Entity Framework as ORM
- SQL Server as database
- Redis for caching
- Xunit for testing
- Keycloak for identity server
- Docker for containerization
- MediatR for separating commands and queries
- Prometheus for monitoring
- Serilog for logging

## Technical debts:
Since this project was in the form of a code challenge, the focus was on brevity and proving concepts. Naturally, there wasn't time to fully implement all features 100% in a one-day timeframe.
If we consider this as a real project with appropriate time, these items should be implemented:

- Tests should be increased. I only wrote tests for two layers to demonstrate how testing can be done. Also load -test, integration test, stress test,.. can be considered in real project.
- Logging, monitoring, and caching should be distributed within the code where needed, but in this project, they are defined as middleware to capture the general aspects of requests.
- A background service for expiring points
- And more...

Overall, this was just a one-day project ;)
