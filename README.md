
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
* [Nautilus](https://www.nexusmods.com/subnautica/mods/1262) (formerly SMLHelper)

## Installation

Automatic:
Use the Vortex mod manager to install from [NexusMods](https://www.nexusmods.com/subnautica/mods/984?tab=files).

Manual:
1. Install BepInEx and the Nautilus plugin.
2. Download zipfile from the [releases page](https://github.com/celvro/QuickSlotsPlus/releases) and extract.
3. Copy the `BepInEx\` folder to your game directory. The path should end up like `Subnautica\BepInEx\plugins\QuickSlotsPlus`.

## Build from source

1. Install Visual Studio or use msbuild.
1. Create the publicized assemblies using https://github.com/elliotttate/Bepinex-Tools/releases
1. Change the $(GameDir) property in the .csproj file if needed, it defaults to Steam directory on C: drive.
1. In VS select Build -> Build Solution, it will copy the files into your BepInEx folder.
1. Launch Subnautica.

## Controller Support

Now supports Controller icons on hotbar!

## Nitrox

Manually edit Nautilus's `BepInEx\plugins\Modding Helper\mod.json` file and add `"NitroxCompat": true`.

Warning: Did not test this after the switch from SMLHelper to Nautilus.

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

### 2.0 (Nautilus update)

* Updated from SMLHelper to Nautilus
* Fixed display of controller icons for slot labels

[Noticed a bug?](https://github.com/celvro/QuickSlotsPlus/issues)
