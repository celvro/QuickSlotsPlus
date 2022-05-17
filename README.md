
# QuickSlots+

A C# mod for Subnautica to add more slots.

## Features

* Up to 20 slots with hotkeys
* Disable adding new items to the quick slots
* Custom hotkey labels
* Icons snap to quick slots during drag/drop
* Fix a bug with saving empty slot positions
* Works with Nitrox (need to edit `mod.json` in SMLHelper)

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


## Requirements

* [QModManager 4](https://github.com/SubnauticaModding/QModManager/releases) - extract to root Subnautica directory
* [SMLHelper](https://github.com/SubnauticaModding/SMLHelper/releases) - extract to QMods folder

*Note:* The github releases of QModManager and SMLHelper are behind Thunderstore.

## Manual Installation

1. Install QModManager and SMLHelper.
2. Extract `QMods\QuickSlotsPlus\` to `Subnautica\QMods\QuickSlotsPlus`.
3. (Nitrox only) Edit SMLHelper's `mod.json` file and add `"nitroxcompat": true`.
   * The file path is `QMods\Modding Helper\mod.json`

## Known issues

* Controller hotkeys don't work for slots 6-20
* Sometimes while testing Nitrox my equipped tool icon disappeared

[Noticed a bug?](https://github.com/celvro/QuickSlotsPlus/issues)
