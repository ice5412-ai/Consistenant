using System.Collections;
using System.Collections.Generic;
using Habillage;
using UnityEngine;

namespace Consistenant
{
    [CreateAssetMenu(menuName = "Data/Gacha Data", fileName = "GachaData")]
    public class Gacha : ScriptableObject
    {
        public string gachaName;
        public FurnitureData furnitureData;
        public CharacterData characterData;
        public int dropChance;
        public int rarity;

        public Gacha(string gachaName, FurnitureData furnitureData, CharacterData characterData, int dropChance, int rarity)
        {
            this.gachaName = gachaName;
            this.furnitureData = furnitureData;
            this.characterData = characterData;
            this.dropChance = dropChance;
            this.rarity = rarity;
        }
    }
}
