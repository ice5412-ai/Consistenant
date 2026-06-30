using System.Collections.Generic;
using UnityEngine;

namespace Habillage
{
    [CreateAssetMenu(menuName = "Data/Character Database")]
    public class CharacterDatabase : ScriptableObject
    {
        public List<CharacterData> Data = new();
    }
}