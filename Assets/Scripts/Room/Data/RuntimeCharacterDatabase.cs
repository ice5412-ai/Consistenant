using System.Collections.Generic;
using UnityEngine;

namespace Habillage
{
    public class RuntimeCharacterDatabase
    {
        public Dictionary<string, CharacterData> Data = new();

        public RuntimeCharacterDatabase(CharacterDatabase database)
        {
            foreach (var data in database.Data)
            {
                Data[data.Name] = data;
            }
        }
    }
}