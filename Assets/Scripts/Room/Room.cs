using System;
using System.Collections.Generic;
using Consistenant;
using DigitalRubyShared;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Habillage
{
    public class Room : MonoBehaviour, ISerializableData
    {
        public GameObject CameraPoint;
        public string HabitTitle;
        public TextMeshProUGUI RoomNameText;
        public ParticleSystem particleSystem;
        public GridData gridData;
        public int variantIndex;
        public bool IsLocked;
        public int RoomPrice = 100;

        public List<Material> VariantData = new();
        public List<Character> Characters = new();

        public UnityEvent<Room> OnTryUnlock;
        public UnityEvent OnLocked;
        public UnityEvent OnUnlocked;

        public MeshRenderer Renderer;
        public RoomLockUI LockUI;
        public RoomConfirmUI ConfirmUnlockUI;
        [SerializeField] GameObject RedAlert;
        [SerializeField] ColorGridChart colorGridChart;



        private void Start()
        {
            LockUI.PriceText.SetText(RoomPrice.ToString());
            ConfirmUnlockUI.ConfirmText.SetText($"Unlock this room for {RoomPrice}?");
            ConfirmUnlockUI.Price = RoomPrice;
            LockUI.gameObject.SetActive(IsLocked);
            colorGridChart.gameObject.SetActive(false);
            particleSystem.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (PlayerData.Data.CreatedHabits.TryGetValue(HabitTitle, out var _data))
            {
                if (_data.ScheduleData.ValidToday())
                {
                    if (_data.DaysData.TryGetValue(DateTime.Today.ToShortDateString(), out var _dayData))
                    {
                        RedAlert.SetActive(!_dayData.ResultData.Completed);
                    }
                    else
                    {
                        RedAlert.SetActive(true);

                    }
                }

                RoomNameText.rectTransform.anchoredPosition = new Vector2(RedAlert.activeSelf ? -0.5f : -0.75f, RoomNameText.rectTransform.anchoredPosition.y);
                RoomNameText.gameObject.SetActive(true);
                RoomNameText.text = _data.Title;
                RoomNameText.color = _data.Color.FromEnum();


                particleSystem.gameObject.SetActive(true);
                // Get the emission module
                var emission = particleSystem.emission;

                // Calculate the new constant min and max values based on your streak logic
                float newConstantMin = _data.CalculateStreak().BestStreak * .1f > 1 ? 1 : _data.CalculateStreak().BestStreak * .1f;
                float newConstantMax = _data.CalculateStreak().BestStreak * 3 > 30 ? 30 : _data.CalculateStreak().BestStreak * 3;

                // Set the new rate over time
                var rateOverTime = new ParticleSystem.MinMaxCurve(newConstantMin, newConstantMax);
                emission.rateOverTime = rateOverTime;

                var main = particleSystem.main;
                main.maxParticles = _data.CalculateStreak().BestStreak * 3 > 30 ? 30 : _data.CalculateStreak().BestStreak * 3;
            }
        }

        public void Lock(bool _value)
        {
            IsLocked = _value;

            LockUI.gameObject.SetActive(_value);

            if (_value)
            {
                OnLocked?.Invoke();
            }
            else
            {
                OnUnlocked?.Invoke();
            }
        }

        public void ResetTitle()
        {
            var _prevTitle = HabitTitle;
            HabitTitle = "";
            PlayerData.Data.CreatedHabits.Remove(_prevTitle);
            DormManager.Current.DeselectRoom();
            DormManager.Current.Save();
        }

        public void TryUnlock()
        {
            OnTryUnlock?.Invoke(this);
        }

        public void ToggleGridChart(bool _On)
        {
            colorGridChart.gameObject.SetActive(_On && !IsLocked);
            RoomNameText.gameObject.SetActive(!_On && !IsLocked && PlayerData.Data.CreatedHabits.TryGetValue(HabitTitle, out var _data));
        }

        public void NextVariant()
        {
            variantIndex = (variantIndex + 1) % VariantData.Count;
            ChangeVariant(variantIndex);
        }

        public void ChangeVariant(int _index)
        {
            variantIndex = _index;

            Renderer.material = VariantData[_index];
        }

        public Character AddCharacter(CharacterData _data)
        {
            var _newChar = Instantiate(_data.Prefab, gridData.Content);
            _newChar.Show(_data);
            _newChar.ID = IDGenerator.GenerateUniqueID();
            Characters.Add(_newChar);

            //PlayerData.Data.Inventory.AddCharacter(_data.Name);

            return _newChar;
        }

        public void RemoveCharacter(Character _character)
        {
            Characters.Remove(_character);

            //PlayerData.Data.Inventory.ActiveCharacter.Remove(_character.Data.Name);

            PlayerData.WriteSave();
            Destroy(_character.gameObject);
        }

        public JSONObject SerializeData()
        {
            var _json = new JSONObject();
            _json.Add("habit", HabitTitle);
            _json.Add("variant", variantIndex);
            _json.Add("lock", IsLocked);
            _json.Add("grid_data", gridData.SerializeData());
            var _charArr = new JSONObject();

            foreach (var _character in Characters)
            {
                _charArr.Add(_character.ID, _character.SerializeData());
            }

            _json.Add("characters", _charArr);

            return _json;
        }

        public void DeserializeData(JSONObject _json)
        {
            HabitTitle = _json["habit"];
            ChangeVariant(_json["variant"]);
            Lock(_json["lock"]);

            gridData.DeserializeData(_json["grid_data"].AsObject);

            foreach (var _charNode in _json["characters"])
            {
                var _id = _charNode.Key;
                var _data = _charNode.Value.AsObject;

                if (RuntimeData.CharacterDatabase.Data.TryGetValue(_data["key"].Value, out var _characterData))
                {
                    //Debug.Log($"Loaded {_data["key"].Value}");
                    var _newChar = Instantiate(_characterData.Prefab, gridData.Content);
                    _newChar.Show(_characterData);
                    Characters.Add(_newChar);
                    _newChar.DeserializeData(_data);

                }
            }
        }
    }
}
