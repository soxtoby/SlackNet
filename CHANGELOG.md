# Changelog

## Unreleased

### Added
- `Conversation.Properties` and `ConversationProperties.Tabs` expose conversation tab metadata.

## v0.20.0

### Breaking
- Removed `ISlackTypeResolver`, along with `UseTypeResolver`, `ISlackServiceProvider.GetTypeResolver`, `SlackServiceBuilder.GetTypeResolver`, `Default.SlackTypeResolver` and `Default.AssembliesContainingSlackTypes`. `Default.JsonSettings` no longer takes a type resolver. Use `ReplaceSlackType` or `RegisterSlackType` to deserialize your own types.

### Added
- `ReplaceSlackType<TSlackType, TReplacement>()` deserializes your own type wherever SlackNet would create one of its own, so you can add properties SlackNet doesn't have yet.
- `RegisterSlackType<TSlackType>(typeIdentifier)` deserializes Slack types that SlackNet doesn't have yet.
- `RegisterFunctionHandler` handles executions of a single custom function by callback ID, with optional typed input values.
- `AgentSessionTitleChanged` event.
- `ContextTeamId`, `Updated`, `IsOpen`, `IsFrozen`, `SharedTeamIds`, `InternalTeamIds`, `ConnectedTeamIds` and `PendingConnectedTeamIds` on `Conversation`.
- `UserTeam`, `SourceTeam`, `DisplayAsBot`, `MediaDisplayType` and `HasMoreShares` on `File`.
- `IsEmailConfirmed`, `WhoCanShareContactCard` and `IsWorkflowBot` on `User`, and `DisplayNameNormalized`, `Image1024` and `IsCustomImage` on `UserProfile`.
- `ParentUserId` and `Subscribed` on `MessageEvent`.
- `ContextTeamId` and `ContextEnterpriseId` on `EventCallback`, and `IsEnterpriseInstall` on `Authorization`.
- `Team` and `Enterprise` on `MemberJoinedChannel`, and `Team` on `MemberLeftChannel`.

### Fixed
- `channel_archive`, `channel_unarchive`, `group_archive` and `group_unarchive` events are deserialized as `ChannelArchive`, `ChannelUnarchive`, `GroupArchive` and `GroupUnarchive` events, rather than as message subtypes.

Thankyou to @we4sz for his help with this release 💪

## v0.19.0

This release adds support for custom workflow functions and dynamic credential providers (which should make it easier to support multiple Slack apps and workspaces). Both of these required some subtle breaking changes, so be sure to check the section below.

Custom step functions that return dynamic options need to complete _before_ acknowledging the event, so I've introduced an experimental `IEventHandler<T>` interface, which provides access to the typed event, the `EventCallback`, and a manual `Acknowledge` function. This can be used for any event type though, so you might find it useful outside of custom step functions.

- Added dynamic credential providers:
  - `UseApiTokenProvider` for Web API requests.
  - `UseAppLevelTokenProvider` for app-level API requests.
  - `UseSigningSecretProvider` and `UseVerificationTokenProvider` for ASP.NET Core request validation.
- Added request type, app, workspace and enterprise IDs to `SlackRequestContext`. API token providers also receive the API method, arguments and originating request.
- Added experimental context-aware event handlers, with callback metadata and control over acknowledgement.
- Added support for custom workflow functions:
  - `FunctionExecuted` event.
  - `FunctionsApi.CompleteSuccess` and `CompleteError`.
  - `FunctionDistributionPermissionsApi`.
  - `FunctionWorkflowStepsApi`, including form-response exports.
  - `WorkflowsFeaturedApi` and `WorkflowTriggerPermissionsApi`.
- Added function execution data, bot access tokens and interactivity pointers to interaction requests. Block option requests also expose function execution data and bot access tokens.
- Added interactivity-pointer support to `ViewsApi.Open` and `Push`.
- Added `AgentsSessionsApi.SetStatus` and `AgentSessionStopped`.
- Added chunks, task display mode, icon and username overrides to message streaming. `StopStream` also supports setting the agent session status.
- Added `TaskCardBlock`, `PlanBlock`, `CardBlock`, `CarouselBlock`, `ContainerBlock`, `AlertBlock`, `DataTableBlock` and `DataVisualizationBlock`, with supporting icons, URL sources and chart types.
- Added `RawNumberCell` and `RichTextPreformatted.Language`.
- Added `AppContextChanged`, `AppHomeOpened.Context` and `MessageEvent.AppContext`.
- Added `MessageDeleted.PreviousMessage`, and `UserTeam` and `SourceTeam` to message events.
- Added `MetadataJson` and `MetadataObject` to `MessageUpdate`.
- Added `BotId` and `EnterpriseId` to `AuthTestResponse`.
- Added `codeVerifier` to `OAuthV2Api.Access` for PKCE.
- Added an `AssistantThreadsApi.SetSuggestedPrompts` overload without a thread timestamp, for the Messages tab.
- Date-like strings remain strings when deserializing untyped JSON.
- ASP.NET Core only creates the hosted Socket Mode client when enabled, and cleans up request scopes when handlers fail.

### Breaking changes

- Renamed `ISlackApiClient.AppsConnectionsApi` to `AppsConnections`.
- Removed deprecated configuration from `SlackEndpointConfiguration`:
  - Move `UseSigningSecret`, `VerifyWith`, `UseEventUrlVerification` and `UseSocketMode` into the `AddSlackNet` callback.
  - Pass connection options to `UseSocketMode`.
  - Keep `MapToPrefix` and `DelayResponse` in `UseSlackNet`. `UseSocketMode` now returns the service configuration.
- Removed `ISlackRequestValidationConfiguration`. Custom validation configuration should use `ISlackSigningSecretProvider` and/or `ISlackVerificationTokenProvider`.
- Configured signing secrets and verification tokens returning `null` or an empty string now reject requests.
- `ReplaceEventHandling`, `ISlackHandlerFactory.CreateEventHandler` and `SlackRequestContext.EventHandlers` now use `SlackNet.Handlers.Experimental.IEventHandler<Event>`. Implement `Handle(IEventContext<Event>)`, or adapt existing handlers with `.ToEventHandler()`. Existing `RegisterEventHandler` registrations continue to work.
- `ISlackServiceProvider` implementations must provide `GetApiTokenProvider()` and `GetAppLevelTokenProvider()`.
- New parameters precede `cancellationToken` in `OAuthV2Api.Access` and `ChatApi.StartStream`, `AppendStream` and `StopStream`. Pass cancellation tokens by name, e.g. `cancellationToken: token`, and rebuild dependent projects.
- Set `SlackRequestContext.RequestId` through its initializer; setting the corresponding dictionary entry no longer updates it.
- Custom API clients must supply their own app-level token for Socket Mode to work.
- Code relying on automatic date conversion in untyped JSON should parse date strings explicitly.
- Deprecated trigger-first `ViewsApi.Open` and `Push` overloads. Use `Open(view, triggerId: triggerId)` and `Push(view, triggerId: triggerId)`.
- Deprecated the `SlackApiClient` constructor accepting HTTP dependencies and a token string. Use the constructor accepting separate API-token and app-level-token providers.

A big thanks to @enhancer13, @vgribok-aws, and @we4sz for their help with this release 🥳


## v0.18.0

- More robust connection handling for socket mode - fixes an issue where socket mode could silently stop working after connections failed.

### Breaking changes
- `ISlackSocketModeClient` and `ICoreSocketModeClient`:
  - Now extends `IAsyncDisposable`.
  - Added `DisconnectAsync()` method.
  - Existing `Disconnect()` marked as obsolete.


## v0.17.11

- Added RichTextAttachmentMention element.
- Added RichTextStyle.Underline.

Thankyou to @AlexanderMakarov124 for this release 🌟


## v0.17.10

- Added `UseSocketMode` to config in `AddSlackNet` callback and deprecated it inside `UseSlackNet`.


## v0.17.9

- Removed dependency on `Microsoft.Bcl.AsyncInterfaces` on .NET 8.
- Added `loadingMessages` parameter to `AssistantThreadsApi.SetStatus`.

Thankyou to @verdie-g for his help with this release 👍 


## v0.17.8

- Added `FeedbackButtonsAction` and `IconButtonAction` types.


## v0.17.7

- Changed `Attachment.IsAppUnfurl` to a nullable bool to align with other bool properties.


## v0.17.6

- Added `StartStream`, `AppendStream`, and `StopStream` methods to `ChatApi`.
- Added `Info` method to `AssistantSearchApi`.
- Added `ToolingApi` with `RotateToken` method.
- Added support for work objects
  - Added `EntityApi` with `PresentDetails` method.
  - Added `EntityDetailsRequested` event.
  - Added a number of new properties to attachments and interaction requests and containers.
  - Added support for including work object metadata in `ChatApi.Unfurl` and `Chat.PostMessage` methods.


## v0.17.5

- Added full suite of list APIs:
  - `ListApi`
  - `ListAccessApi`
  - `ListDownloadApi`
  - Remaining methods in `ListItemsApi`
- Added `ContextActionsBlock` and `TableBlock` block types.
- Updated all Slack documentation links to point to [the new docs site](https://docs.slack.dev/).

### Potentially breaking change
The `ListItemsResponse` class has been moved from the `SlackNet` namespace to the `SlackNet.WebApi` namespace to align with other response classes.


## v0.17.4

- Fixed doubling up of '/' at beginning of path when `RoutePrefix` is empty.


## v0.17.3

- `ErrorResponse.Info` ignores properties that aren't strings, rather than throwing an exception.


## v0.17.2

- More details in `SlackException`s thrown when Slack returns an error response.
- Removed deprecated `asUser` parameter from `ChatApi.Delete`.
- Added `ToDateTime` and `ToTimestampNumber` extensions for converting between long timestamps and `DateTime`s.
- Changed `RichTextDate.Timestamp` to a `long` and added `Date` wrapper property.


## v0.17.1

- Added `Canvas` to `FileType` enum.

Thankyou to @harayuu9 for this release ➕ 


## v0.17.0

- Added a bunch of missing APIs:
  - `AssistantSearchApi`
  - `CanvasesApi`
  - `ExternalTeamsApi`
  - `ListItemsApi`
- Added a bunch of methods to `ConversationsApi`: 
  - `CreateCanvas`
  - `SetExternalInvitePermissions`
  - `ApproveRequestedShareInvite`
  - `DenyRequestedShareInvite`
  - `ListRequestedShareInvites`
- Added `teamId` and `showFilesHiddenByLimit` parameters to `FilesApi.List`.
- Added `OAuthV2Api.Exchange` method.
- Added `UsersApi.LookupDiscoverableContact` method.
- `BookmarksApi.Edit` calls the correct Slack API method.

### Breaking changes
Removed support for workflow steps, which were [deprecated nearly a year ago](https://api.slack.com/changelog/2023-08-workflow-steps-from-apps-step-back).

###
Thankyou to @ynaftali-island for his help with this release 💪


## v0.16.2

- Fewer dependencies on .NET 8 and above.
- Added `AuthApi.TeamsList` method.
- Fixed an `InvalidOperationException` error that occurred when a web socket connection was closed after another error.

Thankyou to @Hawxy and @ynaftali-island for their contributions to this release 🥳


## v0.16.1

- Fixed web socket reconnection when Slack closes the connection.


## v0.16.0

This release replaces the web socket implementation, which should fix socket mode in .NET Framework.

### Breaking changes
- Replaced WebSocket4Net dependency with System.Net.WebSockets.
- `WebSocketWrapper` constructor now takes in a System.Net.WebSockets `ClientWebSocket` and `uri` string.
- `IWebSocket.Send` now returns a `Task`.
- `ICoreSocketModeClient.Send` now returns a `Task`.


## v0.15.5

- Added `Expand` property to `SectionBlock`.
- Added `MarkdownBlock` type.


## v0.15.4

- Added `includeAllMetadata` parameter to `ConversationsApi.Replies`.
- Removed Microsoft.AspNetCore.Mvc.Abstractions dependency from SlackNet.AspNetCore and added it to SlackNet.AzureFunctions.


## v0.15.3

- Fixed issue where calling `FilesApi.CompleteUploadExternal` manually would flip the `threadTs` and `initialComment` arguments.


## v0.15.2

- Fixing the name of `OpenIdApi.Token`'s `redirect_uri` parameter.
- Assistants API support.
  - Added `AssistantThreadStarted` and `AssistantThreadContextChanged` events.
  - Added `AssistantThreadsApi` with `SetStatus`, `SetSuggestedPrompts`, and `SetTitle` methods.
  - Added `AssistantAppThread` message event, which appears in `MessageChanged` events.
- Added missing `PreviousMessage` property to `MessageChanged` event.

Thankyou to @brb3 for his help with this release 🛠️


## v0.15.1

- Added microseconds to `DateTime`s produced by the `ToDateTime` helper. This makes sorting messages by timestamp more reliable, but `Ts` strings should still be used to identify messages, rather than using `DateTime`s.


## v0.15.0

### Breaking changes
`CancellationToken` parameters are no longer nullable. They're still optional, so this should only affect code that was explicitly using `CancellationToken?`.

If you were previously passing in a nullable `CancellationToken`, I'd recommend null-coalescing to either `default` or `CancellationToken.None`. 

If you were specifying the type in testing mocks, e.g. with `Arg.Any<CancellationToken?>()` or `It.IsAny<CancellationToken?>()`, simply remove the `?`. If you were passing in `null` in tests, replace it with `default` or `CancellationToken.None`. If you were asserting that `null` was passed in as the cancellation token, it should be fine to assert on `CancellationToken.None` instead.


## v0.14.0

- Socket mode client (and RTM client) retry initial connection unless Slack returns an error response.
- Improved logging around web socket connections.
- `UseSocketMode` properly disconnects the socket mode client when app is shut down.
- Rate-limited API request retries are cancelled immediately when cancellation is requested.

### Breaking changes
- SlackNet.AspNetCore now depends on Microsoft.Extensions.Hosting.Abstractions.
- Deprecated `Utils.RetryWithDelay` - this was never meant to be public.
- Simplified `IWebSocket` interface and made `Open` async.
- Deprecated `SlackEndpointConfiguration.SocketModeConnectionOptions` setter - the options should be passed into `UseSocketMode` instead.


## v0.13.3

- Updating some dependencies to avoid vulnerable packages:
  - SlackNet now depends on System.Net.Security 4.3.1 and System.Text.RegularExpressions 4.3.1.
  - SlackNet.AspNetCore now depends on System.Text.Encodings.Web 4.5.1.
  - SlackNet.AzureFunctions now depends on Microsoft.Azure.Functions.Worker.Core 1.17.0.


## v0.13.2

- Fixed `UserProfile` deserialization by changing `UserProfile.StatusEmojiDisplayInfo` to a list.
- Added `Unicode` property to `EmojiDisplayInfo`.


## v0.13.1

- Support for new file upload APIs
  - Added `GetUploadUrlExternal` and `CompleteUploadExternal` methods to `FilesApi`.
  - Added `Upload` overloads that take advantage of new upload APIs (see below). The old overloads have been marked as obsolete in favour of the new methods.
  - Added `IHttp` parameter to `FilesApi` constructor. The old constructor has been marked as obsolete, and will use the default `IHttp` implementation.
- Added missing `StatusEmojiDisplayInfo`, `StatusTextCanonical`, `Pronouns`, and `StartDate` properties to `UserProfile`.
- Added `ThreadBroadcast` event. This replaces `ReplyBroadcast`, which will now work again, but has been marked as obsolete.

### Upload API
Slack's new upload API requires at least 3 requests: 1 to get a URL to upload to, 1 to upload the file to that URL, and 1 to complete the upload operation, and the first 2 of those must be repeated for each file being uploaded together.

SlackNet provides two new `Upload` methods that handle these requests for you, providing an API similar to the old upload API. These `Upload` methods take in either a single `FileUpload` object or an `IEnumerable` of them, allowing you to specify the file contents in a number of formats, and return references to the uploaded files in Slack. The `GetUploadUrlExternal` and `CompleteUploadExternal` methods are provided for completeness, but using `Upload` is recommended.


## v0.13.0

- Added `RichTextList.Offset` property.
- Added `RichTextQuote.Border` property.
- Added `RichTextPreformatted.Border` property.
- Added `Highlight`, `ClientHighlight`, and `Unlink` properties to `RichTextStyle`.
- `RichTextStyle` properties aren't serialized if they're false.

### Breaking changes
- `RichTextList.Elements` only accepts `RichTextSection`s.
- `RichTextList.Style` changed from a string to `RichTextListStyle` enum.


## v0.12.3

- Error messages returned from Slack are included in `SlackException`'s message.


## v0.12.2

- Added `SlackFile` property to `Image` element and `ImageBlock`.
- Added `Format` property to `RichTextDate`.
- Added `FileInput` block element.


## v0.12.1

- Added experimental option in `UseSlackNet` to delay responses until after handlers have completed.


## v0.12.0

- `ChatApi.PostEphemeral` correctly returns `PostEphemeralResponse` instead of `PostMessageResponse`.
- Introduced `SlackNet.AzureFunctions` package to properly support running in Azure Functions.

### Azure Functions
Hosting a SlackNet application in Azure Functions is now properly supported with the new SlackNet.AzureFunctions package. Configuration is similar to previous Azure Functions integration with SlackNet.AspNetCore, but the APIs have been simplified, and the isolated worker model is now supported. Additionally, some issues around the function shutting down before a request has finished being handled have been resolved, albeit at the cost of dropping support for early responses (responses will be sent after you've finished handling the request, no matter how early you respond). See the [readme](https://github.com/soxtoby/SlackNet/blob/master/readme.md#slacknetazurefunctions) and [AzureFunctionsExample](https://github.com/soxtoby/SlackNet/tree/master/Examples/AzureFunctionsExample#readme) project for more information.

### Breaking changes
#### SlackNet.AspNetCore
- `VerifyWith`, `UseSigningSecret`, and `UseEventUrlVerification`, called inside `UseSlackNet`, have been marked as obsolete, and will be removed in a future release. These calls should now be made in `AddSlackNet` instead.
- The callback in `AddSlackNet` now takes in an `AspNetSlackServiceConfiguration` instead of a `ServiceCollectionSlackServiceConfiguration`. The API is otherwise compatible.
- `Microsoft.AspNetCore.Http` and `Microsoft.AspNetCore.Http.Abstractions` dependencies have been bumped to versions 2.1.22 and 2.1.1 respectively.
- The methods on `ISlackRequestHandler` no longer take in a `SlackEndpointConfiguration`. The configuration is now injected into the implementation.
- `SlackResult` no longer implements `IActionResult`.

#### Azure Functions
If you're currently using SlackNet.AspNetCore with Azure Functions, you'll need to switch to SlackNet.AzureFunctions, and make the following changes:
- Make sure you're configuring SlackNet with the  `AddSlackNet` extension from `SlackNet.AzureFunctions`.
- Move your `SlackEndpointConfiguration` configuration into `AddSlackNet`, and remove its service collection registration.
- Replace the `ISlackRequestHandler` and `SlackEndpointConfiguration` from your endpoints class with just `ISlackFunctionRequestHandler`, and update its usage to remove the second argument from each method call.
- Change the return types of your function from `Task<SlackResult>` to `Task<SlackFunctionResult>`.

See the [AzureFunctionsExample](https://github.com/soxtoby/SlackNet/tree/master/Examples/AzureFunctionsExample#readme) project for an example.


Thankyou to @matthawley for his help with this release 💪


## v0.11.5

- Fix for invalid parameter error when using an `ExternalMultiSelectMenu`.

Thankyou to @rzezak for this release 🐞🔨


## v0.11.4

- Added `View` property to `BlockOptionsRequest`.
- Added `includeAllMetadata` parameter to `IConversationsApi.History`.

### Potentially breaking change
- If you were passing a `cursor` and/or `CancellationToken` to `IConversationsApi.History` as positional arguments, you'll need to either add `false` as the `includeAllMetadata` argument, or specify the `cursor`/`cancellationToken` parameter names.

Thankyou to @nimoskov for his help with this release ✨


## v0.11.3

- Added optional `teamId` parameter to `IConversationsApi.Create`.
- Added support for message metadata.
- Added `MessageMetadataPosted`, `MessageMetadataUpdated`, and `MessageMetadataDeleted` events.
- Added `BotId`, `AppId`, and `BotProfile` properties to `MessageEvent`.
- Added `TeamId`, `Deleted`, and `Updated` properties to `BotInfo`.
- Added `RichTextInput` and fixed posting messages with `RichTextBlock`s.

### Metadata
When posting a message to Slack, you can specify the metadata in one of two ways:
```c#
await slack.Chat.PostMessage(new Message
    {
        Channel = "#general",
        Text = "My message",

        // Use your own metadata object. It will be serialized with the standard Slack conventions,
        // and the event_type will be the name of the class in snake case.
        MetadataObject = new MyMetadata("my info"),

        // Specify the metadata JSON explicitly. This will take precedence over MetadataObject.
        MetadataJson = new MessageMetadata
            {
                EventType = "my_metadata",
                EventPayload = JToken.Parse("""{ "custom_info": "my info" }""") // Get your JToken from wherever you like  
            }
    });
```
Messages retrieved from Slack will have a `Metadata` property that contains the event type and JSON as a `JToken`. To deserialize the JSON using the standard Slack JSON conventions, use the `ToObject` method on `MessageMetadata`, _not_ the `ToObject` method on `JToken`:
```c#
var myMetadata = message.Metadata.ToObject<MyMetadata>();
```

### Potentially breaking changes
- If you were passing a `CancellationToken` to `IConversationsApi.Create` as a positional argument, you'll need to either add `null` as the `teamId` argument, or specify the `cancellationToken` parameter name.
- Added a `SlackJsonSettings` parameter to the `ChatApi` constructor. If you're constructing `ChatApi` manually, you can pass in `Default.JsonSettings()`, or your own customized settings.
- Pushed the `InitialValue` property of `TextInput` down to its sub-classes, to account for `RichTextInput`'s different type of value.

Thankyou to @jtsai-osa for his help with this release 👍 


## v0.11.2

- Renamed `SnoozeEndTime` on `SnoozeResponse` and `OwnDndResponse` so it deserializes properly.
- Added `SnoozeIsIndefinite` to `SnoozeResponse` and `OwnDndResponse`.


## v0.11.1

- Added missing `IsExtSharedChannel` property to `EventCallback`.
- Incoming requests are logged when using `SlackNet.AspNetCore`.
- Default ASP.NET logger logs with the specific SlackNet category (e.g. `SlackNet.Error` instead of just `SlackNet`).

When using the default logger with ASP.NET, individual SlackNet log categories can be enabled in appsettings.json with:
```json
"Logging": {
  "LogLevel": {
    "SlackNet.Data": "Trace",
    "SlackNet.Serialization": "Trace",
    "SlackNet.Internal": "Debug",
    "SlackNet.Request": "Information",
    "SlackNet.Error": "Error"
  }
}
```
Setting a category to a higher level will filter out logs from that category.


## v0.11.0

- Added the `UserHuddleChanged`, `UserProfileChanged`, and `UserStatusChanged` events.
- Added logging for serialization. Useful for debugging serialization issues, but otherwise I'd recommend filtering out `LogEvent`s in the `Serialization` category.
- Added `Attachments` property to `MessageSearchResult`.
- Added `Unsafe` property to `RichTextLink`.
- Added `ThumbWidth`, `ThumbHeight`, `OriginalUrl`, `ServiceName`, and `ServiceIcon` properties to `Attachment`.

### Breaking Changes
- Removed the stars API and associated classes. See this [Slack announcement](https://api.slack.com/changelog/2023-07-its-later-already-for-stars-and-reminders) for more details.

Thankyou to @fstojanac for his help with this release 💪


## v0.10.24

- Added cursor-based pagination to `TeamApi.AccessLogs` and `FilesApi.List`.
- `ChatApi.PostEphemeral` can post to threads.

Thankyou to @arkolka for their help with this release 🎉


## v0.10.23

- Added `FileIds` to `MessageUpdate` to allow removing and re-adding files when updating a message.
- `SlackApiClient.WithAccessToken()` maintains the value of `DisableRetryOnRateLimit`.


## v0.10.22

- Added `HuddleState` and `HuddleStateExpirationTs` properties to `UserProfile`.

Thankyou to @JoeMyers for this release ➕


## v0.10.21

- Fixed content type of event endpoint challenge.
- Checking content type of request before trying to deserialize it.

Thankyou to @SamuelCharest for his help with this release 🛠️


## v0.10.20

- Added `Link.Email` helper method for creating email links.
- Added `PostToWebhook` method to `SlackApiClient` for posting messages to incoming webhooks.
- Added `IncomingWebhook` property to `OauthV2AccessResponse` for collecting generated webhook URLs.

Thankyou to @TimSon777 for his help with this release 👍


## v0.10.19

- Fixed deserialization of `MessageBlocks` property in attachments.
- `DatePicker.FocusOnLoad` property isn't serialized when false, to avoid a Slack internal error.
- Added `DateTimePicker`, `EmailTextInput`, `UrlTextInput`, and `NumberInput` block elements.

Thanks again to @theteladras for his help with this release 🤝


## v0.10.18

- Added `Border` property to `RichTextList`.
- Singular enum arguments are added to request URLs without quotes (fixes #135).
- Fixing `Search` API response deserialization by changing from `MessageEvent`s to new `MessageSearchResponse` class.

Thank you to @theteladras for his help with this release ✨


## v0.10.17

- Added `ApiAppId` property to `UserProfile`.
- Improved performance of type lookups during deserialization across multiple threads.

Thankyou to @theteladras and @omer-koren for this release 🥇 


## v0.10.16

- Added `Unicode` and `SkinTone` properties to `RichTextEmoji`.
- Added `PinnedTo` and `PinnedInfo` properties to `MessageEvent`.
- Added `DeleteScheduledMessage` method to `ChatApi`.
- Fixed type of `ScheduledMessage.Id` and added missing `Text` property.
- Added `EnterpriseId` property to `EventCallback`.
- Added `TeamAccessGranted` and `TeamAccessRevoked` events.
- When a logger is attached, the default `Http` service will correctly log requests.

Many thanks to @theteladras, @omer-koren, @magnusfox, and @henkli for their help with this release 🥳 


## v0.10.15

- `SlashCommand.TeamName` renamed to `TeamDomain` to match Slack's JSON.
- `DataSource` property added to `SelectElement`.

Thankyou to @ngg for this release ✨


## v0.10.14

- Added team billing & preferences APIs.
- Added support for Slack Connect invitations.


## v0.10.13

- Added `Attachment` properties that appear on message unfurls (shared messages).

Thankyou to @omer-koren for this release ✌️


## v0.10.12

- Fixed deserialization of `EmojiApi.List`'s response so emojis are returned properly.
- Fixed deserialization of `DndApi.TeamInfo`'s response so users' DnD statuses are returned properly.

Thanks to @omer-koren for his help with this release 🛠️


## v0.10.11

- Added `State` to `BlockActionRequest` for when request comes from a message.
- Added `ChannelIdChanged` event.
- Support for unfurling inside message composer and customized unfurl auth message.
- Added `FocusOnLoad` properties to blocks that support it.
- Added `AccessibilityLabel` to `Button` block.

Thanks to @makeitokay for their help with this release 💪


## v0.10.10

- Added bookmarks API.
- Added calls API and call block.
- Added video block.
- Removed unused properties in `AppUninstalled`.
- `Initial*` properties in multi-select menus are not sent to Slack if they're empty. Fixes #114.


## v0.10.9

- Updating Newtonsoft.Json dependency to 13.0.1
- `IUsersApi.SetPresence` takes in a `RequestPresence` enum instead of `Presence`, which has an `Auto` property instead of an `Active` property, as per the Slack API. The old method is still available, but marked as `Obsolete`.


## v0.10.8

- `CoreSocketModeClient` logs and ignores any messages that can't be deserialized
- Indenting extra properties in log event `FullMessageTemplate`


## v0.10.7

- Added optional `teamId` parameter to `IConversationsApi.List`

Thankyou to @jtsai-osa for this release 👍


## v0.10.6

- Added `Subtype` property back to `MessageEvent`, which was accidentally removed in the previous release.


## v0.10.5

- `PlainText.Emoji` is true by default to match Slack's default behaviour when the property isn't specified in the JSON.
- Added `MessageEventBase` base class for `AppMention` & `MessageEvent` with common properties.
- Added `ExtraProperties` property to `Event` to catch any information that Slack sends that doesn't have an existing property.
- Added missing `ClientMsgId` and `Team` properties to `MessageEventBase`.
- Added missing `ChannelType` property to `MessageEvent`. The format doesn't match other `ChannelType` properties, so I've left it as a `string` for now, rather than using the existing `ChannelType` enum.
- Pulled the `EventTs` property found on many event types up to the `Event` base class.


## v0.10.4

- Added missing properties to `OauthV2AccessResponse`.
- `OAuthV2Api.Access` doesn't include authorization header in request.


## v0.10.3

- Fixed content type of `OAuthV2Api.Access`.
- Fixed endpoint for `AppsEventsAuthorizationsApi.List` and renamed API to `AppsEventAuthorizations` to match endpoint.
- Added Slack documentation links to all API methds.


## v0.10.2

- Added AppUninstalled event content

Thankyou to @Kalissaac for this release 👍


## v0.10.1

- Fixing null ref in Http when making GET requests


## v0.10.0

- Added logging.
- Added `SlackRequestContext.Current` to provide static access to the current request context. `Current` throws if not in a request context, so depending on where you use it, you'll want to check `IsAvailable` first.
- Added `RequestId` to `SlackRequestContext`, which provides a unique ID for the current request.
- Added `RequestId` to `RawSocketMessage` and `SocketMessage`.
- Fixes and extra properties for the `OpenIdApi.Token` API.

### Logging
The default logger doesn't do anything, so if you want logging, you'll need to implement the `ILogger` interface and provide it with one of the `UseLogger` configuration methods. If you're using `SlackNet.AspNetCore`, the default logger is overridden with an adaptor for the `Microsoft.Extensions.Logging` logger, which itself can be overridden as necessary.

Implementing `ILogger` should be fairly straightforward - see the [MicrosoftLoggerAdaptor](https://github.com/soxtoby/SlackNet/blob/master/SlackNet.AspNetCore/MicrosoftLoggerAdaptor.cs) for an example. I've provided a range of `ILogEvent` extension methods to adapt log events to a variety of formats. All log events have a `Category` property, and should have a `"Source"` message property, which can be used for filtering and determining the appropriate verbosity.

### Breaking Changes
- `SlackNet.AspNetCore` now depends on the `Microsoft.Extensions.Logging.Abstractions` package.
- `SlackRtmClient` requires an `ILogger` when constructed with specific services.
- `CoreSocketModeClient` requires an `ILogger` when constructed with specific services.
- `SlackSocketModeClient` requires an `ILogger` when constructed.

Thankyou to @objmj-itminds for the Open ID improvements ✨


## v0.9.5

- Sign-in with slack support via Open ID api methods

Thankyou to @objmj-itminds for this release 🎁


## v0.9.4

- Implementation of oauth.v2.access method and required response types
- Adding `Team` property to `MessageEvent`

Thanks to @objmj-itminds for the OAuth API work 🔒 


## v0.9.3

- Added `ChannelId` and `UserId` properties to the `FileShared` event.

Thank you to @objmj-itminds for the addition 🛠


## v0.9.2

- Added optional `includeLocale` parameter to `Info` and `List` in `UsersApi`, and to `Start` in `RtmApi`.
- `SlashCommandResponse`s returned by `ISlashCommandHandler`s are properly passed on to Slack.

#### Potentially breaking changes
While the `includeLocale` parameters are optional, they haven't been added as the last parameter, so on the off chance you were using all of the optional parameters in one of the modified methods, without specifying parameter names, then you'll need to add some parameter names.

Thankyou to @Selm for helping with this release 👍


## v0.9.1

- Fixed null-ref when using socket mode with ASP.NET integration


## v0.9.0

### Socket Mode
This release introduces support for Slack's new [socket mode](https://api.slack.com/apis/connections/socket). Socket mode gives you access to all the available Slack functionality without needing to host a public endpoint, which makes it ideal for testing a new application, or hosting it inside a local network.

### TL;DR
See the [SlackNet.SocketModeExample](https://github.com/soxtoby/SlackNet/tree/master/SlackNet.SocketModeExample) project for example usage. See [Using a DI container](#using-a-di-container) below for example integration code. With a DI container, handler registration is the same as in [SlackNet.EventsExample](https://github.com/soxtoby/SlackNet/blob/master/SlackNet.EventsExample/Startup.cs).

### Using the built-in SlackServiceBuilder
```c#
var slackServices = new SlackServiceBuilder();
slackServices
    .UseAppLevelToken("<app-level OAuth token required for socket mode>")
    .RegisterEventHandler(myEventHandler)
    .RegisterBlockActionHandler(myBlockActionHandler);
var client = slackService.GetSocketModeClient();
await client.Connect();
```

`SlackServiceBuilder` provides registration methods for all the different types of handlers, as well as the ability to replace specific services with custom implementations, should the need arise.

It also provides methods to retrieve any of the SlackNet services, most notably `GetApiClient` and `GetSocketModeClient`.

### The socket mode client
Similar to the RTM client, once connected, the socket mode client will reconnect automatically, until the client is disposed, or socket mode is disabled.

Each message received is treated as a "request", equivalent to receiving a request through the ASP.NET integration. Responses are returned as acknowledgements through the websocket connection.

By default, the socket mode client will open 2 connections, 10 seconds apart, so messages can still be received while one of the connections is being reconnected. You can configure this behaviour by passing a `SocketModeConnectionOptions` object into the client's `Connect` method.

### Using a DI container
The built-in `SlackServiceBuilder` requires services and handlers to be constructed manually, but with a DI container, they can be registered with just a type, leaving construction to the container. Other services registered in the container can be injected as you'd expect, and services and handlers are given appropriate lifestyles.

Since the configuration has been standardised, it's pretty much the same for any container, but here's what it looks like for the provided integrations&hellip;

#### Autofac
Using the [SlackNet.Autofac](https://www.nuget.org/packages/SlackNet.Autofac) package:

```c#
var containerBuilder = new ContainerBuilder();
containerBuilder.AddSlackNet(c => c
    .UseAppLevelToken("<app-level OAuth token>")
    .RegisterEventHandler<MyEventHandler>()
    .RegisterBlockActionHandler<MyBlockActionHandler>());
var container = containerBuilder.Build();
var client = container.Resolve<ISlackSocketModeClient>();
await client.Connect();
```

#### Microsoft.Extensions.DependencyInjection
Using the [SlackNet.Extensions.DependencyInjection](https://www.nuget.org/packages/SlackNet.Extensions.DependencyInjection) package:

```c#
var serviceCollection = new ServiceCollection();
serviceCollection.AddSlackNet(c => c
    .UseAppLevelToken("<app-level OAuth token>")
    .RegisterEventHandler<MyEventHandler>()
    .RegisterBlockActionHandler<MyBlockActionHandler>());
var serviceProvider = serviceCollection.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<ISlackSocketModeClient>();
await client.Connect();
```

#### SimpleInjector
Using the [SlackNet.SimpleInjector](https://www.nuget.org/packages/SlackNet.SimpleInjector) package:

```c#
var container = new Container {
    // AsyncScopedLifestyle is used for scoping handlers
    Options = { DefaultScopedLifestyle = new AsyncScopedLifestyle() }
};
container.AddSlackNet(c => c
    .UseAppLevelToken("<app-level OAuth token>")
    .RegisterEventHandler<MyEventHandler>()
    .RegisterBlockActionHandler<MyBlockActionHandler>());
var client = container.GetInstance<ISlackSocketModeClient>();
await client.Connect();
```

### Breaking Changes

### SlackNet
- Added a dependency on the `Microsoft.Bcl.AsyncInterfaces` package.
- `Default.Http` takes in a `Func<HttpClient>` instead of just a `HttpClient`, to allow a client to be provided per request.
- `Default.RegisterServices` has been removed.

### SlackNet.AspNetCore
- Added a dependency on the `Microsoft.AspNetCore.Http` and `SlackNet.Extensions.DependencyInjection` packages.
- Updated `Microsoft.Extensions.DependencyInjection.Abstractions` dependency to 2.2.0.
- `SlackServiceConfiguration` has been replaced with `ServiceCollectionSlackServiceConfiguration` as the configuration object passed to the `AddSlackNet` configuration callback.
- `IEventsObservables` has been removed. It's simple enough to re-implement if anyone needs it, but it doesn't fit the rest of the API.


## v0.8.2

- Fixed empty `ViewState.Values` deserialization, which occurs when a modal dialog is closed without entering any values.
- Added `Description` property to block kit `Option`s for use with radio buttons.


## v0.8.1

- Added `Files` and `Blocks` to `SlackBot`'s messages.

Thanks to @eFloh for this release ✨


## v0.8.0

A few changes relating to Slack deprecating some of their old API features...

- The API token is always sent up as a header - fixes authorization for Slack apps created after 24th February.

### Breaking Changes
- Channels, Groups, IM, and MPIM APIs have been removed. SlackNet.Bot APIs using these types will continue to work for now, but will be removed fairly soon.
- `EventCallback.AuthedUsers` has been removed.


## v0.7.12

- Added support for workflows.
- Added `Ephemeral` property to `BotMessage`, to allow sending ephemeral replies.
- Marked channel, group, IM, and MPIM APIs as `Obsolete`. These APIs will stop functioning in February 2021.

Thanks to @fstojanac for doing most of the heavy lifting in this release 💪


## v0.7.11

- Added `EventContext` and `Authorizations` to `EventCallback`. `AuthedUsers` is now obsolete.
- Added `AppsEventsAuthorizations` API to `SlackApiClient`.
- Fleshed out the `AppMention` event.
- Fixed deserialization of user mentions in rich text blocks.

Thanks to @fstojanac and @Julian-Robinson for this release 🎉


## v0.7.10

- Added support for `PlainTextInput`s dispatching actions when someone interacts with them.
- Added `TimePicker` block element (note: still a beta feature at time of writing).

Thanks to @fstojanac for his help with this release 🙇‍♂️


## v0.7.9

- Fixed `SlackBot` fetching conversations other than public channels.

Thank you to @Silvenga for this fix 🐱‍🏍


## v0.7.8

- Fixed infinite loop when fetching conversations in `SlackBot`.
- Added `ApiAppId` to `SlashCommand`.


## v0.7.7

- Fixing `InvalidOperationException` caused by re-using request when rate limits hit.


## v0.7.6

- Adding Header block type.
- Requests will automatically retry after a delay when rate limits are hit and Slack responds with a 429 HTTP response.

Thanks to @fstojanac for his help with this release 🥳


## v0.7.5

- Added endpoint configuration method for disabling validation for event URL verification requests.

Thanks again to @fstojanac for this release.


## v0.7.4

- Fixed a thread-safety issue in SlackTypeConverter that could cause deserialization to fail.

Thanks to @fstojanac for finding and fixing this 🐛🔨


## v0.7.3

- `SlackBot` uses conversations API internally, as channels/groups/mpim/im APIs have been deprecated by Slack.
- Added conversation equivalents of channels/groups/mpim/im methods to `SlackBot`.
- `Default` methods' parameters are now optional.
- Added new `Mark` method to `ConversationsApi`.
- Added cursor-based paging to `FilesApi.Info`, `ReactionsApi.List`, and `StarsApi.List`.

### Potentially-Breaking Changes
- Hub-based methods, properties, and types have been marked as `Obsolete`. `Conversation`s have the same properties as `Channel`s and `Im`s, so migrating should be straightforward. The main difference is that rather than having different APIs and methods for channels/groups etc., everything is just a "conversation". If you need to differentiate between the different types of conversations, use the `IsChannel`, `IsGroup` etc. properties.
  - `SlackBot` uses the same API & cache for `Conversation`s, `Channel`s, and `Im`s, and `ConversationIdentifier`s are interchangeable with `HubIdentifiers`, so migrating piecemeal should work OK.
- The `cursor` parameter added to `FilesApi.Info`, `ReactionsApi.List`, and `StarsApi.List` was added before the `CancellationToken`, so on the off chance you were using every single parameter, you'll need to specify the parameter name for the `CancellationToken`.


## v0.7.2

- Exceptions thrown by handlers bubble up properly.
- Responder task correctly completed at end of request.


## v0.7.1

- `User` and `IsUserDeleted` added to `Conversation`.
- `TeamId` added to `User`.
- `Title`, `RealNameNormalized`, `DisplayName`, `Team`, and `ImageOriginal` added to `UserProfile`.
- `UserProfile.AlwaysActive` is now nullable.
- `SelectedDate` properties on `DatePickerAction` and `DatePickerValue` are now nullable.
- `DateTime` properties like `DatePicker.InitialDate` are properly serialized in the `yyyy-MM-dd` format that Slack expects.
- `SlackUrlBuilder` correctly serializes enumerables of enum values, such as the `IEnumerable<ConversationType>` parameters in `IConversationsApi`.

Thank you to @fstojanac and @ivpusic for their help with this release ✨


## v0.7.0

- Added global shortcut support.
- Fixed an issue deserializing large requests.
- More consistent handler registration.
- Added ability to replace the root handlers.
- Simplified Azure Functions integration a little.
- Experimental early-response API.


### Breaking Changes
- `MessageAction`, `IMessageActionHandler`, and `RegisterMessageActionHandler` have been renamed to `MessageShortcut`, `IMessageShortcutHandler`, and `RegisterMessageShortcutHandler` respectively, to match Slack's new terminology. The old types & methods are still available, but have been marked as `Obsolete`.
- All the base handler registration classes (e.g. `SlackEvents` and `SlackBlockActions`) have been removed. If you were using any of these, I'd recommend just copying their old code - they were pretty simple.
  - If you were relying on the `IObservable`s exposed by `SlackEvents`, and you're using SlackNet.AspNetCore, you can inject `IEventsObservables`, which exposes the same observables.
- `RegisterDialogSubmissionHandler` now takes in a `callbackId`, so you can register different handlers for different dialogs. If you want the old behavior where one handler is used for all dialogs, use `ReplaceLegacyDialogSubmissionHandling` instead. Note that this is for Slack's old dialogs - I'd recommend moving to modal views going forward.
- `SlackResponse` has been renamed to `SlackResult`, and now implements `IActionResult`.
  - SlackNet.AspNetCore now depends on Microsoft.AspNetCore.Mvc.Abstractions, which you're probably already depending on.
  - If you're using SlackNet.AspNetCore with Azure Functions, I'd recommend copying the updated [SlackEndpoints example](https://github.com/soxtoby/SlackNet/blob/master/SlackNet.AzureFunctionExample/SlackEndpoints.cs) - it's much simpler now.


### Experimental early-response API
A common issue with the current handler API is that SlackNet waits for all of the handlers' work to complete before responding to Slack, which doesn't wait very long before showing errors in the UI. This release includes an experimental API for responding to Slack _before_ completing all the work a handler performs. This lets you implement a handler like this:

```c#
public Task Handle(SlashCommand command, Responder<SlashCommandResponse> respond)
{
    // Provides feedback as quickly as possible
    await respond(new SlashCommandResponse { Message = new Message { 
        Text = "Command received" } });

    // Posts more information once it's available
    var info = await FetchSomeInfoFromARemoteService();
    await _slackApi.Chat.PostMessage(new Message { 
        Text = $"Here's what you asked for: {info}" });
}
```

These new handler types can be registered with the `RegisterAsync*` methods available inside the configuration callback of `AddSlackNet()`. Work performed after responding should probably still be kept fairly short, in the order of seconds. If you want to perform longer operations, it's probably best to do it in a background thread.

Please give this a go and provide feedback in the [early-response feedback issue](https://github.com/soxtoby/SlackNet/issues/44).


## v0.6.6

- `ExternalMultiSelectAction` correctly inherits from `BlockAction`.
- `ExternalMultiSelectValue` correctly inherits from `ElementValue`.
- Added `DefaultToCurrentConversation` to conversation select menus.
- Added `WithToken()` to `ISlackApiClient` for using a different access token.

Thank you to @mblack-montag for his help with this release 🥇.


## v0.6.5

- Null arguments are ignored for POST requests, same as GET requests. Fixes `as_user` deprecation error when sending messages with "new" tokens.
- Added `Respond` extension method for responding to `SlashCommand`s, similar to the one for responding to `InteractionRequest`s.
- Fixed deserialization error that occurred after responding with `SlashCommand`'s `ResponseUrl`.


## v0.6.4

- Adding some missing fields in `User` type.
- Fixing `UserProfile` image property deserialization.
- Making `AsUser` properties nullable, since Slack reports them as deprecated in some scenarios.
- Fixing `FilesApi.UploadSnippet`.
- Fixing `File.Shares` deserialization.
- Adding `View` to `BlockActionRequest`.
- Adding `Respond` method to `SlackApiClient` for using `ResponseUrl`s.
- `MessageResponseWrapper` allows message to be null, for responses that delete the source message.
- Adding new `Style` property to `ConfirmationDialog`.
- Adding new `Filter` property to `ConversationSelectMenu` & `ConversationMultiSelectMenu`.

Thank you to @bschuedzig and @joshuajung for their contributions to this release 👍


## v0.6.3

- Added `DateTime.FormatForMessage()` extension for localizing `DateTime`s in messages.
- Added `Files` and `Upload` property to `MessageEvent`.
- Fixing URL verification response.
- Added specific types for `ViewInfo` element values.
- Added some missing `BlockAction` types.


## v0.6.2

- Support for slash commands
- Added checkbox and radio group elements


## v0.6.1

Forcing TLS 1.2 for websocket connections


## v0.6.0

- Support for Views (both modals and app home).
- Easier integration with Azure Functions.
- Upgraded to `System.Reactive` v4.

Breaking Changes
--------------------
- `SlackNet` and `SlackNet.Bot` now require .NET Standard 2.0.
- `SlackEventsMiddleware` is no longer available.
- `EventCallback` and `UrlVerification` now inherit from `EventRequest` instead of `Event`.
- Removed deprecated `MessageEvent.Replies` property.

Thank you to @casperOne for contributing to this release 🎉.


## v0.5.0

- Added missing `PostMessage` and `GetPermalink` methods to `IChatApi`.
- Requests can now be validated with a signing secret in `SlackNet.AspNetCore`.
- Fixed `OAuthApi.Access`'s `redirect_uri` request argument.
- Sending a message or replying to a message via `SlackBot` can now be cancelled (if the message hasn't already been sent).
- Errors that occur while sending messages via `SlackBot` are now exposed to user code instead of being swallowed.
- Messages from bots received by `SlackBot` have their `User` property populated correctly.
- Added `SlackBot.GetBotUserById`.
- Fixed enum serialization error messages.
- Added `RemoteFilesApi`.
- Added `ChannelShared` and `ChannelUnshared` events.
- Added `RichTextBlock`.
- Added multi-select block elements.

Breaking Changes
----------------
- Removed deprecated pins and reactions methods dealing with files and file comments.

Thank you 🙏 to @eFloh, @mrmurb, @taskbit, and @Silvenga for their help with this release.


## v0.4.0

- Switching to POST requests for API methods that Slack prefers POST for, allowing for larger requests with more content.
- `ISlackApiClient` exposes two new `Post` methods that POST their `args` in the body of the request.

Breaking Changes
--------------------
- The `IHttp` interface now provides a single `Execute` method that takes in a `HttpRequestMessage`, rather than separate `Get` and `Post` methods.


## v0.3.1

Fixing deserialization of `UserProfile`s where the user has modified their profile, but hasn't filled in any _custom_ fields.


## v0.3.0

- Full support for message blocks.
- `SlackNet.AspNetCore` allows registering handlers for block actions and providing options to `external_select` block elements.
- Added some registration method overloads to `SlackNet.AspNetCore` to allow more flexibility.
- Fixed `DateTime` properties, which were always `null`.
- `RegisterDialogSubmissionHandler` correctly registers the handler without requiring an extra manual registration.

Breaking Changes
----------------
- `Attachment.Ts` is now nullable to allow sending attachments with blocks.
- `IMessageActionHandler.Handle` no longer returns a `MessageActionResponse`, which didn't work anyway. `MessageActionResponse` has been removed.
- `InteractionRequest.CallbackId` property pushed down to old sub-types, since the new `BlockActionRequest` doesn't have it.
- `DialogElement.Type` is now a `string`, to allow extension if necessary. The `DialogElementType` enum has been removed.


## v0.2.0

- Updated existing APIs.
- Added support for message actions.
- Added support for handling dialog cancellation.
- Added methods to `SlackRtmClient` to subscribe to and query for user presence events.
- Fixed a bunch of deserialization failures caused by missing getters (thanks to @eFloh for picking this up).
- Fixed typing of `ConversationsApi.Open`.
- `SlackBot` correctly sets `Hub` on incoming IMs.

Breaking Changes:
---
- `ConversationsApi.Open` takes in a `channelId` _or_ `userIds`, and only returns a channel ID.
- `ConversationsApi.Open` no longer takes in `returnIm`. Use `ConversationsApi.OpenAndReturnInfo` instead.
- Removed `UsersApi.List`'s `presence` parameter, as it is no longer supported.
- Removed `UsersApi.SetActive` method, as it is no longer supported.
- Most file events now only contain a file ID instead of a whole file, since that's what Slack sends.
- `FileShare` messages contain a `Files` list instead of a single `File`.
- Removed `FileCommentsApi`'s `Add` and `Edit` methods, as they are no longer supported.
- `IDialogSubmissionHandler` must now implement `HandleCancel`. Just return `Task.CompletedTask` if you don't care about cancelled dialogs.
- `DndApi.TeamInfo`'s `userIds` parameter is now required.
- With the addition of `MessageAction` support, there are a few "Action"-related renames:
  - Renamed `ISlackActions` to `ISlackInteractiveMessages`.
  - Renamed `IActionHandler` to `IInteractiveMessageHandler`.
  - Renamed `SlackServiceConfiguration.RegisterActionHandler` to `RegisterInteractiveMessageHandler`.


## v0.1.10

- Added AspNetCore project with endpoints for events and interactive messages
- Added Conversations API
- Added Dialog API
- Added some other missing API methods
- Fixed a bunch of issues caused by typos


## v0.1.9

- RTM client has better reconnection logic and doesn't drop events after an error
- Updated dependencies - no longer prerelease!

