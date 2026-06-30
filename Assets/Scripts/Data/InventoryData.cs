using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace Habillage
{
    public class InventoryData : ISerializableData
    {
        //public Action<string> OnAddedItem;
        //public Action<string> OnRemovedItem;
        public Action OnInventoryChanged;
        public Action OnCharacterChanged;
        public Action<int> OnMoneyChanged;
        public Action<float> OnScoreChanged;
        public Dictionary<string, int> Furniture = new();
        public Dictionary<string, int> UsedFurniture = new();
        public Dictionary<string, int> Character = new();
        public Dictionary<string, int> ActiveCharacter = new();
        public int TotalSpawnedCharacter { get; private set; } = 0;

        public int Money { get; private set; } = 15000;
        public float TodayScore = 0;

        public void AddMoney(int _value)
        {
            Money += _value;
            OnMoneyChanged?.Invoke(_value);

            Debug.Log($"Added Money {_value}");
            PlayerData.WriteSave();
        }

        public void ChangeTodayScore(float _value)
        {
            TodayScore = _value;
            OnScoreChanged?.Invoke(_value);

            Debug.Log($"Today Score changed to {_value}");
            PlayerData.WriteSave();
        }

        public bool TrySpendMoney(int _value)
        {
            if (Money >= _value)
            {
                Money -= _value;
                OnMoneyChanged?.Invoke(-_value);
                PlayerData.WriteSave();
                return true;
            }

            return false;
        }

        public void AddCharacter(string _id)
        {
            if (!Character.ContainsKey(_id))
            {
                Character.Add(_id, 0);
            }

            Character[_id] += 1;

            if (!ActiveCharacter.ContainsKey(_id))
            {
                ActiveCharacter.Add(_id, 0);
            }

            Debug.Log($"Added {_id} to inventory");
            OnCharacterChanged?.Invoke();
            PlayerData.WriteSave();
        }

        public void RemoveCharacter(string _id)
        {
            Character[_id] -= 1;
            if (Character[_id] <= 0)
            {
                Character.Remove(_id);
                ActiveCharacter.Remove(_id);
            }
            OnCharacterChanged?.Invoke();
        }

        public void AddFurniture(string _id)
        {
            if (!Furniture.ContainsKey(_id))
            {
                Furniture.Add(_id, 0);
            }

            Furniture[_id] += 1;

            if (!UsedFurniture.ContainsKey(_id))
            {
                UsedFurniture.Add(_id, 0);
            }

            Debug.Log($"Added {_id} to inventory");
            OnInventoryChanged?.Invoke();
            PlayerData.WriteSave();
        }

        public void RemoveFurniture(string _id)
        {
            Furniture[_id] -= 1;
            if (Furniture[_id] <= 0)
            {
                Furniture.Remove(_id);
                UsedFurniture.Remove(_id);
            }
            OnInventoryChanged?.Invoke();
        }

        public bool TryUseFurniture(string _id, out FurnitureData _data)
        {
            RuntimeData.FurnitureDatabase.Data.TryGetValue(_id, out _data);

            if (!Furniture.TryGetValue(_id, out var _value)) return false;

            if (_value <= 0) return false;

            Furniture[_id] -= 1;
            UsedFurniture[_id] += 1;

            OnInventoryChanged?.Invoke();
            PlayerData.WriteSave();
            return true;
        }

        public bool TrySpawnCharacter(string _id, out CharacterData _data)
        {
            RuntimeData.CharacterDatabase.Data.TryGetValue(_id, out _data);

            if (!Character.TryGetValue(_id, out var _value)) return false;
            Debug.Log(_value);
            if (_value <= 0) return false;

            Character[_id] -= 1;
            ActiveCharacter[_id] += 1;
            TotalSpawnedCharacter += 1;

            OnCharacterChanged?.Invoke();
            PlayerData.WriteSave();
            return true;
        }

        public void CollectCharacter(string _id)
        {
            ActiveCharacter[_id] -= 1;
            Character[_id] += 1;
            TotalSpawnedCharacter -= 1;
            OnInventoryChanged?.Invoke();
        }

        public void CollectFurniture(string _id)
        {
            UsedFurniture[_id] -= 1;
            Furniture[_id] += 1;
            OnInventoryChanged?.Invoke();
        }

        public JSONObject SerializeData()
        {
            var _json = new JSONObject();

            _json.Add("money", Money);
            _json.Add("todayscore", TodayScore);
            _json.Add("spawnedchar", TotalSpawnedCharacter);

            var _furnitureNode = new JSONObject();
            foreach (var _kvp in Furniture)
            {
                _furnitureNode.Add(_kvp.Key, _kvp.Value);
            }

            _json.Add("furni", _furnitureNode);

            var _usedFurnitureNode = new JSONObject();
            foreach (var _kvp in UsedFurniture)
            {
                _usedFurnitureNode.Add(_kvp.Key, _kvp.Value);
            }

            _json.Add("used_furni", _usedFurnitureNode);

            var _charNode = new JSONObject();
            foreach (var _kvp in Character)
            {
                _charNode.Add(_kvp.Key, _kvp.Value);
            }
            _json.Add("char", _charNode);

            var _activeCharNode = new JSONObject();
            foreach (var _kvp in ActiveCharacter)
            {
                _activeCharNode.Add(_kvp.Key, _kvp.Value);
            }
            _json.Add("used_char", _activeCharNode);

            return _json;
        }

        public void DeserializeData(JSONObject _json)
        {
            Money = _json["money"];
            TodayScore = _json["todayscore"];
            TotalSpawnedCharacter = _json["spawnedchar"];

            Furniture = new Dictionary<string, int>();
            var _furnitureNode = _json["furni"];
            foreach (var _kvp in _furnitureNode)
            {
                Furniture.Add(_kvp.Key, _kvp.Value);
            }

            UsedFurniture = new Dictionary<string, int>();
            var _usedFurnitureNode = _json["used_furni"];
            foreach (var _kvp in _usedFurnitureNode)
            {
                UsedFurniture.Add(_kvp.Key, _kvp.Value);
            }

            Character = new Dictionary<string, int>();
            var _character = _json["char"];
            foreach (var _kvp in _character)
            {
                Character.Add(_kvp.Key, _kvp.Value);
            }

            ActiveCharacter = new Dictionary<string, int>();
            foreach (var _kvp in _json["used_char"])
            {
                //Debug.Log(_kvp.Key);
                ActiveCharacter.Add(_kvp.Key, _kvp.Value);
            }
        }
    }
}