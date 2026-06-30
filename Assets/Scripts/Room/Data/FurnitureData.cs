using UnityEngine;
using Consistenant;

namespace Habillage
{
    
    [CreateAssetMenu(menuName = "Data/Furniture Data", fileName = "FurnitureData")]
    public class FurnitureData : ScriptableObject
    {
        public string Key;
        public int Price;
        public Sprite Icon;
        public Vector3Int Size = Vector3Int.one;
        public Furniture Prefab;
    }
}
