
# QuickSlots+

A C# mod for Subnautica to add more slots.

## Features

* Up to 20 slots with hotkeys
* Disable adding new items to the quick slots
* Custom hotkey labels
* Icons snap to quick slots during drag/drop
* Fix a bug with saving empty slot positions

### Add custom labels

1. Set hotkey in game, look at name.
2. Create a file named "Subnautica\Qmods\QuickSlotsPlus\customLabels.json". Limited support for unicode.
```json
{
    "LeftArrow": "⬅️",
    "RightArrow": "Right"
}
```
3. I've already added some default custom labels. You can override them with the KeyCode name.


## Requirements

* [QModManager 4](https://www.nexusmods.com/subnautica/mods/201?tab=files) - extract to root Subnautica directory
* [SMLHelper](https://www.nexusmods.com/subnautica/mods/113?tab=files) - extract to QMods folder

## Installation

Download the zip file from [NexusMods](https://www.nexusmods.com/subnautica/mods/984)

Extract it to your QMods folder.


## Known issues

* Controller hotkeys don't work for 6-20, maybe an issue with SMLHelper
* Controller hotkeys only "kind of" work for 1-5, it unbinds tools randomly
