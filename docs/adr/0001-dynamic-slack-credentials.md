# Dynamic Slack credentials use separate async providers

SlackNet will support dynamic Slack credentials with separate async providers for API tokens, app-level tokens, signing secrets, and verification tokens. Static credential methods install constant providers, `WithAccessToken` creates a client-scoped override for both outbound token providers, and SlackNet routes `apps.connections.open` to the app-level token provider so regular API token providers do not need to know that special case.

Outbound providers receive a Slack API call context containing the Web API method and arguments. When the call occurs while handling an incoming Slack request, the context also exposes that originating request and its request type, app ID, workspace ID, and enterprise ID. Independent calls have no originating request.

## Considered Options

- A single provider returning all credentials was rejected because each credential is optional and has a different lifecycle.
- A single API token provider with app-level fallback was rejected because custom providers could accidentally lose socket mode behavior.
- Provider contexts exposing raw request bodies, headers, submitted verification tokens, or untyped request objects were rejected because they invite unclear or unsafe implementations.

## Consequences

Inbound validation providers receive the same request context used by request handlers. Its request type, API app ID, team ID, and enterprise ID are populated before validation. URL verification has its own request type because those identifiers are not available there. Parsed request identity used before signature validation is untrusted and only suitable for selecting candidate validation credentials.
