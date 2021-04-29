# MysticsItems
Adds 20 new items, with plenty more to come!  

![](https://i.imgur.com/WYCK8vE.gif)  
![](https://i.imgur.com/XWpPavo.png)  

### Important notes
#### Manual installation
This mod requires [R2API](https://thunderstore.io/package/tristanmcpherson/R2API/)  
Put both `MysticsItemsPlugin.dll` and `MysticsItemsStrings.json` in the plugins folder.  
#### Multiplayer
Make sure that your config matches with the configs of other players in the lobby.

### Console commands
`mysticsitems_grantall` - grants all achievements  
`mysticsitems_unlocklogs` - unlocks all logbook entries  

### Bug reports and suggestions
If you find a bug or want to make a suggestion on a certain feature, make an issue on the [issue tracker](https://github.com/TheMysticSword/MysticsItems/issues) and make sure that a similar one doesn't already exist.  
Make a post on the [Discussions](https://github.com/TheMysticSword/MysticsItems/discussions) tab if you want to leave an idea for a new item or make a balance suggestion.

### Credits
TheMysticSword - Coding  
Marwan60 - Modelling  
Tuning fork SFX by Martin Chappell, used in Ratio Equalizer (https://freesound.org/people/martian/sounds/19308/)  
Sounds from the Disc Room Game Jam audiopack by doseone, used in Timely Execution (https://discroom.com/game-jam-tutorial-audio.zip)  
Other sounds made with sfxia by rxi (https://rxi.itch.io/sfxia)  
  
### What's new?
#### 1.1.9:
* Scratch Ticket:
    * No longer affects chance effects that have less than 1% chance of occuring
        * This change was made to prevent purposefully rare effects from being affected (for example, elite equipment drops)
* Rift Lens and Gate Chalice debuffs were converted to items (Lunar Teleportation Affliction and Rift Affliction)
* Added new VFX for Relentless Vendetta, Lunar Teleportation Affliction, Rift Affliction, Rift Chests and Faulty Spotter
* Updated movement speed reduction percentage in Rift Lens, Gate Chalice and Microphone logbook descriptions to properly reflect their slowing coefficient
* Updated Legendary Mask flames
* Faulty Spotter now plays the cast sound once instead of playing the sound for each individual Spotter
* Fixed unimplemented and config-disabled items showing up in the drop pool
* Fixed Contraband Gunpowder pickups stopping mid-air and causing errors
* Fixed Treasure Map decal appearing for clients when nobody has the item
* Fixed Thought Processor reducing cooldowns below 0, essentially giving skills a second charge
* Fixed Wirehack Wrench not dropping an item when ShareSuite's "3D Printer & Cauldron Compatibility" is enabled
* Fixed Faulty Spotter targetting allies and neutral entities
  
(Previous changelogs can be found [here](https://github.com/TheMysticSword/MysticsItems/blob/main/CHANGELOG.md))

![](https://i.imgur.com/gBBfdeO.png)
