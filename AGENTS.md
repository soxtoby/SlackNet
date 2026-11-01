XML docs should match the Slack docs, but without documenting limits, and referencing existing types and members where possible.

# Tests

Test the behaviour a change introduces. Code that follows an established pattern inherits that pattern's tests. e.g. a property mapped by the snake_case naming convention, or an event or block identified by its class name, needs no test of its own.

# Changelog

A change a user would notice gets an entry under `## Unreleased` in `CHANGELOG.md`, under one of these headings, which `./do prepare-release` reads to choose the next version:

- `### Breaking`: a change that breaks existing code or behaviour, such as a removed or renamed member. Minor before 1.0, major after.
- `### Added`: a new API method, event, block, property, or configuration option. Patch before 1.0, minor after.
- `### Changed`: a deliberate change to existing behaviour. Patch before 1.0, minor after.
- `### Fixed`: a bug fix. Patch.

Keep entries short and user-focused, naming the types and members involved. A release with a theme can open with a short paragraph before the headings.
