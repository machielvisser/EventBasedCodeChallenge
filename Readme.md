# User Registration Library

The goal of this project is to show how a user registration library can look like when based on the event sourcing pattern.
It is implemented in .Net standard (multi targeted) so that it can be used in .Net Core and .Net Framework applications.

## Event Sourcing
The implementation that makes event sourcing happening in this library consists of several components:
* EventRepository: Sets the correct stream identifiers and add encryption to the event store
* CommandProcessor: Rehydrates the aggregate from the event store and applies the provided command
* EventStream: Base class for the aggregates
* QueryProcessor: Rehydrates the aggregate from the event store and applies the provided query

## User Registration
This library provides the following features:
* Register a user
* Update the email address of a registered user
* Delete a user
* Find registered users based on their email address (or a partial)

The library provides 2 external facing interfaces:
* UserRegistrationCommandService: Used to Change the data
* UserRegistrationQueryService: Used to Query the data

### Register a user
When a user is registered the resulting event is persisted in the event store, such that it can be queried later.
The password is entered in plaintext, this should of course be improved by hashing the input immediately.
A restriction is that the email address should be unique in the system.
This is a complicated requirement because event sourcing does not provide immediate consistancy across streams.
Here it is solved by introducing the UserRegistry aggregate that holds the index to the existing users.

### Update the email address of a registered user
The same restriction as before applies here: the email address should be unique.
Another restriction is: before the update becomes effective it should be verified.
The verification is made possible by the library by providing functionality to indicate that the verification has been done.

### Delete a user
The goal here is to delete a user based on a provided user identifier.
After the delete intention is given the data should be actually deleted after 30 days from the system.
In an event sourced system events should never be deleted, and scheduling is also a challenge in an event sourced system.
To solve this all events are encrypted with a unique key per event stream. By deleting the key, the data of the corresponding stream is effectively deleted.
This library only sends the command to the key store to indicate when to delete the encryption key of the event stream.
**Note: the UserRegistry aggregate still holds the email address in the event history after the deletion. A future improvement could be to move this aggregate to an read store.
But to do this reliably a message infrastructure with guaranteed delivery is required. Otherwise it becomes hard or very inefficient to keep the read model up to date.**

### Find registered users based on the email address
It should be possible to query users based on their email address or a part of it.
Using the UserRegistry that was introduced for the uniqueness test a query to find the users.

## CLI
The CLI provides a means of interacting with the library. In the repository the CLI uses mock instances for the event store and the key store.\
To get an overview of the possible command run the CLI without input parameters.

To register a user the following command can be used: `add -e user@domain.com -p password`\
To indicate that the email has been verified: `verify -e user@domain.com`\
To search for a user based on its email address: `search -q User@dom` -> will result in user@domain.com\
