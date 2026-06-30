using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Habillage
{
    public class Furniture : MonoBehaviour, ISerializableObject
    {
        public Canvas Canvas;
        public FurnitureData Data;
        public Button DestroyButton;
        public FurnitureState State;
        public UnityEvent OnSpawned;
        public UnityEvent OnSelected;
        public UnityEvent OnDeselected;
        public UnityEvent OnValid;
        public UnityEvent OnInvalid;

        [Header("Debug")] 
        [SerializeField] private bool isDebug;

        [SerializeField] private float pivotRadius = 0.1f;

        private void Start()
        {
            Canvas.gameObject.SetActive(false);
        }

        public void Spawned()
        {
            OnSpawned?.Invoke();
        }

        public void UpdateState(FurnitureState newState)
        {
            if (State == newState)
                return;

            State = newState;
            
            switch (newState)
            {
                case FurnitureState.Valid:
                    OnValid?.Invoke();
                    break;
                case FurnitureState.Invalid:
                    OnInvalid?.Invoke();
                    break;
            }
        }
        
        public void Select(bool value)
        {
            if (value)
            {
                OnSelected?.Invoke();
            }
            else
            {
                OnDeselected?.Invoke();
            }
        }
    
        private void OnDrawGizmos()
        {
            if (!isDebug) return;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, pivotRadius);
        }

        public JSONObject SerializeData()
        {
            var json = new JSONObject();
            json.Add("id", ID);
            json.Add("key", Data.Key);
            json.Add("pos", transform.position);

            return json;
        }

        public void DeserializeData(JSONObject _json)
        {
            ID = _json["id"];
            transform.position = _json["pos"];
        }

        public string ID { get; set; }
    }
}
