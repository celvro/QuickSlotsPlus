
# QuickSlots+

A C# mod for Subnautica to add more slots.

## Features

* Up to 20 slots with hotkeys
* Disable adding new items to the quick slots
* Custom hotkey labels
* Icons snap to quick slots during drag/drop
* Save quick slot positions when you exit the game
* Works with Nitrox

## Requirements

* [QModManager 4](https://www.nexusmods.com/subnautica/mods/201)
* [SMLHelper](https://www.nexusmods.com/subnautica/mods/113)

## Installation

1. Install QModManager and SMLHelper.
1. Extract `QMods\QuickSlotsPlus\` to `Subnautica\QMods\QuickSlotsPlus`.

## Build from source

1. Install Visual Studio or use msbuild.
1. Create the publicized assemblies using https://github.com/elliotttate/Bepinex-Tools/releases
1. Change the $(GameDir) property in the .csproj file if needed, it defaults to Steam directory on C: drive.
1. In VS select Build -> Build Solution, it will copy the files into your QMods folder.
1. Launch Subnautica.

## Nitrox

Manually edit SMLHelper's `QMods\Modding Helper\mod.json` file and add `"NitroxCompat": true`.

### Add custom labels

1. Set hotkey in game, look at name.
2. Create a file named `QMods\QuickSlotsPlus\customLabels.json`. Limited support for unicode.
```json
{
    "LeftArrow": "⬅️",
    "RightArrow": "Right"
}
```
3. I've already added some [default custom labels](https://github.com/celvro/QuickSlotsPlus/blob/fe41a7685674630b3e1b4fba457562b3d6f3bd66/Utility/LabelUtil.cs#L112). 
You can override them with the KeyCode name.

## Known issues

* Controller hotkeys don't work for slots 6-20
* Can't use CTRL or ALT modifiers for hotkeys
* Sometimes while testing Nitrox my equipped tool icon disappeared

[Noticed a bug?](https://github.com/celvro/QuickSlotsPlus/issues)
