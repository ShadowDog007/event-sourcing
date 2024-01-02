# event-sourcing

Event sourcing POC

## Dependencies

- [MartenDB](https://martendb.io/)
- [MassTransit](https://masstransit.io/)
- [FluentValidation](https://docs.fluentvalidation.net/)

## Features

### Configuration

The core library includes a base set of appsettings files which are included in all domain applications
so that you don't have to add boilerplate settings for every application (e.g. development credentials)

The core library supports `appsettings.core.{Environment}.json` to support mulitple different environments.

**TODO: Look into environment variable replacements of setting values, which could be useful if there are lots of shared setting values**

### Aggregates

You can define aggregates by creating a class extending `AggregateBase<TState>`

Aggregates must define a set of `Handle` methods (the name isn't important)
which take sub-types of `Command` as one of the parameters.

These `Handle` methods map the commands into events and add them to the aggregate stream.

These methods can add additional arguments to inject services, or a CancellationToken (if your handler is async).

Source generators create code for the following:

- Handler implementation, including validation and service resolution.
- Registration of MassTransit command consumer (for commands via message broker)
- Registration of API endpoints to handle commands and fetch the aggregate state

### Projections

You can define projections by creating a class extending `AggregateProjection<TState>`.

Your projection should have an `Apply` method for every event created by the matching `AggregateBase<TState>` implementation.

Preferably the `Apply` method should use this signature `TState Apply(TState current, TEvent @event)`
and the implementation should call `=> current.Apply(@event) with { /* Any updates from the event */ }`.
The `.Apply(@event)` will set the properties which are common between `Event` and `VersionedModel` (used for `TState`).
And with all models being records with `{ get; init; }` properties to encourage immutable models to avoid bugs due to mutation of state.

**TODO: Implement source generator which identifies the event types created by the aggregate and automatically forces implementation of Apply methods**

## Considerations

- Application aggregate models/commands are exposed via API's so care must be taken to ensure there are no breaking changes.

## TODO

- Health check endpoints
- OpenTelemetry
- Refine API endpoints
- Refactor Commands/Events/Models into separate library
- Source generation of API clients.
  - Note: May need to refactor the api generation to not rely on aggregate type
- Backend for frontend (GraphQL?) with searching capbility (ElasticSearch?)
- Finish docker-compose setup
