using UnityEngine;

namespace Habillage
{
    public static class RuntimeData
    {
        public static RuntimeFurnitureDatabase FurnitureDatabase;
        public static RuntimeCharacterDatabase CharacterDatabase;
        public static ColorDatabase ColorDatabase;

        public static void Initialize()
        {
            var furnitureDatabase = Resources.Load<FurnitureDatabase>("FurnitureDatabase");
            FurnitureDatabase = new RuntimeFurnitureDatabase(furnitureDatabase);

            var characterDatabase = Resources.Load<CharacterDatabase>("CharacterDatabase");
            CharacterDatabase = new RuntimeCharacterDatabase(characterDatabase);

            ColorDatabase = Resources.Load<ColorDatabase>("ColorData");
            Debug.Log("Initialized runtime data");
        }
    }
}