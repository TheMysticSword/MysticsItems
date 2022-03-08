#### 2.0.4:
* **Updated for the 1st March 2022 version of the game**
    * Note: the mod wasn't fully tested on the new version. Bug reports are appreciated!
* **Gameplay Changes**
    * Marwan's Ash:
        * Damage: ~~5 (+1 per level)~~ ⇒ 2 (+0.2 per level)
        * Damage Over Time Duration: ~~10s~~ ⇒ 2s
        * Level for First Upgrade: ~~14~~ ⇒ 17
        * Level for Second Upgrade: ~~21~~ ⇒ 23
    * Rift Lens:
        * Rifts: ~~3 (+3 per stack)~~ ⇒ 3 (+1 per stack)
        * Countdown time now becomes slightly shorter on each loop
    * Fragile Mask:
        * Cooldown: ~~3s~~ ⇒ 0s
    * Mystic Sword:
        * Now doesn't trigger on non-Teleporter-boss enemies after reaching stage 6
            * This should prevent the item from becoming too powerful on loops
    * Cutesy Bow:
        * Now has an internal 0.5s timer that prevents multi-hits and damage-over-time effects from breaking the item too quickly
    * Ceremony of Perdition:
        * Shared Damage: ~~25% (+25% per stack)~~ ⇒ 10% (+10% per stack)
    * Devil's Cry:
        * Moved to Legendary tier
        * Hits with less than 1.0 proc coefficient contribute less to the hit counter
    * Super Idol:
        * Armor at Full Power: ~~50~~ ⇒ 90
    * Mechanical Arm:
        * Damage Bonus Per Crit: ~~200%~~ ⇒ 100%
        * Charge buff timer is now affected by proc coefficient, and runs out faster when the equipment is charged
    * Treasure Map:
        * Now spawns an item for each player in Multiplayer
* Added a new config file - MysticsItems_ContentToggle.cfg
* Added Spanish translation - thank you, RCaled!
* Added screenshake to Contraband Gunpowder explosions for greater impact
* Fixed Nuclear Accelerator damage bonus being 100x less effective than intended
* Fixed Contraband Gunpowder FX not clearing themselves until stage teleportation, leading to high memory usage
* Fixed non-Primary skills without cooldowns having the ability to trigger Metronome multiple times in a single beat
* Fixed Cup of Expresso not playing SFX on clients
#### 2.0.3:
* Added item displays for SniperClassic
* Choc Chip no longer affects the Invincibility buff
* Devil's Cry can no longer proc itself in a proc chain
* Fixed Manuscript and Mystic Sword hover tooltips in the Tab menu overriding other item descriptions when these items are lost (through Scrappers, 3D Printers, etc.)
* Fixed Spare Wiring affecting drones bought after the owner had lost the item
#### 2.0.2:
* Increased Warning System's charge radius from 30m to 50m
* Improved Rift Lens time calculation
* Set Ceremony of Perdition's proc coefficient to 0
    * Can be reverted with the Balance config
* Reduced Mystic Sword SFX volume
* Fixed Ceremony of Perdition being able to proc itself
* Fixed Last of Us challenge unlocking on killing Birdsharks on Distant Roost and Wandering Vagrant's tracking bombs instead of the final boss
* Fixed Smart Shopper challenge failing on purchasing a Multishop Terminal of any item tier other than common
* Fixed ItemStats display for Spare Wiring and Ceremony of Perdition being 100x higher
* Fixed Wireless Voltmeter not giving base shield
* Fixed Ceremony of Perdition not giving base crit chance
* Fixed Gate Chalice not doing anything on use
* Fixed Puzzle of Chronos additional scaling not resetting between runs
#### 2.0.1:
* Fixed a bug where the current profile fails to load if HereticUnchained mod is enabled
#### 2.0.0:
* **Added 20 New Items**
    * New Item: Manuscript
    * New Item: Platinum Card
    * New Item: Super Idol
    * New Item: Metronome
    * New Item: Spare Wiring
    * New Item: Mystic Sword
    * New Item: Cutesy Bow
    * New Item: Hiker's Backpack
    * New Item: Choc Chip
    * New Item: Failed Experiment
    * New Item: Devil's Cry
    * New Item: Ceremony of Perdition
    * New Item: Marwan's Ash
    * New Item: Ten Commandments of Vyrael
    * New Equipment: Warning System
    * New Equipment: Mechanical Arm
    * New Equipment: From Omar With Love
    * New Lunar Item: Moonglasses
    * New Lunar Item: Puzzle of Chronos
    * New Lunar Equipment: Fragile Mask
* **Added 3 New Challenges**
    * New Challenge: Smart Shopper
    * New Challenge: Beat the Heat
    * New Challenge: The Summit
* **Added 8 New Lore Entries**
    * New Lore Entry: Manuscript
    * New Lore Entry: Cup of Expresso
    * New Lore Entry: Platinum Card
    * New Lore Entry: Mystic Sword
    * New Lore Entry: Super Idol
    * New Lore Entry: Wirehack Wrench
    * New Lore Entry: Mechanical Arm
    * New Lore Entry: Fragile Mask
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
            * Buff Duration: ~~6s~~ ⇒ 9s
            * The buff now also increases movement speed
        * Gate Chalice:
            * Cooldown: ~~140s~~ ⇒ 60s
            * Now removes 3 random items instead of giving an affliction
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
        * Wirehack Wrench:
            * Now drops 2 copies of a Printer's item instead of 1
    * Challenges:
        * Last of Us:
            * Now requires dealing the final blow to the final boss as the sole surviving ally instead of escaping the moon as the only player alive
        * Sincere Apologies:
            * Now requires giving a Sawmerang to an Equipment Drone with the Artifact of Chaos enabled and dying from it
        * Enemy Spotted:
            * Broken Spotter Drone is now hidden in a location accessible to all characters without a double jump
        * The challenges listed above will be reset when loading the new version for the first time
* **Misc Changes**
    * Added new config files:
        * MysticsItems_General - general mod settings
        * MysticsItems_Balance - item and equipment value tweaking
    * Added French translation by Vyrael
    * Temporarily removed the Turkish translation
    * Added on-player item displays for Crystallized World and Mysterious Monolith
    * Added MysticsRisky2Utils dependency
    * Crit Chance display in BetterUI is now affected by Scratch Ticket
        * May not be compatible with other mods that override the Crit Chance display in BetterUI
        * Can be disabled in the General config
    * Slightly improved Wirehack Wrench printer detection
    * Enemy Spotted and Burn to Kill challenge descriptions were made clearer
    * Broken Spotter Drone is no longer destroyed on repair, allowing for multiple people to repair it at once
    * Broken Spotter Drone is now 50% bigger
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
    * Fixed Black Monolith texture becoming white on the one of Titanic Plains' variants
    * Fixed Legendary Mask not being displayed on Scavengers and the Engineer
#### 1.1.13:
    * Added Turkish translation by Omar Faruk
#### 1.1.12:
    * Fixed Treasure Map zone being visible with 1m radius when nobody has the item
        * (Note added in 1.1.13) This bugfix was not done correctly, therefore the bug is present in 1.1.12 and 1.1.13
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
#### 1.1.10:
* Added VFX for entities under Deafened debuff from the Vintage Microphone
* Fixed model overlay shaders being 100 times bigger on certain modded characters
* Fixed Vintage Microphone projectiles colliding with entities and the world
* Fixed Vintage Microphone projectile hitbox being rotated by 90 degrees
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
#### 1.1.7:
* Added item displays for the Bandit
* Added config options for disabling specific items
    * Notes regarding disabling challenge-locked items:
        * Respective challenges will not be disabled
        * Respective challenges will not be sorted with the other item challenges, and instead will be put at the end of the challenge list
        * This might lead to assets required for completing the challenge not being loaded, preventing you from completing the challenge
* Fixed Treasure Map and Timely Execution not working properly in multiplayer
* Fixed Voltmeter not giving shield
* Fixed a bug with the Logbook failing to load with the mod enabled
* Fixed Legendary Mask interactable not being pickable
#### 1.1.6:
* Fixed Treasure Map base charge time being 1s instead of 120s
* Fixed Engineer being forced to place turrets for the rest of the stage once the special ability is used
* Fixed Rift Chests sharing item drop rates with the Rusty Lockbox
* Fixed LanguageAPI failing to load when the mod is enabled, preventing strings from loading in other mods
    * Note: if this issue persists, manually delete the `MysticsItems.language` file in the mod folder
#### 1.1.5:
* Now works on the 1.1 version of the game
* Tuning Fork:
    * Cooldown: ~~45s~~ ⇒ 20s
* Scratch Ticket:
    * Reward: ~~$30 (+$20 per stack)~~ ⇒ 100% (+100% per stack) of the current Shrine cost
* Crystallized World:
    * Now works in all holdout zones
* Treasure Map:
    * Now works as a holdout zone, allowing the use of Lepton Daisy, Focused Convergence and Crystallized World
* Donut:
    * Extra Barrel Spawns: ~~0~~ ⇒ 1 (+1 per stack)
* Fixed Legendary Mask's second Archaic Wisp from the Artifact of Swarms not having health decay and stat buffs
* Fixed Frost Relic not functioning when the mod is enabled
* Fixed wrong ItemStats on Cup of Expresso and Scratch Ticket
* Fixed Treasure Map chest cost not scaling with difficulty
* Fixed Spine Implant particle count scaling based on the ratio between damage taken and green health instead of total health, causing a lot of particles to appear with Transcendence
* Fixed Scratch Ticket giving reward every time a Shrine of Chance with 2 wins is used, giving the reward every time you fail a shrine with 2 wins while carrying a Mysterious Monolith
* Fixed Crystallized World floating up before 100% charge despite not being able to trigger on 100% charge
* Fixed broken Spotter interactable not being pingable
#### 1.1.4:
* Added lore entries for Donut, Treasure Map, Spine Implant
* Added missing item displays
    * Mysterious Monolith appears around shrines instead of being visible on the player
* Added particles and sounds to Spine Implant
* Treasure Map now creates the chest immediately, but locks it until you charge the zone, allowing you to see the price beforehand
* Contraband Gunpowder:
    * Powder Flask Drop Chance: ~~100%~~ ⇒ 10%
        * This change was made to make the item depend more on synergies with other pickup-dropping items
* Banned Wireless Voltmeter from enemies
* Unbanned Nuclear Accelerator from enemies
* Unbanned Contraband Gunpowder from Scavengers
* Fixed Treasure Map not spawning the chest with the Artifact of Sacrifice
* Fixed Treasure Map not spawning the chest with TILER2
* Fixed Ratio Equalizer visuals being squished when playing as certain characters
* Fixed `mysticsitems_grantall` console command missing
#### 1.1.3:
* Fixed wrong info in the readme
#### 1.1.2:
* Treasure Map:
    * Excavation Time: ~~60s (-10s per stack)~~ ⇒ 120s (-40s per stack)
    * Changed tier to red
    * Now spawns a legendary chest instead of giving a gold reward
* Spine Implant:
    * Armor: ~~5 (+5 per stack)~~ ⇒ 10 (+10 per stack)
* Relentless Vendetta:
    * Buff Duration (same stage death): ~~2s~~ ⇒ 2s (+0.5s per stack)
* Contraband Gunpowder:
    * Damage: ~~200% (+150% per stack)~~ ⇒ 250% (+200% per stack)
    * Radius: ~~8m (+1.5m per stack)~~ ⇒ 8m (+1.6m per stack)
    * Powder flasks now drop on kill with 100% chance instead of dropping on hit with 7% chance
    * Powder flasks no longer roll down the ground, and disappear after 60 seconds instead of 10
    * Reduced explosion visuals
* Now compatible with ItemStats
* Nuclear Accelerator now causes you to emit particles to indicate its' activity, emitting more particles the more speed increase you have
* Temporarily removed the character item system until more items are added
* Relentless Vendetta SFX will no longer play from deaths of allies that spawned on the same stage
* Fixed Treasure Map and Rift Lens working in Hidden Realms
* Fixed Timely Execution causing a crash for clients on death
* Fixed Timely Execution visual animations not being networked
#### 1.1.1:
* Fixed Wirehack Wrench and Crystallized World not working without DebugToolkit
* Fixed Proximity Nanobots triggering even if you don't have the item in your inventory
* Fixed Crystallized World missing on Primordial Teleporter
#### 1.1.0:
* Character items are no longer found in regular drop pools. Instead, an airdrop will occur every 10 minutes in a random spot, containing one random character item for each person in the lobby. If one of the characters doesn't have any character items for them, scrap is dropped. If nobody has any items tied to their character, the airdrop doesn't occur.
* Added one new character item for Commando and Artificer
* Donut:
    * Fractional Healing: ~~25% (+5% per stack)~~ ⇒ 10% (+10% per stack)
* Treasure Map:
    * Radius: ~~20m~~ ⇒ 15m
    * Changed function: now grants $100 gold after standing in the zone for 60 seconds
* Scratch Ticket;
    * Reward: ~~$40 (+$10 per stack)~~ ⇒ $30 (+$20 per stack)
* Nuclear Accelerator:
    * Changed function: now increases damage by 1% for each 2.5% movement speed increase you have
* Contraband Gunpowder:
    * Damage: ~~350% (+80% per stack)~~ ⇒ 200% (+150% per stack)
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
* Renamed Hexahedral Monolith to Mysterious Monolith
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
