# Changelog

## Unreleased - 2026-08-09

### Added

- Native modifier framework with automatic registration, timed and permanent modifiers, lifecycle callbacks, player queries, and synchronized add, remove, and clear operations.
- Role-specific option groups with strongly typed singleton access.
- Host-side role assignment overrides for integrations that need deterministic vanilla-role allocation.
- Player leave event carrying the disconnected client data.
- Italian localization for the built-in FungleAPI interface and network messages.
- Public access to registered custom ability buttons.
- Murder event source information alongside the target and result flags.
- `Value` compatibility accessor for number options.

### Fixed

- Custom buttons now initialize safely when sprites or HUD templates are unavailable.
- Button visibility updates correctly after role changes and while minigames, meetings, or exile screens are open.
- HUD button grids are rearranged dynamically and invalid or abstract button classes are ignored.
- Button failures are isolated so one broken mod button no longer prevents the remaining HUD from loading or updating.
- Vanilla report, sabotage, and vent sprites are captured safely before custom HUD initialization.
- Role and task-panel updates now tolerate missing player, role, text, and HUD objects.
- Role visibility checks no longer throw when a role, team, or player reference is missing.
- Role option registration now includes all matching role option groups.
- Plugin discovery, ordering, configuration filenames, names, and versions now work when plugin metadata or GUID values are missing.
- Modifier lifecycle notifications now run on updates, meetings, deaths, disconnects, and game teardown.
- Role assignments can be corrected by the host without creating duplicate vanilla special roles.

### Changed

- Custom buttons are updated centrally from the HUD instead of relying on a component attached to every cloned button.
- Custom role options and grouped role options are initialized together.
- The project file embeds the Italian translation and compiles the new role-option, event, and modifier systems without machine-specific dependency paths.
