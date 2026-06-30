using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Habillage
{
    [CreateAssetMenu(menuName = "Data/Furniture Database")]
    public class FurnitureDatabase : ScriptableObject
    {
        [SerializeField] private List<FurnitureData> Data = new();
        public SerializedDictionary<string, FurnitureData> FurnitureDict;

        public IEnumerable<FurnitureData> GetData()
        {
            return Data.Where(data => data && data.Prefab);
        }
    }
}
