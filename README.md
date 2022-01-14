# Mystic's Items
Adds 20 new items!  

![](https://i.imgur.com/WYCK8vE.gif)  
![](https://i.imgur.com/XWpPavo.png)  

### Important notes
#### Manual installation
Put both `MysticsItemsPlugin.dll` and `MysticsItems.language` in the plugins folder.  
#### Multiplayer
Make sure that your config matches with the configs of other players in the lobby.  

### Bug reports and suggestions
If you encounter an issue with the mod (bug, typo, mistranslation, incompatibility with another mod, etc.), report it on the [issue tracker](https://github.com/TheMysticSword/MysticsItems/issues) and make sure that a similar issue isn't open at the moment.  
Consider providing an output log in your bug report to make the fixing process easier.  
Make only one issue per bug.  
**Do not** post balance suggestions and feedback on the issue tracker.

### Configuration
The mod creates two config files on the first launch:
    * MysticsItems_General.cfg - general mod settings
    * MysticsItems_Balance.cfg - item and equipment value tweaking

### Credits
TheMysticSword - Coding  
Marwan60 - Modelling  
Omar Faruk - Turkish translation  
Tuning fork SFX by Martin Chappell, used in Ratio Equalizer (https://freesound.org/people/martian/sounds/19308/)  
Fireworks Crackle SFX by 16FThumaF, used in Contraband Gunpowder (https://freesound.org/people/16FThumaF/sounds/505264/)  
Big Thud SFX by Reitanna Seishin, used in Siren Pole deploy sound (https://freesound.org/people/Reitanna/sounds/332668/)  
Nuclear Alarm SFX by plasterbrain, used in Siren Pole (https://freesound.org/people/plasterbrain/sounds/242856/)  
Whooshes collection "Whoosh, Metal, Alien" SFX by Beatrix Moersch aka Framing Noise, used in Mechanical Arm (https://freesound.org/people/Framing_Noise/sounds/256911/)  
Bubbles 004 SFX by ristooooo1, used in Cup of Expresso (https://freesound.org/people/ristooooo1/sounds/539819/)  
Sweep Downwards SFX by Alastair Pursloe, used in Rift Lens final countdown (https://freesound.org/people/stair/sounds/387552/)  
Clock Ticking SFX by Mortifreshman, used in Rift Lens final countdown (https://freesound.org/people/Mortifreshman/sounds/237210/)  
Metronome SFX by Druminfected, used in Metronome (https://freesound.org/people/Druminfected/sounds/250552/)  
Single Kick (Reverb) SFX by YellowTree, used in Metronome (https://freesound.org/people/YellowTree/sounds/172709/)  
Short Success Sound Glockenspiel Treasure Video Game SFX by FunWithSound, used in Metronome (https://freesound.org/people/FunWithSound/sounds/456965/)  
Sounds from the Disc Room Game Jam audiopack by doseone, used in Timely Execution (https://discroom.com/game-jam-tutorial-audio.zip)  
Other sounds made with sfxia by rxi (https://rxi.itch.io/sfxia)  
Splotchy Metal, Dull Metal, Scuffed Plastic, Crisscross Foam, Polished Streaked Marble Top & Light Bumpered Rock normal maps from FreePBR.com (https://freepbr.com/)  
  
### What's new?
#### 2.0.0:
* **Added 9 New Items** dont forget to update the item count in the desc and in the readme header
    * New Item: Manuscript
    * New Item: Platinum Card
    * New Item: Super Idol
    * New Item: Metronome
    * New Item: Mystic Sword
    * New Equipment: Warning System
    * New Equipment: Mechanical Arm
    * New Lunar Item: Moonglasses
    * New Lunar Item: Puzzle of Chronos
* **Added 2 New Challenges**
    * New Challenge: Smart Shopper
    * New Challenge: Beat the Heat
* **Gameplay Changes**
    * Items:
        * Spine Implant:
            * Armor: ~~10 (+10 per stack)~~ ⇒ 15 (+15 per stack)
        * Cup of Expresso:
            * Max Express Boost: ~~3 (+3 per stack)~~ ⇒ 3 (+2 per stack)
            * Express Boosts are no longer removed when losing an item
            * Picking up Lunar items now grants boosts
        * Wireless Voltmeter:
            * Base Shield: ~~4%~~ ⇒ 12%
            * Damage Reflection: ~~800% (+800% per stack)~~ ⇒ 1600% (+800% per stack)
        * Crystallized World:
            * Enemies that are immune to freeze now get a new debuff, Crystallized, that locks all of their skills and roots them in place
        * Thought Processor:
            * Effect changed: now increases attack speed for every 1% health missing
        * Timely Execution:
            * Cooldown: ~~60s (-33% per stack)~~ ⇒ 60s (-50% per stack)
        * Gate Chalice:
            * Cooldown: ~~140s~~ ⇒ 60s
            * Now removes 2 random items instead of giving an affliction
            * Now teleports to the final stage if the Primordial Teleporter is aligned with the moon
        * Rift Lens:
            * Rift Chests were replaced with Unstable Rifts
            * Unstable Rifts: ~~1 (+1 per stack)~~ ⇒ 3 (+3 per stack)
            * Downside changed: instead of getting slowed down, the owner will die if they fail to close all Unstable Rifts in a certain amount of time
        * Nuclear Accelerator:
            * Effect changed: now increases damage by 1% for every 2% of movement speed increase on the owner, and increases movement speed by 14% on the first stack
        * Contraband Gunpowder:
            * Radius: ~~8m (+1.6m per stack)~~ ⇒ 15m (+3m per stack)
            * Damage: ~~250% (+200% per stack)~~ ⇒ 500% (+400% per stack)
            * Proc Coefficient: ~~0~~ ⇒ 1
            * Powder Flask Chance: ~~10%~~ ⇒ 25%
        * Treasure Map:
            * Now drops a Legendary item for free instead of creating a Legendary Chest
        * Vintage Microphone:
            * Debuff no longer increases enemy skill cooldowns
            * Debuff now reduces enemy attack speed and enemy damage
            * Now also boosts the owner in the opposite direction on use
        * Relentless Vendetta:
            * Renamed to Vendetta
            * Buff Time: ~~8s (+8s per stack)~~ ⇒ 16s (+16s per stack)
            * Effect is now triggered only when an ally is killed by an enemy. Deaths from health decay (Squid Polyp, Happiest Mask), timers (The Back-Up) and on-demand ally kills (Engineer Turret, Rein's Sniper Decoy) no longer trigger this item.
        * Ratio Equalizer:
            * Cooldown: ~~20s~~ ⇒ 45s
            * Effect changed: now makes the current health fraction of all nearby characters equal to yours
        * Scratch Ticket:
            * No longer affects items that are unaffected by Luck
    * Challenges:
        * Last of Us:
            * Now requires being the only member of the ally team alive instead of being the only player alive
        * Sincere Apologies:
            * Now requires giving a Sawmerang to an Equipment Drone with the Artifact of Chaos enabled and dying from it
        * Enemy Spotted:
            * Broken Spotter Drone is now hidden in a location accessible to all characters without a double jump
        * The challenges listed above will be reset when loading the new version for the first time
* **Misc Changes**
    * Added new config files:
        * MysticsItems_General - general mod settings
        * MysticsItems_Balance - item and equipment value tweaking
    * Added on-player item displays for Crystallized World and Mysterious Monolith
    * Added MysticsRisky2Utils dependency
    * Crit Chance display in BetterUI is now affected by Scratch Ticket
        * May not be compatible with other mods that override the Crit Chance display in BetterUI
        * Can be disabled in the General config
    * Enemy Spotted and Burn to Kill challenge descriptions were made clearer
    * Broken Spotter Drone is no longer destroyed on repair, allowing for multiple people to repair it at once
    * Broken Spotter Drone is 50% bigger
    * The flames of the guaranteed Legendary Mask on Scorched Acres now fly higher to make searching for it easier
    * Express Boost buff icon color changed from gray to light blue to make it stand out as a positive effect
    * Rift Lens is now locked behind the Beat the Heat challenge
    * Improved Nuclear Accelerator model
    * Improved Cup of Expresso SFX
    * Improved Contraband Gunpowder SFX and VFX
    * Added random pitch variation to Ratio Equalizer SFX
    * Several item models now use normal maps for less plain light reflections
    * Artifact of Enigma will no longer roll Wirehack Wrench and Gate Chalice
        * These changes can be reverted with the new Balance config
    * Removed mysticsitems_unlocklogs and mysticsitems_grantall console commands
        * Instead, use [CheatUnlocks](https://thunderstore.io/package/TheMysticSword/CheatUnlocks/) or [UnlockAll](https://thunderstore.io/package/mistername/UnlockAll/)
    * Removed item disabling config
* **Bug Fixes**
    * Fixed Treasure Map zone being visible with 1m radius when nobody has the item
    * Fixed Timely Execution giving invincibility only to hosts and monsters
    * Fixed Crystallized World screen flash effect applying to players outside of the teleporter radius
    * Fixed certain text strings not getting translated due to regional settings
    * Fixed Scratch Ticket being banned from enemies and Engineer Turrets
    * Fixed wrong outline colour on lunar item icons
    * Fixed the flames of the Legendary Mask unlockable pickup not fading out when it cools off
    * Fixed Spine Implant triggering on blocked damage and jump pads
  
(Previous changelogs can be found [here](https://github.com/TheMysticSword/MysticsItems/blob/main/CHANGELOG.md))

![](https://i.imgur.com/gBBfdeO.png)
