# MysticsItems
Adds 20 new items, with plenty more to come!  

![](https://i.imgur.com/WYCK8vE.gif)  
![](https://i.imgur.com/GZ1eiSK.png)  

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
If you have a general suggestion or a bug report, please make an issue about it on the issue tracker: https://github.com/TheMysticSword/MysticsItems/issues  
Before creating a new issue, please make sure that a similar one hasn't already been created. You can also check the Milestones tab to see the progress on the next update and what bugs have already been fixed internally.
If you want to suggest an idea for a new item, make a post on the Item Ideas discussion category: https://github.com/TheMysticSword/MysticsItems/discussions/categories/item-ideas

### Credits
TheMysticSword - Coding  
Marwan60 - Modelling  
Tuning fork SFX for Ratio Equalizer provided by Martin Chappell (https://freesound.org/people/martian/sounds/19308/)  
Other sounds made with sfxia by rxi (https://rxi.itch.io/sfxia)  
  
### What's new?
#### 1.1.8:
* Now works on game version 1.1.1.4
* Donut:
    * Now triggers on all interactables
* Scratch Ticket:
    * Changed function: now increases chance of luck-based effects
* Faulty Spotter:
    * Cooldown: ~~20s~~ ⇒ 30s
        * The 20 second cooldown wouldn't acccount the 10 seconds of the crit-marked debuff, causing the item to have 10 seconds of effective cooldown. This change was made to fix the issue.
* Relentless Vendetta:
    * Buff Duration: ~~15s (+5s per stack)~~ ⇒ 15s (+15s per stack)
    * Buff Duration (same-stage death): ~~2s (+0.5s per stack)~~ ⇒ 1s (+1s per stack)
* Cup of Expresso:
    * Maximum Buffs: ~~3 (+1 per stack)~~ ⇒ 3 (+3 per stack)
* Contraband Gunpowder now interacts with Starstorm 2's Stirring Soul
* Updated Treasure Map effects
* Updated Cup of Expresso effects
* Team-based items (Crystallized World, Treasure Map and Mysterious Monolith) now work only if their owners are alive
* Fixed Treasure Map behaving differently for clients and hosts in multiplayer
* Fixed Timely Execution having no cooldown
* Fixed Donut healing orb size not being networked  
  
(Previous changelogs can be found [here](https://github.com/TheMysticSword/MysticsItems/blob/main/CHANGELOG.md))

![](https://i.imgur.com/gBBfdeO.png)
