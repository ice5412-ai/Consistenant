using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consistenant;

namespace Habillage
{
    [CreateAssetMenu(menuName = "Data/Character Data")]
    public class CharacterData : ScriptableObject
    {
        public string Name;
        public List<Sprite> Sprites = new();
        public Character Prefab;
    }
}
