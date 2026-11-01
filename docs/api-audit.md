# Slack API audit

Run from the repo root with the .NET 10 SDK and .NET 8 runtime installed:

```sh
dotnet run --project SlackNet.ApiAudit
```

The pilot compares `users.*`, `conversations.*` and `chat.*` with Slack's method index and Markdown references. It records the real request argument names, including multipart fields, by invoking SlackNet's wrappers against an in-memory client; no Slack methods execute during discovery. Documented response examples are inspected using SlackNet's JSON contracts and polymorphic deserialization. When overloads expose different response shapes, each example uses the response contract with the fewest discrepancies. Elided or invalid examples remain explicitly unverified without discarding parameter coverage.

Results go to ignored `artifacts/api-audit/report.md` and `report.json`. Public source snapshots include SHA-256 hashes and changes since the previous run in that output directory. The first run reports existing gaps. Changelog and official Node SDK snapshots are investigation signals; the pilot does not parse them into schemas. Compare their changes before proposing a fix.

To replay downloaded sources without network access:

```sh
dotnet run --project SlackNet.ApiAudit -- --offline true
```

## Live probes

Provide a dedicated dev-workspace bot or user token through the `SLACK_API_AUDIT_TOKEN` environment variable or `--token-file /private/path`. Never pass the token itself in arguments or commit it. The tool does not load application settings or log HTTP payloads.

```sh
dotnet run --project SlackNet.ApiAudit -- --live true --channel C_FIXTURE --user U_FIXTURE
```

The user is optional and defaults to the token's user. The channel is optional; omitting it leaves channel info and history explicitly unverified. An empty channel is not automatically replaced by another workspace channel. List probes use the authenticated workspace ID, or `--team T_FIXTURE` for organization tokens that need an explicit workspace.

Read-only probes: `auth.test`, `users.list`, `users.info`, `conversations.list`, `conversations.info`, `conversations.history`. Use `users:read`, `channels:read`, and `channels:history` for public-channel coverage; private channels require the corresponding scopes and membership. Token type, workspace features and permissions can restrict coverage. Lists sample one page of 20 users/channels; history samples up to 15 messages. No messages or fixtures are created or modified. No pagination or rate-limit retries occur in this pilot. Rate limits, scope errors, empty samples and missing fixtures require further investigation, not a clean bill of health.

Live payloads exist only in memory. Saved evidence contains field paths and JSON kinds, not scalar values, IDs, message text, tokens or dynamic dictionary keys. Unknown fields captured in extension data remain findings. Missing fields never cause property removals. Nulls and empty arrays provide no evidence about their possible values or element types. Undocumented parameter discovery and verification of parameter effects require additional targeted scenarios.

## Review and PRs

Findings have IDs derived from their kind, method, path and model. To acknowledge an intentional omission or rejected finding, add `{"Id":"finding-id","Reason":"specific reason or existing PR URL"}` to `docs/api-audit-suppressions.json`. Suppressed findings remain visible. Sources or parser failures make the command exit nonzero; findings alone do not. Coverage gaps remain in the report.

For each run, review source changes and findings. Group related findings into one candidate PR, reusing an existing PR containing the IDs. Include the finding IDs, source links/hashes, live observations and unverified scenarios. Observed fields do not establish requiredness, all possible values or availability in other workspaces. Request acceptance alone does not prove a new parameter works. Preserve customer-implemented interfaces; flag necessary breaks explicitly. Match Slack XML docs and add concise Unreleased entries. Test new behavior; conventional snake_case properties inherit existing tests.

The **API audit** GitHub Actions workflow is manually triggered and uploads the report and public sources. Live runs use the repository secret `SLACK_API_AUDIT_TOKEN`. It has read-only repository permissions and does not create PRs or run an agent. Start scheduled detection and automatic draft PR creation after reviewing the pilot's findings and coverage.
