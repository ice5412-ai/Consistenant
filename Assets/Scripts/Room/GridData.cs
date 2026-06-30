using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

namespace Habillage
{
    public class GridData : MonoBehaviour, ISerializableData
    {
        public Grid Grid;

        public Transform Content;
        //public Vector3Int maxSize = new(10, 4, 4);
        public Vector2Int XRange = new(-5, 5);
        public Vector2Int YRange = new(-2, 2);
        public Vector2Int ZRange = new(-2, 2);
        private Dictionary<Vector3Int, PlacementData> occupiedPosition = new();
        public Dictionary<string, Furniture> SpawnedObjects = new();
        public Dictionary<string, PlacementData> PlacedObjects = new();

        public bool PlaceObjectAt(Vector3Int gridPosition, Vector3Int size, string objectKey, string id)
        {
            id ??= IDGenerator.GenerateUniqueID();
            
            var posToOccupy = CalculatePositions(gridPosition, size);
            var data = new PlacementData(posToOccupy, id, objectKey);

            if (!CanPlaceObjectAt(gridPosition, size)) return false;
            
            foreach (var pos in posToOccupy)
            {
                occupiedPosition[pos] = data;
            }
            
            PlacedObjects.Add(id, data);

            return true;
        }

        public Furniture SpawnObject(FurnitureData data)
        {
            if (!data.Prefab) return null;

            if (!PlayerData.Data.Inventory.TryUseFurniture(data.Key, out data)) return null;

            var newObject = Instantiate(data.Prefab, Content, true);
            newObject.Spawned();
            //Spawn object at middle of the room
            newObject.transform.position = transform.position;

            var id = IDGenerator.GenerateUniqueID();
            newObject.ID = id;
            newObject.DestroyButton.onClick.AddListener(() => DeleteObject(id));
            SpawnedObjects.Add(newObject.ID, newObject);
            
            var gridCell = Grid.WorldToCell(newObject.transform.position);
            if (!PlaceObjectAt(gridCell, newObject.Data.Size, newObject.Data.Key, newObject.ID))
            {
                newObject.UpdateState(FurnitureState.Invalid);
            }

            return newObject;
        }
        
        public void DeleteObject(string id)
        {
            //Debug.Log("Delete");
            if (!SpawnedObjects.TryGetValue(id, out var furniture)) return;
            
            PlayerData.Data.Inventory.CollectFurniture(furniture.Data.Key);
            
            SpawnedObjects.Remove(id);
            RemoveObject(id);
            Destroy(furniture.gameObject);
        }
        
        public void RemoveObject(string id)
        {
            if (!PlacedObjects.TryGetValue(id, out var data)) return;
            
            foreach (var occupiedPos in data.OccupiedPositions)
            {
                //Debug.Log($"Removed {id} from {occupiedPos.ToString()}");
                occupiedPosition.Remove(occupiedPos);
            }

            PlacedObjects.Remove(id);
        }

        private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector3Int objectSize)
        {
            List<Vector3Int> returnVal = new();
            for (int x = 0; x < objectSize.x; x++)
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    for (int z = 0; z < objectSize.z; z++)
                    {
                        returnVal.Add(gridPosition + new Vector3Int(x, y, z));
                    }
                }
            }
            return returnVal;
        }
        
        public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector3Int objectSize)
        {
            List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
            foreach (var pos in positionToOccupy)
            {
                if (occupiedPosition.ContainsKey(pos))
                    return false;
                
                if (!InRange(pos.x, XRange) || !InRange(pos.y, YRange) || !InRange(pos.z, ZRange))
                {
                    return false;
                }
            }
            return true;
        }
        
        public void ValidateAllObjects()
        {
            foreach (var furniture in SpawnedObjects.Values)
            {
                if (furniture.State == FurnitureState.Invalid)
                {
                    var gridCell = Grid.WorldToCell(furniture.transform.position);
                    if (CanPlaceObjectAt(gridCell, furniture.Data.Size))
                    {
                        //PlaceObject
                        PlaceObjectAt(gridCell, furniture.Data.Size, furniture.Data.Key, furniture.ID);
                        
                        furniture.UpdateState(FurnitureState.Valid);
                    }
                }
            }
        }

        public void ClearInvalidObjects()
        {
            foreach (var furniture in SpawnedObjects.Values.ToArray())
            {
                if (furniture.State == FurnitureState.Invalid)
                {
                    DeleteObject(furniture.ID);
                }
            }
        }

        public void ClearAllObjects()
        {
            foreach (var furniture in SpawnedObjects.Values.ToArray())
            {
                DeleteObject(furniture.ID);
            }
        }

        public bool InRange(int value, Vector2Int range)
        {
            return (range.x <= value && value <= range.y);
        }

        public JSONObject SerializeData()
        {
            var json = new JSONObject();
            
            foreach (var kvp in SpawnedObjects)
            {
                if (kvp.Value.State == FurnitureState.Valid)
                {
                    json.Add(kvp.Key, kvp.Value.SerializeData());
                }
            }

            return json;
        }

        public void DeserializeData(JSONObject _json)
        {
            foreach (var furnitureNode in _json)
            {
                var id = furnitureNode.Key;
                var data = furnitureNode.Value.AsObject;
                if (RuntimeData.FurnitureDatabase.Data.TryGetValue(data["key"].Value, out var furnitureData))
                {
                    //Debug.Log($"Loaded {furnitureData.Key}");
                    var newObject = Instantiate(furnitureData.Prefab, Content, true);
                    newObject.DeserializeData(data);
                    newObject.DestroyButton.onClick.AddListener(() => DeleteObject(id));
                    var gridPos = Grid.WorldToCell(newObject.transform.position);
                    PlaceObjectAt(gridPos, newObject.Data.Size, newObject.Data.Key, newObject.ID);
                    SpawnedObjects.Add(newObject.ID, newObject);
                }
            }
        }
    }
}
