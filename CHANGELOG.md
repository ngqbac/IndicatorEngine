# Changelog

## [1.0.0] - Initial Release
### Added
- Hierarchical `IndicatorId` with helper utilities (child id + ancestor hierarchy).
- `IndicatorTree` runtime core:
    - boolean state and counter state
    - parent aggregation from children
    - events for active changes and node removal
- Blueprint system:
    - `IIndicatorBlueprint` + `AbsIndicatorBlueprint`
    - `BlueprintRegistry` + `BlueprintHooks` for registration and resolution
- Visual binding layer:
    - `IndicatorVisual` for binding `IIndicatorHost` ↔ `IndicatorId`
    - forwards tree events to bound hosts
    - lazy hydration with recursion guard and pending retry support
- Logging base abstraction (`AbsIndicatorLogger`) for project-defined logging output.

## [1.0.1] - Maintenance
### Added
- Added manual refresh APIs to force-sync indicator state:
  - `RefreshBlueprint(AbsIndicatorBlueprint blueprint)`
### Changed
- Renamed manual refresh APIs for clarity:
  - `RefreshFromRoot(IndicatorId)` → `RefreshBranch(IndicatorId)`