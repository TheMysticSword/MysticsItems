# MysticsItems
Adds 21 new items, with plenty more to come!  

![](https://i.imgur.com/qbVukcN.png)  

![](https://i.imgur.com/WYCK8vE.gif)  

Also features a new subset of character-specific items that can only drop if one of the players are using a specific character and can only be picked up by that character. Currently, this only includes one Commando item.

### Console commands
`mysticsitems_grantall` - grants all achievements  
`mysticsitems_unlocklogs` - unlocks all logbook entries  

### Bug reports and suggestions
Please visit the issue tracker on the mod's GitHub repository! https://github.com/TheMysticSword/MysticsItems/issues  
Before creating a new issue, please make sure that a similar one hasn't already been created. You can also check the Milestones tab to see the progress on the next update and what bugs have already been fixed internally.

### To-do
* More character-specific items  
* Missing item displays on characters  
* Missing logbook lore entries  

### Credits
TheMysticSword - Coding  
Marwan60 - Modelling  
Tuning fork SFX for Ratio Equalizer provided by Martin Chappell (https://freesound.org/people/martian/sounds/19308/)  
Other sounds made with sfxia by rxi (https://rxi.itch.io/sfxia)  
  
### Changelog
#### 1.0.2:
* Character items are no longer found in regular drop pools. Instead, an airdrop will occur every 10 minutes in a random spot, containing one random character item for each person in the lobby. If one of the characters doesn't have any character items for them, the other characters get one extra. If nobody has any items tied to their character, the airdrop doesn't occur.
* Donut:
    * Fractional Healing: ~~25% (+5% per stack)~~ ⇒ 10% (+10% per stack)
* Treasure Map:
    * Radius: ~~20m~~ ⇒ 15m
    * Changed function: now grants $100 gold after standing in the zone for 60 seconds
* Scratch Ticket;
    * Reward: ~~$40 (+$10 per stack)~~ ⇒ $30 (+$20 per stack)
* Tactical Scope:
    * Falloff Reduction: ~~5m (+5m per stack)~~ ⇒ 10m (+10m per stack)
* Nuclear Accelerator:
    * Changed function: now increases damage by 1% for each 2.5% movement speed increase you have
* Contraband Gunpowder:
    * Damage: ~~350% (+80% per stack)~~ ⇒ 200% (+150% per stack)
    * Radius: ~~8m (+1.5m per stack)~~ ⇒ 10m (+1.5m per stack)
    * Reduced visual explosion intensity
* Thought Processor:
    * Cooldown Reduction: ~~12% (+2% per stack)~~ ⇒ 10% (+5% per stack)
* Wireless Voltmeter:
    * Damage: ~~800% (+100% per stack)~~ ⇒ 800% (+800% per stack)
* Legendary Mask
    * Cooldown: ~~90s~~ ⇒ 140s
    * Damage: ~~250%~~ ⇒ 300%
* Banned Wireless Voltmeter from Mithrix
* The console will now print an error on mod load if one of the character stat hooks fails to activate for easier detection of mod incompatibilities
* Reduced the amount of little gunpowder particles from Contraband Gunpowder pickups and disabled their collision with the world
* Renamed the Hexahedral Monolith to Mysterious Monolith
* Fixed TILER2 incompatibility
* Fixed CrystalWorldRender error
* Fixed Treasure Map zone being visible even when nobody has the item
* Fixed a hard lock when anyone has a Gate Chalice debuff by the end of a telporter even
* Fixed Faulty Spotters not disappearing after death
* Fixed broken Spotter interactable having oversized sparks
* Fixed Thought Processor triggering on Loader's primary skill
#### 1.0.1:
* Nuclear Accelerator:
    * ~~10% damage increase per 25% extra speed~~ ⇒ 10% damage increase per 100% extra speed
    * Buff Time: ~~2s (+2s per stack)~~ ⇒ 6s (+4s per stack)
    * Now calculates extra speed based off movement speed instead of sprinting speed
* Wireless Voltmeter:
    * Proc Coefficient: ~~1.0~~ ⇒ 0.0
        * This change was made to prevent uncontrollable procs of elemental bands
* The following items are no longer inherited by deployables: Treasure Map, Rift Lens, Tactical Scope, Scratch Ticket, Hexahedral Monolith, Crystallized World
* Fixed wrong calculations on Nuclear Accelerator
* Fixed Shrines of Chance having empty drop pools
* Fixed Command Essences showing undiscovered modded items as locked
* Fixed a Russian string in the English version of the Contraband Gunpowder's logbook description
* Fixed Scratch Ticket proccing on each win after the first one instead of only on the second win
* Fixed Engineer getting infinite turrets
* Fixed ItemDropList incompatibility

![](https://i.imgur.com/gBBfdeO.png)
