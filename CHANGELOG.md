#### 2.1.13:
* Gachapon Coin:
    * Passive Crit Bonus: ~~0.5% (+0.5% per stack)~~ ⇒ 1% (+1% per stack)
* Marwan's Ash:
    * Damage: ~~4 (+0.8 per level)~~ ⇒ 2 (+0.4 per level)
* Marwan's Light:
    * DoT Damage: ~~0.2% (+0.02% per level beyond 17)~~ ⇒ 0.2% (+0.04% per level beyond 17)
    * DoT Damage Cap: 800% of base damage ⇒ 1000% of base damage
        * Can now be configured
* Spare Wiring:
    * Spark Damage (from drones): ~~200% (+160% per stack)~~ ⇒ 125% (+100% per stack)
    * Spark Damage (from players): ~~40% (+32% per stack)~~ ⇒ 50% (+40% per stack)
* Treasure Map:
    * Unearth Time: ~~60s~~ ⇒ 120s
* Ten Commandments of Vyrael:
    * The additional 'On-Hit' event now has a 2.0 proc coefficient instead of inheriting the proc coefficient of the triggering hit
        * This change should make the item more of a sidegrade to 57 Leaf Clover, significantly benefitting frequent attacks with low proc coefficient
* Rift Lens:
    * Rift Count: ~~3 (+1 per stack)~~ ⇒ 2 (+1 per stack)
* Puzzle of Chronos:
    * Health Regen: ~~3.2 HP/s (+3.2 HP/s per stack)~~ ⇒ 3 HP/s (+3 HP/s per stack)
* Added item displays for Chirr from Starstorm 2
* Fixed Snow Ring and Wireless Voltmeter being able to hit the same enemy multiple times if it has more than 1 bullseye hitbox (e.g. Magma Worm)
* Updated Korean translation
#### 2.1.12:
* Wireless Voltmeter:
    * Moved from Legendary to Uncommon tier
    * Passive Shield: ~~12%~~ ⇒ 10%
    * Damage Reflection: ~~1600% (+800% per stack)~~ ⇒ 300% (+300% per stack)
* Ten Commandments of Vyrael:
    * Effect adjusted: 10th hit no longer guarantees 'On-Hit' effects, and only triggers an 'On-Hit' event an additional time, as if the enemy was immediately struck for the 11th time
    * The hit count is no longer affected by proc coefficient
* Super Idol:
    * Health Increase: ~~+60%~~ ⇒ +50%
    * Armor Increase: ~~+60~~ ⇒ +50
* Devil's Cry:
    * Effect Trigger Interval: ~~every 9 crits~~ ⇒ every 5 crits
* Moonglasses:
    * Bonus Crit Damage: ~~+150% (+150% per stack)~~ ⇒ +400% (+400% per stack)
        * The previous bonus caused the crit damage to average out to less than normal. For example, if the player had 100% crit chance and picked up one pair of Moonglasses, their average crit damage would become: `(2 + 1.5) / 2 = 1.75x`, which is lower than the usual 2x from crits. That made the item beneficial only after regaining full crit again, which caused the item's effect to be boring and reliant on luck in finding crit-increasing items. This new change makes the item increase the player's average damage with 1 stack while retaining the downside of making on-crit effects trigger half as often.
* Added item displays for Deputy (https://thunderstore.io/package/Bog/Deputy/)
* Added a tip in Manuscript's description to hover over the item in-game to check current stat bonuses
* Fixed Smart Shopper and Beat the Heat challenges becoming impossible to complete after failing until a game restart
* Fixed Marwan's Ash triggering on zero-damage hits (e.g. Railgunner Concussion Device, Bell Totem from Aetherium)
* Fixed Purrfect Headphones not showing soundwaves around its item model when the buff is active
* Fixed Metronome's "Disable Sound" and "Crit Loss" options not appearing in the Risk of Options menu
* Potentially fixed Stargazer's Records star pickups being invisible for clients in co-op
* Adjusted the volume of some sound effects
#### 2.1.11:
* Added Korean translation (thanks to SAMA0613 & p157157 on github!)
* Re-added Turkish translation from v1.1.13 (strings from newer versions will be missing)
#### 2.1.10:
* Reduced the frequency of Rift Lens's Unstable Rift projectile firing
* Reduced the intensity of Super Idol's character glow
* Fixed Unstable Rifts' "attacker entity" being technically vulnerable
* Fixed some BetterUI ItemStats calculations being multiplied by 100
#### 2.1.9:
* Added changelog to Thunderstore
#### 2.1.8:
* Wirehack Wrench:
    * Cooldown: ~~140s~~ ⇒ 45s
* Added a PassiveShieldPerStack config option to Charger Upgrade Module (0% by default)
* Added an overload for ElitePotion_AddSpreadEffect that takes R2API.ModdedDamageType instead of DamageType
* Fixed Fragile Mask killing on taking hits that deal 0 damage (e.g. Railgunner Concussion Device)
* Custom sounds now have a low pass filter at low health, which should also work with Fogbound Lagoon's underwater system
* Removed unnecessary OrbAPI dependency
#### 2.1.7:
* Marwan's Light:
	* The burn is no longer upgraded by Ignition Tank
		* Can be reverted by setting the new Gameplay config option "Marwan's Light Ignition Tank" to true
* Mystic Sword:
	* Damage Per Kill: ~~+2% (+2% per stack)~~ ⇒ +2%
* Wirehack Wrench:
	* Items Dropped: ~~2~~ ⇒ 1
* Spine Implant:
	* Armor: ~~15 (+15 per stack)~~ ⇒ 20 (+20 per stack)
* Constant Flow:
	* Slow Reduction: ~~35% (+35% per stack)~~ ⇒ 20% (+20% per stack)
	* Initial Root Slow: ~~80%~~ ⇒ 90%
	* Root Slow Reduction: ~~15% (+15% per stack)~~ ⇒ 10% (+10% per stack)
* Cutesy Bow:
	* Armor: ~~40~~ ⇒ 30
* Cup of Expresso:
	* Movement and Attack Speed Bonus Per Buff: ~~+7%~~ ⇒ +10%
* Moonglasses:
	* Crit Damage: ~~+100% (+100% per stack)~~ ⇒ +150% (+150% per stack)
* Puzzle of Chronos:
	* Regeneration: ~~+6 HP/s (+6 HP/s per stack)~~ ⇒ +3.2 HP/s (+3.2 HP/s per stack)
* Edited some item tags for more accurate Benthic Bloom transformations and effects from tag-related modded items:
	* Removed Utility tag from Gachapon Token, Ghost Apple, Donut, Vendetta, Contraband Gunpowder, Nuclear Accelerator, Crystallized World, Wireless Voltmeter
	* Added Healing tag to Ghost Apple
	* Added Damage tag to Cup of Expresso, Contraband Gunpowder
	* Added Utility tag to Super Idol
	* Added InteractableRelated tag to Spare Wiring
* Deleted "Sincere Apologies." challenge and made Timely Execution unlocked by default
* Added "Initial Crit Stacking" Gameplay config option, allowing items that give a crit chance on the first stack to also give crit chance on extra stacks
* Snow Ring and Wireless Voltmeter now cannot be triggered by Vengeance doppelgangers
* Fixed Snow Ring icon being twice as big when scrapped/printed away
* Fixed Spikestrip 2.0 mod incompatibility where Rift Lens projectiles were invisible and spammed console errors
* Fixed Rift Lens giving less time after a respawn
#### 2.1.6:
* Updated Simplified Chinese translation - thank you, Dying_Twilight! <3
* Marwan's Ash:
	* Stacking now makes its per-level bonuses stronger by +25% instead of +10%, but is now also hyperbolic and capped up to +100% (like Tougher Times)
* Nuclear Accelerator:
	* Damage Per 1% Speed: ~~+0.5% (+0.25% per stack)~~ ⇒ +0.25% (+0.25% per stack)
* Mystic Sword:
	* Damage Cap: ~~40% (+40% per stack)~~ ⇒ 40% (+20% per stack)
* Rift Lens:
	* Unstable Rifts now fire random bolt projectiles and are guarded by ice walls!
* Added BetterUI stats for items added in 2.1.0
* Fixed Black Monolith little displays appearing near shrines when nobody has the item
* Fixed Black Monolith occasionally not giving extra shrine uses
* Fixed Time Dilator causing error spam and invisible enemies when used by Engineer Turrets
#### 2.1.5:
* Vendetta:
	* Buff Duration: ~~16s (+16s per stack)~~ ⇒ 16s (+4s per stack)
	* Buff duration on enemies can now be configured (by default: 0.5s (+0.125s per stack))
* Choc Chip config now has an Ignored Buff Names option that you can use to select which buffs Choc Chip should not affect
	* By default, it ignores:
		* Medkit delayed heal, because it's technically counted as a buff, so Choc Chip would *extend* it instead of shortening
		* Invincibility, because extending it makes Merc too powerful
		* All of the Void Fog debuffs, because they are counted as buffs, and therefore would get extended
* Cup of Expresso buffs are no longer removed when all stacks of the item are gone
* Treasure Map zone now has a minimum radius of 2.25m to prevent it from being unusable with Focused Convergence stacks
* Fixed Black Monolith little monoliths around shrines disappearing when the shrines are used up
* Fixed Keep Up The Rhythm challenge counting as complete when reaching Void Fields
* Fixed Marwan's Light burn stacking damage on each hit
#### 2.1.4:
* Fixed everything that the last patch broke. Sorry!!
* Added a config option to revert the previous patch's Warning System fix with Shrines of the Mountain (under Gameplay > Warning System Mountain Shrines)
* Fixed the "Treasure Map Per-Player Drops", "Treasure Map Sea Bear Circle", "Nuclear Accelerator Alternate" and "Devil's Cry Alternate Damage" Gameplay config options not appearing in the Mod Options
#### 2.1.3:
* Updated the French translation. Thanks, Fyrebw! <3
* Removed the 0.2s delay from the Timely Execution trigger, making it activate immediately on dropping to low HP
* Crystallized World debuff now "knocks" bosses out of their attack state
* Purrfect Headphones now works in all kinds of holdout zones instead of only Teleporters
* Fixed Ghost Apple giving an Apple Stem even if the apple was scrapped beforehand
* Fixed Marwan's Ash staying active after being scrapped
* Fixed Warning System re-enabling activated Shrines of the Mountain
* Slightly optimized certain internal on-damage-taken and on-inventory-changed events. Should make the mod lag the game less?
* I didn't test any of this, so my condolences if the game doesn't load anymore after this update! -Mystic
#### 2.1.2:
* Marwan's Ash:
    * The burn's damage over time is now capped at 20000% of the owner's base damage per tick, to prevent it from being too powerful against high-health special bosses
    * Removed burn DoT particles and overlay material
* Stargazer's Records:
    * Chance: ~~20%~~ ⇒ 12%
* "'Tis But A Scratch" challenge now requires reducing 10.000 points of damage instead of 5.000
* Updated Russian localization files
* Slightly optimized on-hit events for Marwan's Weapon and Ceremony of Perdition
* Potentially fixed Marwan's Ash sometimes not upgrading
* Fixed Stargazer's Records triggering from all kinds of attacks (even with 0 proc coefficient), and also not multiplying the chance by proc coefficient
* Fixed Stargazer's Records stars not spawning for clients
#### 2.1.1:
* Ghost Apple:
    * Duration: ~~15 minutes~~ ⇒ 20 minutes
    * Apple Branch renamed to Apple Stem
* Nuclear Accelerator:
    * No longer gives bonus damage for sprinting
* Gachapon Coin:
    * Now passively gives +0.5% crit chance and +1% attack speed to prevent it from being useless on Commencement
* Fixed Snow Ring missing from the Enabled Items options
* Fixed Snow Ring world pickup being massive
* Fixed all of the new item icons have 4x size when printed/scrapped
* Fixed "Keep Up The Rhythm" and "Cool It!" challenges not completing
* Fixed Time Dilator debuff not disappearing from the player if inflicted in Void Fields
* Banned Snow Ring from enemies
* Regen-increasing item descriptions now mention that they scale with level
* Added MoffeinArchaicWisp/RiskyMod compatibility for Archaic Mask. Thanks, Moffein!
#### 2.1.0:
* **Merry Christmas!**  
![Preview of the update. Artificer with some of the new items in the Bazaar Between Time with snowflake particles.](https://i.imgur.com/lhGjR5A.png)
* **Special thanks to eM-Krow & Heyimnoop for funding the donation goal for this update!**
* **Added 10 New Items**
    * New Item: Gachapon Coin
    * New Item: Constant Flow
    * New Item: Ghost Apple
    * New Item: Inoperative Nanomachines
    * New Item: Purrfect Headphones
    * New Item: Stargazer's Records
    * New Item: Time Dilator
    * New Item: Charger Upgrade Module
    * New Item: Snow Ring
    * New Equipment: Regurgitator
* **Added 5 New Challenges**
    * New Challenge: 'Tis But A Scratch
    * New Challenge: Keep Up The Rhythm
    * New Challenge: So Many Fans!
    * New Challenge: Cool It!
    * New Challenge: Pirates of the Cariskean
* **Gameplay Changes**
    * Platinum Card:
        * Discount: ~~100%~~ ⇒ 75%
    * Super Idol:
        * Gold Cap: ~~$1200 (-50% per stack), scales with time~~ ⇒ $800 (-50% per stack), scales with time
        * Gold cap now scales to higher amounts later into the run to make it harder to reach in loop runs where gold is plentiful.
    * Hiker's Backpack:
        * Now gives an extra jump on the first stack
    * Crystallized World:
        * Freeze Duration: ~~7s~~ ⇒ 5s
* **Misc Changes**
    * Bazaar Between Time now has festive particles during Christmas and New Year celebrations. You can turn them off by disabling Fun Events in the mod settings.
    * Nuclear Accelerator now slightly tints damage numbers green
	* Re-enabled BetterUI support
        * Some info might be wrong or missing, this will be fixed in a later update
    * Added various gameplay config options:
        * Treasure Map Sea Bear Circle - make the Treasure Map zone visible as a 1m-wide circle when nobody has the item!
        * Nuclear Accelerator Alternate - make the Nuclear Accelerator increase damage depending on current velocity instead of current movement speed buff!
        * Metronome Crit Loss - lose Metronome buff stacks when using a skill off-beat!
    * Treasure Map's DropItemForEachPlayer config option was moved to the Gameplay section and renamed to Per-Player Treasure Map Drops
* **Bug Fixes**
	* Fixed config options for Equipment cooldown, Enigma and Bottled Chaos compatibility bypassing the "Ignore Balance Changes" option, which prevented Equipment cooldowns from updating with mod patches
* **Known Issues**
    * Non-English localization strings are outdated. Sorry! This will be fixed later. We just wanted to get the update out ASAP :)
#### 2.0.20:
* Super Idol:
	* Extra Health: ~~+100%~~ ⇒ +60%
* Treasure Map:
	* Zone Radius: ~~10m~~ ⇒ 7m
* Timely Execution:
	* Buff Duration: ~~9s~~ ⇒ 7s
* Rift Lens:
	* Rifts now drop only Common items
	* Rifts can no longer get locked by Teleporter events and Void Seeds
	* Made the timer *slightly* more forgiving
* Puzzle of Chronos:
	* Difficulty Scaling: ~~+20% (+20% per stack)~~ ⇒ +30% (+30% per stack)
* Metronome:
	* Crit Per Beat: ~~+0.2% (+0.2% per stack)~~ ⇒ +0.5% (+0.5% per stack)
* Fixed Metronome giving +1318927000% in Multiplayer (this time for sure)
* Fixed Marwan's Light particles being extremely large on certain modded enemies
#### 2.0.19:
* Marwan's Ash:
	* Extra Hit Damage: ~~6 (+1.2 per level)~~ ⇒ 4 (+0.8 per level)
* Cutesy Bow:
	* Armor: ~~60~~ ⇒ 40
* Vendetta:
	* Now available for enemies
	* Now gives only 0.75s of the buff if triggered by enemies
* Spare Wiring:
	* Player sparks now deal 40% contact damage instead of 200%. Drone spark damage was not changed.
	* Now banned from enemies
* Platinum Card:
	* Discount: ~~-50%~~ ⇒ -100%
* Crystallized World:
	* Now has a minimum pulse radius of 60m, preventing it from becoming weaker with smaller holdout zones (e.g. Commencement Pillars)
		* Can be reverted in the config by setting MinPulseRadius to 0
* Super Idol:
	* Health: ~~+150%~~ ⇒ +100%
	* Armor: ~~90~~ ⇒ 60
* Wirehack Wrench:
	* Cooldown: ~~90s~~ ⇒ 140s
* Added an option to disable Metronome sounds (in General > Effects)
* Potentially fixed the bug with Metronome bonus randomly reaching +1318927000%
* Marwan's Ash particles now have a higher VFX cost, which should start removing them more often on deep loop runs
#### 2.0.18:
* Fixed language tokens missing
#### 2.0.17:
* Added logbook entry for From Omar With Love
* Metronome:
	* Effect reworked - now creates a rhythm indicator that adds +0.2% crit chance until the end of the stage if a non-Primary skill is used in time with rhythm
	* Rhythm indicator is now faded to be less distracting
* Fragile Mask:
	* No longer has a 1 second lingering effect after disabling, allowing you to quickly turn it off in response to an incoming attack
	* Can be reverted in the balance config
	* Cooldown changed from 0s to 3s to make the initial activation more risky
* Enemies marked by the Faulty Spotter now trigger the extra arrows from Huntress's Flurry as if the attack was a random crit
* Contraband Gunpowder no longer hits the owner with the Artifact of Chaos
* Potentially fixed Hiker's Backpack giving infinite skill charges with certain mods
* Fixed Legendary Mask being misplaced and absurdly big on Void Fiend
* Fixed Metronome playing sounds for NPC allies that have it
* Removed the "orange" world display from Crystallized World
#### 2.0.16:
* Vintage Microphone:
	* Boost Distance: ~~15m~~ ⇒ 20m
* Contraband Gunpowder:
	* Damage: ~~500% (+400% per stack)~~ ⇒ 300% (+240% per stack)
* Warning System:
	* Cooldown: ~~90s~~ ⇒ 75s
* Metronome:
	* Increased the beat window
	* Preparation ticks now play at the speed of the current song
* Crystallized World:
	* Enemies no longer deal damage while under the effects of the Crystallized debuff
* Super Idol:
	* Gold for Max Buff: ~~$400 (-50% per stack)~~ ⇒ $1200 (-50% per stack)
	* Max Bonus Health: ~~100%~~ ⇒ 150%
* Moved the Moonglasses non-crit damage reduction for Bandit option to the General config, and set it to be disabled by default
* Removed the Moonglasses non-crit damage reduction for Railgunner option. Instead, Railgunner gets less extra damage from crit chance.
* Fixed Rift Lens instantly killing the owner under certain circumstances
* Fixed Warning System making Shrines of the Mountain available after a Teleporter gets charged
* Fixed Warning System sometimes not triggering Warbanner
* Fixed Ten Commandments of Vyrael causing elite aspects to drop when they trigger Lost Seer's Lenses (DLC1)
* Fixed From Omar With Love's pickup model being extremely large
* Fixed Moonglasses' pickup model being too small
* Fixed Platinum Card being consumed on purchasing the only remaining multishop choice
#### 2.0.15:
* Marwan's Ash:
	* Damage: ~~6 (+1.2 per level, multiplied by item stack)~~ ⇒ 6 (+1.2 (+10% per stack) per level)
* Marwan's Light:
	* Burn Over Time: ~~0.2% (+0.02% per level, multiplied by item stack)~~ ⇒ 0.2% (+0.02% (+10% per stack) per level)
	* Burn Duration: ~~2s~~ ⇒ 5s
* Marwan's Weapon:
	* Radius: ~~7m (+1.4m per level, multiplied by item stack)~~ ⇒ 7m (+1m (+10% per stack) per level)
* Legendary Mask:
	* Wisp Cooldown Reduction: ~~100%~~ ⇒ 50%
	* Wisp Attack Speed: ~~200%~~ ⇒ 150%
* Hiker's Backpack:
	* Cooldown Reduction: ~~8%~~ ⇒ 15%
* Added option to turn Frayed Bow into an untiered item
* Fixed Treasure Map and From Omar With Love holograms rendering over everything
* Fixed certain parts of the Metronome UI being invisible
* Fixed Treasure Map beeping after a Siren Pole is charged
* Fixed black screen with PlayerBots on stage transition
* Fixed Contraband Gunpowder crashing the game if the screenshake intensity is set to 0
* Fixed cooldown reduction on Archaic Wisps from Legendary Mask being 100% less than intended
* Fixed Mechanical Arm gaining charges while off cooldown
* Fixed Wireless Voltmeter giving less shield than intended with Shaped Glass, Artifact of Glass and other sources of curse
#### 2.0.14:
* Marwan's Ash:
	* Damage: ~~2 (+0.2 per level)~~ ⇒ 6 (+1.2 per level)
		* The damage was initially nerfed to 2 after testing the item with high fire rate characters such as Commando, but the item turned out too weak on other characters in the end, so we're buffing the damage back. The item also mistakenly increased its damage by 10% per level instead of 20%, which was inconsistent with how survivor damage increases by 20% with level.
* Warning System:
	* Can now be used during Teleporter events
	* Now available in Enigma and Bottled Chaos (DLC1) pools by default
* Vintage Microphone:
	* No longer pushes back the owner if it's a flying body
		* This should prevent Equipment Drones from hitting walls and taking high damage when using this equipment
* Rift Lens:
	* Timers are now individual for each player instead of being shared
		* This should fix countdown timer networking issues
* Fixed Timely Execution cooldown being blocked by Ben's Raincoat
* Fixed Item Drones from Tinker's Satchel accepting upgraded versions of Marwan's Ash and causing error spam
* Fixed Gate Chalice causing errors when used in the Bazaar Between Time
* Fixed Puzzle of Chronos regeneration not scaling with level
* Fixed Rift Vision not being reset when teleporting to a Hidden Realm before closing all Unstable Rifts, causing the user to die instantly
* Fixed all challenges having Preon Accumulator as the icon
#### 2.0.13:
* **Added 11 New Lore Entries**
    * New Lore Entry: Spare Wiring
	* New Lore Entry: Nuclear Accelerator
	* New Lore Entry: Hiker's Backpack
	* New Lore Entry: Failed Experiment
	* New Lore Entry: Warning System
	* New Lore Entry: Moonglasses
	* New Lore Entry: Cutesy Bow
	* New Lore Entry: Frayed Bow
	* New Lore Entry: Choc Chip
	* New Lore Entry: Vintage Microphone
	* New Lore Entry: Ratio Equalizer
* **Gameplay Changes**
	* Marwan's Ash:
		* Extra Damage Limit (when used by enemies): ~~21.6~~ ⇒ 4
		* Burn Damage Limit (when used by enemies): ~~18.4% of your maximum health per second~~ ⇒ 1.6% of your maximum health per second
		* Spread Radius Limit (when used by enemies): ~~113.4m~~ ⇒ 10m
	* Gate Chalice:
		* Removed the downside of removing items
			* The downside of losing out on unopened chests from skipping stages was enough already. The second downside was unnecessary.
	* Mechanical Arm:
		* Bonus Damage Per Charge: ~~100%~~ ⇒ 200%
		* Charges no longer disappear after 20 seconds
		* Now gains charges when you deal crits only while equipment is on cooldown
	* Mystic Sword:
		* Max Damage: ~~100% (+100% per stack)~~ ⇒ 40% (+40% per stack)
	* Devil's Cry:
		* Critical Strike Chance: ~~+5%~~ ⇒ +10%
		* Now triggers on every 9th Critical Strike instead of every 5th
* **Misc Changes**
	* Added Portuguese translation - thank you, Olek!
	* Added [Risk of Options](https://thunderstore.io/package/Rune580/Risk_Of_Options/) support
	* Removed the Content Toggle config and moved all of its options to the General config
	* Updated the description of the "Smart Shopper" challenge to clarify that the challenge cannot be completed with the Artifact of Sacrifice
	* "Sincere Apologies." challenge is now completed from getting hit with an Equipment Drone's Sawmerang instead of getting killed with it
	* Scratch Tickets no longer increase the visual crit chance on BetterUI's stat tracker
		* Instead, add a $mysticsitemscrit modifier to your BetterUI's StatsDisplay config to show crit chance affected by luck and Scratch Tickets
	* Removed config options for disabling mod compatibility with BetterUI, WhatAmILookingAt and ProperSave
	* Renamed all config options from the General config to be more human-readable
		* Note: this will reset your General config options
	* Added Legendary Mask to the Bottled Chaos pool (Survivors of the Void DLC)
* **Bug Fixes**
	* Fixed Gate Chalice being usable multiple times on Commencement
	* Fixed occasional version mismatch between hosts and clients when updating the mod
	* Fixed Spare Wiring's player firing count and interval not being configurable
	* Fixed ThinkInvisible's Admiral skill "Beacon: Special Order" giving an infinite amount of Marwan's Ash
	* Fixed damage effects caused by Ten Commandments of Vyrael being guaranteed to crit
#### 2.0.12:
* Fixed for the 1.2.3 version of the game
* Rift Lens countdown now gets extra time when at least one rift is inside a Void Seed
* Failed Experiment now works with Mending and Voidtouched elites
* Fixed Platinum Card not reducing prices of multishops
#### 2.0.11:
* Fixed Treasure Map beeping even when nobody has the item
#### 2.0.10:
* Marwan's Ash:
	* Now has caps for extra damage, burn damage and spread radius for non-ally characters
		* This should make the item more fair when used by enemies and the final boss
* Platinum Card:
	* Discount: ~~10%~~ ⇒ 50%
* Gate Chalice:
	* Now has special interactions on certain stages
	* Can no longer be used in the Bazaar Between Time
* Thought Processor:
	* Effect changed: using skills with cooldowns now reduces all other skill cooldowns
* Legendary Mask:
	* Can now be activated without an enemy target to spawn an Archaic Wisp nearby
* Added item displays for DLC1 survivors
* Added a reminder message about a remaining Treasure Map spot when the teleporter is charged
* Added a filling up indicator for Super Idol over the inventory icon that shows the current percentage of the gold buff
* Added General config options to toggle Contraband Gunpowder effects
* Updated Spare Wiring VFX and SFX
* Adjusted Mechanical Arm logbook model to be more arm-like
* Updated Puzzle of Chronos item display and icon
* Fixed Treasure Map not dropping any items when the 1nsiderItems mod is enabled
* Fixed Treasure Map calling certain server-only functions on clients
* Fixed Marwan's Light burn DoT counting as 2 debuffs for the Death Mark
* Fixed Choc Chip extending Fire terminal 1 second debuff
* Fixed Choc Chip not reducing the duration of damage-over-time effects
* Fixed the WhatAmILookingAt mod not detecting the mod
* Fixed Manuscript & Mystic Sword overriding extended item descriptions from BetterUI
* Fixed wrong BetterUI item stat modifier values from Scratch Ticket
* Fixed Platinum Card not playing a sound
* Fixed Puzzle of Chronos obscuring Railgunner's scope vision
* Fixed Mechanical Arm making the player ping themselves
* Fixed the "Sincere Apologies." challenge unlocking when anything dies from a Sawmerang instead of the player specifically
* Updated localization files
#### 2.0.9:
* Added Simplified Chinese translation - thank you, ACroptf8!
* Fixed Spanish translation not working
* Added DropItemForEachPlayer balance config option for Treasure Map
* Added Hits balance config option for Ten Commandments of Vyrael
* From Omar With Love now has a 0.5s hidden cooldown to prevent wasting all charges with the Gesture of the Drowned
* Logbook entries for Marwan's Light, Marwan's Weapon and Frayed Bow are now properly unlocked on item transformations
* Updated localization files
#### 2.0.8:
* Fixed ItemStats compatibility causing issues with inventory icons
* Improved the Bazaar item choice even more
#### 2.0.7:
* Marwan's Ash:
	* Burn damage is now always set to enemy health percentage, instead of choosing to deal the player's base damage if it's higher
	* No longer banned from enemies and the final boss
* Hiker's Backpack:
	* Now increases Engineer's turret limit
		* Can be reverted in the balance config
* Wirehack Wrench:
	* Cooldown: ~~45s~~ ⇒ 90s
* Vendetta:
	* Now banned from enemies
* Added BetterUI ItemStats integration
* Reduced Contraband Gunpowder explosion screenshake
* Fixed Metronome's PrepareTicks config value not applying in-game, defaulting to 3 ticks
* Fixed Puzzle of Chronos regeneration bonus being multiplied by the regen bonus of the first stack
* Fixed Nuclear Accelerator not working when the MoreItems mod is enabled
* Fixed Cutesy Bow's remaining hit count going back to 100 when continuing a game with ProperSave
* Improved the item choice in the Bazaar Between Time
	* Can be reverted in the Content Toggle config
#### 2.0.6:
* Failed Experiment:
    * Radius: ~~12 (+2.4m per stack)~~ ⇒ 15m (+3m per stack)
* Faulty Spotter:
    * Debuff Duration: ~~10s~~ ⇒ 7s
* Nuclear Accelerator:
    * Damage Per 1% Speed: ~~1% (+0.5% per stack)~~ ⇒ 0.5% (+0.25% per stack)
* Moonglasses:
    * Now halve all non-crit damage for characters with 'backstabs deal guaranteed crits' and 'convert crit chance into crit damage' passives
        * This change should fix the item being strictly positive for Bandit and Railgunner
* Puzzle of Chronos:
    * Base Ally Regen: ~~+3 HP/s (+3 HP/s per stack)~~ ⇒ +6 HP/s (+6 HP/s per stack)
* Treasure Map:
    * Time: ~~120s (-50% per stack)~~ ⇒ 60s (-25% per stack)
    * Radius: ~~15m~~ ⇒ 10m
        * These changes should make the digging event feel more dynamic (managing between standing in a small area and leaving it when enemies close in) and make it feel less like another Teleporter event
* Fixed Nuclear Accelerator not working
* Fixed Scratch Tickets adding crit chance to BetterUI when playing as Railgunner
* Fixed Manuscript and Mystic Sword bonuses resetting when loading a game with ProperSave
#### 2.0.5:
* Mystic Sword:
    * Damage: ~~3% (+3% per stack)~~ ⇒ 2% (+2% per stack)
    * Damage Bonus Limit: ~~None~~ ⇒ 100% (+100% per stack)
    * Reverted the hidden limit on non-Teleporter bosses from the previous patch
* Vintage Microphone:
    * No longer randomly triggered by Bottled Chaos
* Warning System:
    * Base Radius: ~~50m~~ ⇒ 75m
    * Charge Time: ~~30s~~ ⇒ 45s
    * Cooldown: ~~75s~~ ⇒ 90s
    * Siren Pole now passively charges itself instead of requiring players to stand inside the radius
* Marwan's Ash:
    * Burn effect can now be upgraded with the Ignition Tank item from the Survivors of the Void DLC
* Metronome:
    * Now plays 5 ticks in total instead of 4 to fit the soundtrack's 5/4 signature
* Fixed sounds not working
* Fixed Nuclear Accelerator becoming weaker when sprinting and stronger when unsprinting instead of the opposite
* Fixed Treasure Map radius containing a large glowing sphere
* Fixed Treasure Map and Rift Lens not dropping items
* Fixed content toggle config not disabling items
* Fixed Choc Chip extending item cooldown buffs and the visual debuff of the Void fog
* Removed Hiker's Backpack special skill fix option "BackpackEnableSkillFixes" from the general config
    * No longer necessary - all special skills work properly when stacked as of the 1st March 2022 update
* Fixed Scratch Ticket's "AlternateBonus" config option incorrectly multiplying chances by 100
* Fixed Rift Lens having infinite countdown
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
