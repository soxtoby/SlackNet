# SlackNet

SlackNet is a .NET library for receiving Slack requests and calling Slack APIs.

## Language

**Slack credential provider**:
A user-supplied component that resolves one optional Slack app credential for a specific request or API call.
_Avoid_: Token provider

**Slack request identity**:
The request data SlackNet can safely expose before or after request validation to help select app credentials.
_Avoid_: Account, tenant

**Slack API credential provider**:
A user-supplied component that resolves an outbound Slack API credential from the Slack API request being made.
_Avoid_: Request token provider

**Slack API call context**:
The outbound Slack Web API method and arguments SlackNet exposes before sending a Slack API call, together with the originating Slack request when one exists.
_Avoid_: Slack request identity

**Slack credential provider chain**:
An ordered set of Slack credential providers where a provider can return a credential or defer to the next provider.
_Avoid_: Provider replacement

**Slack request validation credential provider**:
A user-supplied component that resolves an inbound request validation credential from Slack request data.
_Avoid_: API token provider

**Slack request context**:
The request-scoped state SlackNet keeps available while validating and handling an incoming Slack request.
_Avoid_: API request context

**Untrusted Slack request identity**:
Parsed Slack request fields used to select validation credentials before request validation succeeds.
_Avoid_: Verified identity

## Relationships

- A **Slack credential provider** uses **Slack request identity** to resolve one credential type.
- A **Slack API credential provider** uses **Slack API call context**, including the originating **Slack request identity** when one exists.
- A **Slack request validation credential provider** requires **Slack request identity**.
- A signing secret provider, verification token provider, and request handler share the same **Slack request context**.
- A **Slack request context** contains **Untrusted Slack request identity** before signature validation succeeds.
- A **Slack request context** should not expose raw bodies, headers, submitted verification tokens, or untyped request objects as credential-provider input.
- Verification token providers return the expected verification token; SlackNet compares it to the submitted token internally.
- Signing secret providers return the expected signing secret; SlackNet computes and compares the Slack signature internally.
- Provider method names should include the credential name, not use generic `GetToken`.
- A fixed Slack credential is represented as a **Slack credential provider** that ignores context.
- A **Slack credential provider chain** preserves default credential behavior when users add conditional providers.
- A helper base class can preserve built-in token selection for custom **Slack API credential provider** implementations.

## Example dialogue

> **Dev:** "Can the Slack credential provider use the team ID for every request?"
> **Domain expert:** "Only when Slack supplied it; before signature validation, app ID or endpoint mapping is safer."

## Flagged ambiguities

- "token provider" sounds API-token-only, but the design also covers signing secrets and verification tokens; resolved as **Slack credential provider**.
- A single provider returning all credentials would couple optional credentials; resolved as one provider per credential type.
- API tokens and app-level tokens are outbound credentials, while verification tokens and signing secrets are inbound validation credentials; resolved as separate provider categories.
- Static credential configuration and dynamic credential configuration should share provider infrastructure; resolved as fixed values being constant providers.
- App-level tokens are outbound API credentials for `apps.connections.open`; resolving them through the same API-token infrastructure creates precedence questions with user providers.
- Custom API token providers should not need to know SlackNet's built-in `apps.connections.open` app-level token rule.
- Provider context should describe the current request, not include the selected default token.
- Slack API calls made while handling a Slack request expose that originating request to credential providers.
- Slack API calls made independently have no originating Slack request.
- Helper base classes should not require custom providers to pass SlackNet configuration through constructors.
- Regular API-token resolution and app-level-token resolution are distinct outbound credential decisions.
- SlackNet selects the app-level token provider for `apps.connections.open`; regular API token providers do not handle that routing.
- `WithAccessToken` creates a client-scoped override for both outbound credential providers.
- Slack credential providers resolve credentials asynchronously.
- Missing inbound validation provider means that validation mode is disabled.
- A configured inbound validation provider returning no credential means request validation fails.
- A configured outbound credential provider returning no credential means the Slack API request is sent without a bearer token.
- URL verification follows the same inbound validation path unless event URL verification is disabled.
- URL verification has its own request type because SlackNet cannot provide the event identifiers available on event callbacks.
- Existing string credential configuration methods install constant async providers.
- Dependency-injection packages may expose provider type overloads in addition to instance and factory overloads.
- Credential provider calls use the cancellation token from the operation that needs the credential.
- Credential provider exceptions propagate to the operation that requested the credential.
