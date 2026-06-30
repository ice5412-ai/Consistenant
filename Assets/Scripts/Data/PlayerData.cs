
using System;
using System.Collections.Generic;
using System.IO;
using Consistenant;
using SimpleJSON;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace Habillage
{
    public class PlayerData : ISerializableData
    {
        public static PlayerData Data;

        #region ReadWrite

        private const string SaveFileName = "save.json";
        private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        public static PlayerData ReadSave()
        {
            Data = new PlayerData();

            if (!File.Exists(SavePath))
            {
                Debug.Log("No save data.");
                return Data;
            }

            var _text = File.ReadAllText(SavePath);

            Debug.Log($"ReadSave {SavePath}");

            var _dataJson = JSON.Parse(_text);

            Data.DeserializeData(_dataJson.AsObject);

            return Data;
        }

        public static void WriteSave()
        {
            File.WriteAllText(SavePath, Data.SerializeData().ToString());
            Debug.Log($"Saved to {SavePath}");
            Debug.Log(Data.SerializeData().ToString());
        }

        public static void ClearSave()
        {
            Data = new PlayerData();
        }

        #endregion

        public HabitData CurrentHabit;
        public Room CurrentRoom;

        public InventoryData Inventory = new();
        public DateTime NotedDay = DateTime.Today;
        public DateTime NextGenerate = DateTime.Now;
        public int Daily_Rewards = 0;
        public bool DRtaken = false;
        public JSONObject DormData = new();

        public Dictionary<string, HabitData> CreatedHabits = new();
        public int Guarantee;
        public int UnlockedRoom;

        public void AddHabit(HabitData _habit)
        {
            CreatedHabits.TryAdd(_habit.Title, _habit);
        }

        public void UpdateGuarantee(int _value)
        {
            Guarantee += _value;
            WriteSave();
        }

        public void SelectHabit(Room _room)
        {
            CurrentRoom = _room;
            CreatedHabits.TryGetValue(_room.HabitTitle, out CurrentHabit);
        }

        public void DeselectHabit()
        {
            CurrentHabit = null;
            CurrentRoom = null;
        }

        public JSONObject SerializeData()
        {
            var _json = new JSONObject();
            _json.Add("inventory", Inventory.SerializeData());
            _json.Add("unlocked", UnlockedRoom);

            var _habitNode = new JSONObject();

            foreach (var _kvp in CreatedHabits)
            {
                _habitNode.Add(_kvp.Key, _kvp.Value.SerializeData());
            }
            _json.Add("habit", _habitNode);

            _json.Add("dorm", DormData);
            _json.Add("guarantee", Guarantee);
            _json.Add("noted", NotedDay);
            _json.Add("gen", NextGenerate);
            _json.Add("DR", Daily_Rewards);
            _json.Add("DRt", DRtaken);
            return _json;
        }

        public void DeserializeData(JSONObject _json)
        {
            Inventory.DeserializeData(_json["inventory"].AsObject);

            if (_json.HasKey("unlocked"))
                UnlockedRoom = _json["unlocked"];

            CreatedHabits = new Dictionary<string, HabitData>();
            foreach (var _habitNode in _json["habit"])
            {
                var _habit = new HabitData();
                _habit.DeserializeData(_habitNode.Value.AsObject);
                CreatedHabits.Add(_habitNode.Key, _habit);
            }

            DormData = _json["dorm"].AsObject;
            Guarantee = _json["guarantee"];
            NotedDay = _json["noted"];
            NextGenerate = _json["gen"];
            Daily_Rewards = _json["DR"];
            DRtaken = _json["DRt"];
        }
    }
}

