
# QuickSlots+

A C# mod for Subnautica to add more slots.

## Features

* Up to 20 slots with hotkeys
* Disable adding new items to the quick slots
* Custom hotkey labels
* Works with Nitrox

> [!Warning]
> Switched from SMLHelper to Nautilus, make sure to update.

## Requirements

* [BepInEx](https://www.nexusmods.com/subnautica/mods/1108)
* [Nautilus](https://www.nexusmods.com/subnautica/mods/1262) (formerly SMLHelper) — **1.0.0-pre.46 or newer**. Earlier builds have no `CreateInput` API, so the extra slots cannot register and the mod fails to load with a `TypeInitializationException`.

> [!Note]
> Vortex resolves Nautilus from Nexus normally. Thunderstore mod managers (r2modman, Gale,
> Thunderstore Mod Manager) cannot, because Nautilus is not published on Thunderstore — if you
> installed QuickSlots+ from Thunderstore, install Nautilus from the Nexus link above by hand.

## Installation

Automatic:
Use the Vortex mod manager to install from [NexusMods](https://www.nexusmods.com/subnautica/mods/984?tab=files).

Manual:
1. Install BepInEx and the Nautilus plugin.
2. Download zipfile from the [releases page](https://github.com/celvro/QuickSlotsPlus/releases) and extract.
3. Copy the `BepInEx\` folder to your game directory. The path should end up like `Subnautica\BepInEx\plugins\QuickSlotsPlus`.

## Build from source

1. Install Visual Studio or use msbuild. Game assemblies are pulled automatically via NuGet (`Subnautica.GameLibs` / `Subnautica.BelowZero.GameLibs`).
1. Change the $(GameDir) property in the .csproj file if needed, it defaults to Steam directory on C: drive.
1. In VS select Build -> Build Solution (or `msbuild QuickSlotsPlus.csproj /p:Configuration=Subnautica` / `BelowZero`), it will copy the files into your BepInEx folder.
1. Launch Subnautica.

## Controller Support

Now supports Controller icons on hotbar!

## Nitrox

Tested with Nitrox 1.8.1.0 and Nautilus 1.0.0-pre.56. No extra setup is needed, and the old
`"NitroxCompat"` setting from SMLHelper no longer exists.

Nitrox 1.8.1.0 hangs on the epilepsy warning screen with Nautilus 1.0.0-pre.45 or newer, even without
other mods. QuickSlots+ includes a workaround for this, so if you remove it you will also need to roll
Nautilus back to 1.0.0-pre.44 to keep playing Nitrox. Nitrox's chat and Discord keys keep their defaults
but can't be changed in the options menu.

> [!Warning]
> Nitrox saves your quick slot bindings per world at whatever length your client used, and
> restores them without checking the slot count in effect. Lowering **Slot count**, or joining a
> world with the mod removed, after playing with more slots can raise an `IndexOutOfRangeException`
> while the world loads. Keep the slot count consistent for a given Nitrox save.

### Reserve Item Slots

You can add a file named `BepInEx\plugins\QuickSlotsPlus\allowItems.json` to allow/reserve items to auto equip to specific slots.
The first value is the TechType, and the second is the slot number to reserve. Use -1 to equip to the first empty non-reserved slot.

For example to reserve slot 5 for Knife and choose the first empty slot for Flares:
```json
{
    "Knife": 5,
    "Flare": -1
}
```
To find the TechType, look in the Nautilus debug log after picking up an item.

### Add custom labels

1. Set hotkey in game, look at name.
2. Create a file named `BepInEx\plugins\QuickSlotsPlus\customLabels.json`. Limited support for unicode.
```json
{
    "LeftArrow": "⬅️",
    "RightArrow": "Right"
}
```
3. I've already added some [default custom labels](https://github.com/celvro/QuickSlotsPlus/blob/fe41a7685674630b3e1b4fba457562b3d6f3bd66/Utility/LabelUtil.cs#L112). 
You can override them with the KeyCode name.

## Known issues

* Can't use CTRL or ALT modifiers for hotkeys
* Sometimes while testing Nitrox my equipped tool icon disappeared

## Changes

### 3.1 (Nitrox fix)

* Fixed the game hanging on the epilepsy warning screen when using Nitrox with Nautilus 1.0.0-pre.45 or newer
* Tested quick slots 7-10 and saved slot positions with Nitrox 1.8.1.0 and Nautilus 1.0.0-pre.56
* Requires Nautilus 1.0.0-pre.46 or newer
* Harmony patches are now removed when the plugin is unloaded

### 2.0 (Nautilus update)

* Updated from SMLHelper to Nautilus
* Fixed display of controller icons for slot labels

[Noticed a bug?](https://github.com/celvro/QuickSlotsPlus/issues)
