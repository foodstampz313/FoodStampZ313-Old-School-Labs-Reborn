# FoodStampZ313's Old School Labs Reborn

SPT 4.x rebuild of the Old School Labs Reborn concept.

## Features

- Gives colored Labs keycards effectively unlimited uses by setting `MaximumNumberOfUsage` to a configurable high value.
- Applies configurable old-school prices to the colored Labs keycards.
- Keeps the configuration in `config/config.json`.

## Install for users

Install the compiled release folder into:

```text
SPT/user/mods/FoodStampZ313s_Old_School_Labs_Reborn/
```

The compiled release must contain the DLL and config file. This source package must be built before it is a playable release.

## Build

1. Install .NET 9 SDK.
2. Open this project in Visual Studio or Rider.
3. Restore NuGet packages.
4. Build in Release mode.
5. Copy the folder from:

```text
bin/Release/FoodStampZ313s.OldSchoolLabsReborn/FoodStampZ313s.OldSchoolLabsReborn/
```

into:

```text
SPT/user/mods/
```

## Credits

- FoodStampZ313 — concept, packaging, release/testing.
- OpenAI ChatGPT — technical framework assistance.
- MrFums / Old Colored Keycards — original concept inspiration only; no original files were used.
