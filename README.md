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
If you find a bug or want to make a suggestion on a certain feature, make an issue on the [issue tracker](https://github.com/TheMysticSword/MysticsItems/issues) and make sure that a similar issue isn't open at the moment.  
Consider providing an output log in your bug report to make the fixing process easier.  
Make a post on the [Discussions](https://github.com/TheMysticSword/MysticsItems/discussions) tab if you want to leave an idea for a new item or make a balance suggestion.

### Credits
TheMysticSword - Coding  
Marwan60 - Modelling  
Tuning fork SFX by Martin Chappell, used in Ratio Equalizer (https://freesound.org/people/martian/sounds/19308/)  
Sounds from the Disc Room Game Jam audiopack by doseone, used in Timely Execution (https://discroom.com/game-jam-tutorial-audio.zip)  
Other sounds made with sfxia by rxi (https://rxi.itch.io/sfxia)  
  
### What's new?
#### 1.1.12:
* Fixed Treasure Map zone being visible with 1m radius when nobody has the item
#### 1.1.11:
* Nuclear Accelerator:
	* Changed function: sprint to charge, increase damage by 10% per 4 seconds spent sprinting for 4 seconds after sprinting
	* Added SFX and VFX
* Relentless Vendetta:
	* Buff Duration: ~~15s (+15s per stack)~~ ⇒ 8s (+8s per stack)
	* Shorter buff duration condition changed from "ally died on the same stage it spawned on" to "ally had decaying health"
* Fixed 'Last of Us' challenge unlock being granted to all players instead of being granted only to the survivor
* Tweaked Wirehack Wrench code to prevent it from randomly stopping detecting 3D Printers
* Fixed Crystallized World hidden outline sphere clipping through the actual model
* Fixed Rift Affliction and Deafened VFX missing fade-in and fade-out animations, causing them to stay indefinitely
* Fixed skill cooldowns not being re-calculated once the Deafened debuff from Vintage Microphone wears off
* Fixed Cup of Expresso VFX moving with the character's body parts, causing jittery movement
* Fixed Treasure Map zone playing the Focused Convergence SFX on stage entry when nobody has the item
* Fixed Vintage Microphone projectiles triggering on-hit items
* Fixed custom sound volume having wrong scaling from master and SFX volume sliders, resulting in sounds being much louder than intended
* Fixed custom flame particles not rendering at certain angles and distances
* Fixed Relentless Vendetta SFX playing only for the host
  
(Previous changelogs can be found [here](https://github.com/TheMysticSword/MysticsItems/blob/main/CHANGELOG.md))

![](https://i.imgur.com/gBBfdeO.png)
