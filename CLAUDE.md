# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

QuickSlotsPlus is a BepInEx/Harmony C# mod for the games Subnautica and Subnautica: Below Zero. It expands the vanilla 5-slot quickbar to up to 20 slots, adds hotkey labels, controller icon support, and configurable item auto-binding. It depends on the Nautilus modding framework (formerly SMLHelper).

## Build

This is a legacy-style (non-SDK) MSBuild `.csproj`, built via Visual Studio or `msbuild`, not `dotnet build`. There are two build configurations selected via the `Configuration` property, each targeting a different game and defining a different preprocessor constant:

- `Subnautica` configuration → defines `SUBNAUTICA`, outputs to `build\Subnautica\`
- `BelowZero` configuration → defines `BELOWZERO`, outputs to `build\BelowZero\`

```
msbuild QuickSlotsPlus.csproj /p:Configuration=Subnautica
msbuild QuickSlotsPlus.csproj /p:Configuration=BelowZero
```

Each configuration pulls a different `GameLibs` package, but NuGet writes one set of restore assets
per project. The csproj therefore points `MSBuildProjectExtensionsPath` at `obj\<Configuration>\`,
set before the `Microsoft.Common.props` import because that is where the property is read. Without
it, building one game straight after restoring the other compiles against the wrong game's
`Assembly-CSharp` and fails in [LabelUtil.cs](Utility/LabelUtil.cs) with `CS0117` on
`GameInput.GetBindingName` / `GetInputName` / `GetKeyCodeAsInputName` / `uGUI.GetDisplayTextForBinding`.
Those errors mean a stale restore, not a missing API.

After a successful build, the `PostBuildEvent` in [QuickSlotsPlus.csproj](QuickSlotsPlus.csproj) automatically xcopies the built DLL into `release\<Configuration>\BepInEx\plugins\QuickSlotsPlus\` and into `$(GameDir)\BepInEx\plugins\QuickSlotsPlus\` (or `QuickSlotsPlus_BZ` for BelowZero). `$(GameDir)` defaults to the Steam install path and must be edited in the csproj if the game lives elsewhere. The same event also copies the `Localization\` folder. The install copy is skipped when the game is running (the game memory-maps the DLL, and `xcopy` deletes the installed file before failing to replace it), so close the game and rebuild to install.

Before building, publicized game assemblies must be available — see the "Build from source" section in [README.md](README.md) (uses https://github.com/elliotttate/Bepinex-Tools). Game/BepInEx/Nautilus assemblies are pulled via NuGet (see `RestoreAdditionalProjectSources` in the csproj, pointing at nuget.org and nuget.bepinex.dev) rather than committed to the repo.

**Locating restored NuGet packages/DLLs (e.g. `Nautilus.dll`, game libs):** they land in the default NuGet global-packages folder, `C:\Users\lustick\.nuget\packages\` (`/c/Users/lustick/.nuget/packages/` in Git Bash) — do NOT search the whole drive (`find / -iname ...`) for them. Each package is a subfolder named after its (lowercased) package ID, e.g. `subnautica.nautilus\<version>\lib\net472\Nautilus.dll` or `subnauticazero.nautilus\<version>\lib\net472\Nautilus.dll`. Use `find "/c/Users/lustick/.nuget/packages/<package-name>" -iname "*.dll"` scoped to the specific package folder instead.

There are no automated tests; verification is manual, in-game.

## Architecture

**Dual-game support via preprocessor constants.** Nearly every file has `#if SUBNAUTICA` / `#else` (BelowZero) branches, because the two games' `GameInput`/keybinding APIs differ:
- Subnautica registers extra quickbar slots as new bindable `GameInput.Button` enum entries at runtime via Nautilus's `EnumHandler`/`GameInputHandler` (see the `Slot6`...`Slot20` fields in [QuickSlotsPlus.cs](QuickSlotsPlus.cs)).
- Below Zero instead exposes each extra slot as its own `[Keybind]` config option (`HotKey6`...`HotKey20` in [StandardConfig.cs](StandardConfig.cs)) read via reflection in [LabelUtil.cs](Utility/LabelUtil.cs).

**Harmony patches ([Patches/](Patches/)) are the core mechanism** — the mod has no game loop of its own beyond a mostly-dead `Update()` in Subnautica; all behavior is Harmony prefixes/postfixes/transpilers applied at `Awake()`:
- [Inventory_Awake_Patch.cs](Patches/Inventory_Awake_Patch.cs) — postfix on `Inventory.Awake`; replaces `Inventory.main.quickSlots` with a `QuickSlots` instance sized to the configured `slotCount` instead of the vanilla hardcoded 5.
- [QuickSlots_SlotNames_Patch.cs](Patches/QuickSlots_SlotNames_Patch.cs) — transpilers on `QuickSlots.Update`/`SelectInternal`/`DeselectInternal` that rewrite `ldsfld` opcodes to point at `LabelUtil.slotNames` (a 20-length array) instead of the vanilla 5-length static array, working around a bug where the vanilla field intermittently resets and causes index-out-of-range exceptions.
- [QuickSlots_BindToEmpty_Patch.cs](Patches/QuickSlots_BindToEmpty_Patch.cs) — prefix (fully replaces, returns `false`) on `QuickSlots.BindToEmpty`; implements the `disableBindToEmpty` option and the `allowItems.json`-driven reserved-slot/auto-equip logic (`ItemHelper` class in the same file).
- [uGUI_QuickSlots_Init_Patch.cs](Patches/uGUI_QuickSlots_Init_Patch.cs) — postfix on `uGUI_QuickSlots.Init`; triggers `LabelUtil.DrawLabels` to (re)draw hotkey label text over the slot icons.
- [GameInput_Patch.cs](Patches/GameInput_Patch.cs) — postfix on `GameInput.TryBind`; redraws quickslots whenever the user rebinds a key, so labels stay in sync.
- [GameInputSystem_Initialize_Patch.cs](Patches/GameInputSystem_Initialize_Patch.cs) — Subnautica only; transpiler on `GameInputSystem.Initialize` that adds Nitrox's buttons (values 1000+) to the list it builds actions from, because Nitrox otherwise hangs on the epilepsy warning screen with Nautilus pre.45+.

**Redraw flow.** Any time slot count, labels, or bindings change (options menu `OnChange` callbacks in [StandardConfig.cs](StandardConfig.cs), or a keybind change), `QuickSlotsPlus.RedrawQuickSlots()` in [QuickSlotsPlus.cs](QuickSlotsPlus.cs) runs: it saves the current `binding` array, deselects the active slot, re-invokes `Inventory_Awake_Patch.Postfix` to rebuild the `QuickSlots` object and labels, then restores as much of the old binding array as fits the new slot count.

**Config** ([StandardConfig.cs](StandardConfig.cs)) is a Nautilus `ConfigFile` rendered as an in-game options menu (`[Menu("QuickSlots+")]`), registered once as `QuickSlotsPlus.Options`. Most options carry `[OnChange(nameof(RedrawQuickSlots))]` so the UI updates live without a restart.

**User-facing JSON files** (read from the mod's own plugin folder at runtime, not shipped/committed — see [README.md](README.md) for schema):
- `allowItems.json` — TechType → reserved slot number, read by `ItemHelper` in [QuickSlots_BindToEmpty_Patch.cs](Patches/QuickSlots_BindToEmpty_Patch.cs). Supports `//` and `/* */` comment stripping before JSON parsing.
- `customLabels.json` — KeyCode/binding name → custom label string override, read by [LabelUtil.cs](Utility/LabelUtil.cs) (`LoadCustomLabels`, reloaded every `DrawLabels` call). `LabelUtil.DefaultLabels` provides built-in symbol labels (e.g. `Alpha1` → `'1'`) that custom labels can override.
- Both loaders catch `JsonReaderException` and surface the error in-game (`ErrorMessage`/`BasicText`) rather than crashing.

**Localization**: one JSON file per game language (39, named exactly like the game's `LanguageFiles`), kept in `release\<Configuration>\BepInEx\plugins\QuickSlotsPlus*\Localization\` and loaded by `QuickSlotsPlus.RegisterLocalization()` in `Awake()`. It resolves the folder from its own assembly and calls `LanguageHandler.RegisterLocalization` per file, because Nautilus's `RegisterLocalizationFolder()` resolved the game's `Managed` folder instead and silently registered nothing. In [StandardConfig.cs](StandardConfig.cs), option labels use `Label = "<id>"`, **not** `LabelLanguageId`: the game's `TranslationLiveUpdate` re-translates each row's stored key on a language change, and `LabelLanguageId` stores already-translated text, which leaves the Mods tab stuck in the old language. Tooltips use `TooltipLanguageId` (no live update; they refresh when the panel is reopened). The `OptionSlot6`-`OptionSlot20` keys are what the game's own key bindings menu shows for the extra slots, so keep their wording consistent with each language's vanilla `OptionSlot1`-`OptionSlot5`. Every file must carry the same key set.

**Versioning**: the plugin version lives in `versionString` in [QuickSlotsPlus.cs](QuickSlotsPlus.cs) and must be kept in sync with `release/manifest.json`'s `version_number` when cutting a release.
