# Models keep the properties SlackNet doesn't know about in ExtraProperties

SlackNet's models derive from `SlackObject`, whose `ExtraProperties` holds anything Slack sends that the model has no property for, so an app can read a new field before SlackNet has a property for it. It is Json.NET's `[JsonExtensionData]`, as `Event.ExtraProperties` already was, declared once on the base instead of on each model. It is read-only: `WriteData` is off, so an app that sends back an object it received, the blocks of a message for example, sends what SlackNet knows about and nothing else.

`ModelLintTests` fails for a public model that has no catch-all, so a new model has to derive from `SlackObject`, and checks that every `SlackObject` keeps an unknown property and doesn't write it back.

## Considered Options

- A property on each model, like `Event` and `MessageSearchResult` have, was rejected: it would be pasted on about 250 models, and nothing would tell the author of the next model to add it.
- Extension data stored outside the model, in a table that the contract resolver fills, was rejected: it needs no change to the models, but then there is no `ExtraProperties` property to find, which is what events already have.
- Writing the extra properties back, as `Event.ExtraProperties` does, was rejected for models: apps send back what they received, and SlackNet shouldn't start sending properties that Slack may not accept as input.
- Adding it only to the models with known gaps (`User`, `Conversation`, `File`) was rejected: the next gap is in another model.

## Consequences

Json.NET fills one extension data member per type, the most derived one, so a model with extension data of its own can't also use `SlackObject`'s: `ExtraProperties` would stay empty. `Event`, `MessageSearchResult`, `Icons`, `ErrorResponse` and `WebApiResponse` keep theirs and don't derive from `SlackObject`, and the lint accepts them. `Event.ExtraProperties` is still written back, as it was. Every other model now holds an empty dictionary, and properties that used to be skipped when reading are parsed and kept.
