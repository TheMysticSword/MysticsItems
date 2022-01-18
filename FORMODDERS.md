# Adding Mystic's Items support for your own mod
First of all, add Mystic's Items as a soft dependency to your mod. Use MysticsItems.MysticsItemsPlugin.PluginGUID as the dependency GUID.  

### Contraband Gunpowder
Call `MysticsItems.OtherModCompat.ExplosivePickups_TryExplode(CharacterBody body)` when your custom pickup gets picked up.  
It will trigger an explosion if the body has the item.

### Failed Experiment
Call  
`MysticsItems.OtherModCompat.ElitePotion_AddSpreadEffect(BuffDef eliteBuffDef, GameObject vfx, BuffDef debuff, DotController.DotIndex dot, float damage, float procCoefficient)`  
to add info about the effect that you want to spread when your elite dies.  
Not all arguments are necessary for this - for example, if you want the spread effect to only inflict a debuff with a visual effect, use this:  
  
```
ElitePotion_AddSpreadEffect(
    eliteBuffDef: <your elite's buffdef>,
    vfx: <your explosion effect prefab>,
    debuff: <your debuff>
);
```
