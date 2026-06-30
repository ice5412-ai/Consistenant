using System.Collections.Generic;

namespace Habillage
{
    public class RuntimeFurnitureDatabase
    {
        public Dictionary<string, FurnitureData> Data = new();

        public RuntimeFurnitureDatabase(FurnitureDatabase data)
        {
            foreach (var furnitureData in data.GetData())
            {
                Data[furnitureData.Key] = furnitureData;
            }
        }
    }
}