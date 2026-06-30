using System.Collections.Generic;
using UnityEngine;

namespace Habillage
{
    public class PlacementData
    {
        public List<Vector3Int> OccupiedPositions;
        public string ID;
        public string ObjectID;
        
        public PlacementData(List<Vector3Int> occupiedPositions, string id, string objectID)
        {
            OccupiedPositions = occupiedPositions;
            ID = id;
            ObjectID = objectID;
        }
    }
}