using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using JimmysUnityUtilities;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Habillage
{
    public class DormManager : MonoBehaviour, ISerializableData
    {
        public static DormManager Current;
        public CameraController CameraController;
        public Room SelectedRoom;
        public LayerMask RoomLayer;
        public PlacementSystem PlacementSystem;
        public GameObject WarningPanel;

        public UnityEvent OnDoubleTapRoom;
        public UnityEvent<GameObject> OnSelectedRoom;
        public UnityEvent OnDeselectRoom;
        public UnityEvent OnEnterEditRoom;
        public UnityEvent OnExitEditRoom;
        public UnityEvent<Character> OnAddedCharacter;
        public UnityEvent<Room> OpenHabitCreation;
        public List<GameObject> CloseWhenSelect;

        public List<Room> AllRooms = new();

        private void Awake()
        {
            Current = this;
        }

        private void OnDestroy()
        {
            Current = null;
        }

        private void Start()
        {
            Load();
        }

        private void OnEnable()
        {
            InputManager.Instance.TapGesture.StateUpdated += TapGestureUpdated;
            InputManager.Instance.DoubleTapGesture.StateUpdated += DoubleTapGestureUpdated;
            SyncRoom();
        }

        public void SyncRoom()
        {
            for (var _index = 0; _index < AllRooms.Count; _index++)
            {
                var room = AllRooms[_index];
                room.gameObject.SetActive(PlayerData.Data.UnlockedRoom + 1 > _index);
                room.Lock(PlayerData.Data.UnlockedRoom <= _index);
            }
        }

        private void OnDisable()
        {
            InputManager.Instance.TapGesture.StateUpdated -= TapGestureUpdated;
            InputManager.Instance.DoubleTapGesture.StateUpdated -= DoubleTapGestureUpdated;
        }

        private void Update()
        {
#if UNITY_STANDALONE_WIN
            
            if (!InputManager.IsOverUI)
            {
                //Debug.Log("click");
                var ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
                if (Physics.Raycast(ray, out var hit, float.MaxValue, RoomLayer))
                {
                    //Debug.Log(hit.collider.name);
                    if (Input.GetMouseButtonUp(0))
                    {
                        //Debug.Log(Input.GetMouseButtonUp(0));
                        if (hit.transform.TryGetComponent(out Room room))
                        {
                            SelectRoom(room);
                            //Debug.Log("Room selected: " + room.name);
                        }
                    }
                }
            }
            /*else
            {
                // Debugging which UI element is being pointed at
                Debug.Log("Pointer is over a UI element.");
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, results);
                foreach (var result in results)
                {
                    Debug.Log("UI Element: " + result.gameObject.name);
                }
            }*/
#endif
        }

        private void TapGestureUpdated(GestureRecognizer gesture)
        {
            if (Input.touchCount <= 0) return;
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

            //Show Edit button after selected room
            if (gesture.State != GestureRecognizerState.Ended) return;
            var ray = Camera.main.ScreenPointToRay(new Vector3(gesture.FocusX, gesture.FocusY, 0.0f));
            if (Physics.Raycast(ray, out var hit, float.MaxValue, RoomLayer))
            {
                if (hit.transform.TryGetComponent(out Room room))
                {
                    if (gesture is TapGestureRecognizer _tapGesture)
                    {
                        SelectRoom(room);
                        foreach (GameObject toClose in CloseWhenSelect)
                        {
                            toClose.SetActive(false);
                        }
                    }
                }
            }
        }

        private void DoubleTapGestureUpdated(GestureRecognizer gesture)
        {
            if (Input.touchCount <= 0) return;
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

            if (gesture.State != GestureRecognizerState.Ended) return;

            var ray = Camera.main.ScreenPointToRay(new Vector3(gesture.FocusX, gesture.FocusY, 0.0f));
            if (Physics.Raycast(ray, out var hit, float.MaxValue, RoomLayer))
            {
                if (hit.transform.TryGetComponent(out Room room))
                {
                    if (room == SelectedRoom)
                    {
                        OnDoubleTapRoom?.Invoke();
                    }
                }
            }
        }


        public void DeselectRoom()
        {
            SelectRoom(null);
        }

        public void SelectRoom(Room newRoom)
        {
            if (newRoom && newRoom.IsLocked) return;

            if (newRoom && newRoom.HabitTitle.IsNullOrWhiteSpace())
            {
                OpenHabitCreation?.Invoke(newRoom);
                return;
            }

            if (SelectedRoom == newRoom) return;

            SelectedRoom = newRoom;

            if (SelectedRoom)
            {
                PlayerData.Data.SelectHabit(newRoom);
                CameraController.Focus(newRoom.CameraPoint);
                OnSelectedRoom?.Invoke(newRoom.gameObject);
            }
            else
            {
                PlayerData.Data.DeselectHabit();
                CameraController.UnFocus();
                OnDeselectRoom?.Invoke();
            }
        }

        public void RemoveRoomHabit()
        {
            if (SelectedRoom)
            {
                SelectedRoom.ResetTitle();
                DeselectRoom();
            }
        }
        
        public void ChangeRoomVariant()
        {
            if (SelectedRoom)
            {
                SelectedRoom.NextVariant();
            }
        }

        public void TryConfirmEdit()
        {
            if (PlacementSystem.HaveInvalidObjects())
            {
                WarningPanel.SetActive(true);
            }
            else
            {
                EditRoom(false);
            }
        }

        public void EditRoom(bool value)
        {
            PlacementSystem.enabled = value;
            WarningPanel.SetActive(false);
            if (value)
            {
                foreach (var room in AllRooms)
                {
                    room.gameObject.SetActive(room == SelectedRoom);
                }
                OnEnterEditRoom?.Invoke();
            }
            else
            {
                foreach (var room in AllRooms)
                {
                    room.gameObject.SetActive(true);
                }
                OnExitEditRoom?.Invoke();
                Save();
            }
        }

        public void AddCharacter(CharacterData _data)
        {
            if (!SelectedRoom) return;
            if (!PlayerData.Data.Inventory.TrySpawnCharacter(_data.Name, out _data)) return;

            var _newChar = SelectedRoom.AddCharacter(_data);
            OnAddedCharacter?.Invoke(_newChar);
            Save();
        }

        public void RemoveCharacter(Character _character)
        {
            if (SelectedRoom)
            {
                SelectedRoom.RemoveCharacter(_character);
                PlayerData.Data.Inventory.CollectCharacter(_character.Data.Name);
                Save();
            }
        }

        public void TryUnlockRoom(Room _room)
        {
            if (PlayerData.Data.Inventory.TrySpendMoney(_room.RoomPrice))
            {
                UnlockRoom(_room);
            }
            else
            {
            }
        }

        public void UnlockRoom(Room _room)
        {
            _room.Lock(false);
            PlayerData.Data.UnlockedRoom++;
            Save();
            SyncRoom();
        }

        public void Save()
        {
            PlayerData.Data.DormData = SerializeData();
            PlayerData.WriteSave();
            Debug.Log("Saved Dorm");
        }

        public void Load()
        {
            DeserializeData(PlayerData.Data.DormData);
            Debug.Log("Loaded Dorm");
        }

        public JSONObject SerializeData()
        {
            var json = new JSONObject();

            var roomArray = new JSONArray();
            foreach (var room in AllRooms)
            {
                roomArray.Add(room.SerializeData());
            }

            json.Add("rooms_data", roomArray);
            return json;
        }

        public void DeserializeData(JSONObject _json)
        {
            if (_json.HasKey("rooms_data"))
            {
                var roomArray = _json["rooms_data"].AsArray;
                for (var index = 0; index < roomArray.Count; index++)
                {
                    var roomNode = roomArray[index];
                    AllRooms[index].DeserializeData(roomNode.AsObject);
                }
            }
        }
    }
}
