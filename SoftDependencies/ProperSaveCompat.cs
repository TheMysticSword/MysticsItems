using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MysticsItems.SoftDependencies
{
    internal static class ProperSaveCompat
    {
        internal static void Init()
        {
            ProperSave.SaveFile.OnGatherSaveData += SaveFile_OnGatherSaveData;
            ProperSave.Loading.OnLoadingEnded += Loading_OnLoadingEnded;
        }

        public class MysticsItemsSaveData
        {
            [DataMember(Name = "p")]
            public List<MysticsItemsPlayerSaveData> players { get; set; }

            public class MysticsItemsPlayerSaveData
            {
                [DataMember(Name = "nidv")]
                public ulong NetworkIdValue { get; set; }

                [DataMember(Name = "nidsv")]
                public string NetworkIdStrValue { get; set; }

                [DataMember(Name = "nidsi")]
                public byte NetworkIdSubId { get; set; }

                [DataMember(Name = "msct")]
                public Dictionary<string, int> ManuscriptBuffs { get; set; }

                [DataMember(Name = "msb")]
                public float MysticSwordBonus { get; set; }

                [DataMember(Name = "lah")]
                public int LimitedArmorHits { get; set; }
            }

            internal MysticsItemsSaveData()
            {
                players = new List<MysticsItemsPlayerSaveData>();
                foreach (var pcmc in PlayerCharacterMasterController.instances)
                {
                    if (!pcmc.networkUser) continue;

                    var playerData = new MysticsItemsPlayerSaveData();
                    players.Add(playerData);
                    playerData.NetworkIdValue = pcmc.networkUser.Network_id.value;
                    playerData.NetworkIdStrValue = pcmc.networkUser.Network_id.strValue;
                    playerData.NetworkIdSubId = pcmc.networkUser.Network_id.subId;

                    var master = pcmc.master;
                    if (master && master.inventory)
                    {
                        var inventory = master.inventory;

                        var manuscriptComponent = inventory.GetComponent<Items.Manuscript.MysticsItemsManuscript>();
                        if (manuscriptComponent)
                        {
                            playerData.ManuscriptBuffs = new Dictionary<string, int>();
                            foreach (var buff in manuscriptComponent.buffOrder) playerData.ManuscriptBuffs.Add(buff.ToString(System.Globalization.CultureInfo.InvariantCulture), manuscriptComponent.buffStacks[buff]);
                        }

                        var mysticSwordComponent = inventory.GetComponent<Items.MysticSword.MysticsItemsMysticSwordBehaviour>();
                        if (mysticSwordComponent)
                        {
                            playerData.MysticSwordBonus = mysticSwordComponent.damageBonus;
                        }

                        var limitedArmorComponent = master.GetComponent<Items.LimitedArmor.MysticsItemsLimitedArmorBehavior>();
                        if (limitedArmorComponent)
                        {
                            playerData.LimitedArmorHits = limitedArmorComponent.GetTotalStock();
                        }
                    }
                }
            }

            internal void Load()
            {
                foreach (var user in NetworkUser.readOnlyInstancesList)
                {
                    var playerInfo = players.FirstOrDefault(x => x.NetworkIdValue == user.Network_id.value && x.NetworkIdStrValue == user.Network_id.strValue && x.NetworkIdSubId == user.Network_id.subId);
                    if (playerInfo.Equals(default)) continue;

                    var master = user.master;
                    if (master && master.inventory)
                    {
                        var inventory = master.inventory;

                        var manuscriptComponent = inventory.GetComponent<Items.Manuscript.MysticsItemsManuscript>();
                        if (!manuscriptComponent) manuscriptComponent = inventory.gameObject.AddComponent<Items.Manuscript.MysticsItemsManuscript>();
                        foreach (var buffNameAndCount in playerInfo.ManuscriptBuffs)
                        {
                            for (var i = 0; i < buffNameAndCount.Value; i++)
                                manuscriptComponent.AddBuff((Items.Manuscript.MysticsItemsManuscript.BuffType)Enum.Parse(typeof(Items.Manuscript.MysticsItemsManuscript.BuffType), buffNameAndCount.Key));
                        }

                        var mysticSwordComponent = inventory.GetComponent<Items.MysticSword.MysticsItemsMysticSwordBehaviour>();
                        if (!mysticSwordComponent) mysticSwordComponent = inventory.gameObject.AddComponent<Items.MysticSword.MysticsItemsMysticSwordBehaviour>();
                        mysticSwordComponent.damageBonus = playerInfo.MysticSwordBonus;

                        var limitedArmorComponent = master.GetComponent<Items.LimitedArmor.MysticsItemsLimitedArmorBehavior>();
                        if (!limitedArmorComponent) limitedArmorComponent = master.gameObject.AddComponent<Items.LimitedArmor.MysticsItemsLimitedArmorBehavior>();
                        var fullBows = UnityEngine.Mathf.FloorToInt(playerInfo.LimitedArmorHits / Items.LimitedArmor.hits);
                        limitedArmorComponent.stockHolders.Clear();
                        limitedArmorComponent.oldItemCount = fullBows;
                        for (var i = 0; i < fullBows; i++) limitedArmorComponent.AddStock();
                        limitedArmorComponent.stockHolders.Add(playerInfo.LimitedArmorHits - fullBows * Items.LimitedArmor.hits);
                    }
                }
            }
        }

        private static void SaveFile_OnGatherSaveData(Dictionary<string, object> obj)
        {
            obj.Add("MysticsItems_SaveData", new MysticsItemsSaveData());
        }

        private static void Loading_OnLoadingEnded(ProperSave.SaveFile obj)
        {
            var data = obj.GetModdedData<MysticsItemsSaveData>("MysticsItems_SaveData");
            if (data != null) data.Load();
        }
    }
}
