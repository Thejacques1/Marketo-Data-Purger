# Delete Marketo Opportunities

![CircleCI](https://img.shields.io/circleci/build/github/Thejacques1/Marketo-Opportunity-Purger/develop.svg)

Provide the OpportunityRoleId's and OpportunityId's and this app will remove all Opportunities and Opportunity Roles from Marketo via the Marketo API.

The Marketo support and development team provide no means at this stage to delete All Opportunities and Opportunity Roles from a Marketo instance from within the Marketo UI. The solution provided is to re-provisioning the Marketo instance. Doing so would force the customer to lose all custom fields, campaigns and velocity scripts. This is less than ideal.

This simple .Net Core app uses Dapper to access your Marketo OpportunityRoleId's and OpportunityId's stored as provided by Marketo at time of creating Opportunities and associating them with Leads.

## Prerequisites

* REST API Url, ClientId and Secret for Marketo instance (Follow steps here: [link](https://developers.marketo.com/rest-api/))
* Database Connection String
* Database SQL query toaccess Opportunity and OpportunityRole ID's

## Getting Started

* Populate appsettings.json with Marketo and database configuration sections
* Populate sql query in DatabaseRepository.cs

### Built With

* [.Net core 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1) - The developer platform
* [Dapper](https://github.com/StackExchange/Dapper) - Database ORM

### Disclaimer

This will break your Marketo Daily API limit. Speak to your account manager about breaking the limit temporarily first.

The app can be optimized further but my situation didn't require it. 
It current does chunks/batches of 300 records over 10 concurrent API requests (and sleeps the thread for 21 seconds) to stay within the 10 concurrent API request and 100 API requests per 20 seconds limit of the Marketo API. The purging logic could be optimized further using locking to work right below the limit of 100 API requests per 20 seconds, however, this is only neccesary if you have a large volume of Opportunities and you have time constraints.

### Future work

As this is only used for a once off run I have skipped Unit tests. If I see anyone else use this I would be happy to extend it.
This app can also be extended to include the removal of Lead records from Marketo.

### Stats

350 000 odd Opportunities and 350 000 odd OpportunityRoles take roughly 3-5 hours to complete depending on the performance of the Marketo API at the time.

### Authors

* **Jacques Marais** - *Initial work* - [TheJacques1](https://github.com/Thejacques1/)
