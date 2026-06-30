using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

namespace Habillage
{
    public class Character : MonoBehaviour, ISerializableObject
    {
        public Rigidbody Rigidbody;
        public CharacterData Data;
        public CharacterMovement CharacterMovement;
        public FingersDragDropComponentScript DragDrop;
        [SerializeField] private LayerMask objectLayerMask;
        
        public SpriteRenderer Renderer;

        public int variantIndex;

        public UnityEvent OnTap;

        public void Show(CharacterData data)
        {
            Data = data;
            Renderer.sprite = Data.Sprites[variantIndex];
        }

        private void OnEnable()
        {
            InputManager.Instance.TapGesture.StateUpdated += UpdateTap;
        }

        private void OnDisable()
        {
            InputManager.Instance.TapGesture.StateUpdated -= UpdateTap;
        }

        public void UpdateTap(GestureRecognizer _gesture)
        {
            if (_gesture.State != GestureRecognizerState.Ended)
            {
                return;
            }
            
            Camera camera = Camera.main;
            
            var ray = camera.ScreenPointToRay(new Vector3(_gesture.FocusX, _gesture.FocusY, 0.0f));

            if (Physics.Raycast(ray, out var hit, float.MaxValue, objectLayerMask))
            {
                if (hit.transform.GetComponentInParent<Character>()?.transform == transform)
                {
                    OnTap?.Invoke();
                }
            }
        }

        public void BeginDrag()
        {
            CharacterMovement.enabled = false;
        }

        public void Drag()
        {
            
        }

        public void EndDrag()
        {
            CharacterMovement.enabled = true;
        }
        
        public void ChangeVariant()
        {
            variantIndex = (variantIndex + 1) % Data.Sprites.Count;
            Renderer.sprite = Data.Sprites[variantIndex];
        }

        public JSONObject SerializeData()
        {
            var json = new JSONObject();
            json.Add("id", ID);
            json.Add("key", Data.Name);
            json.Add("variant", variantIndex);

            return json;
        }

        public void DeserializeData(JSONObject _json)
        {
            ID = _json["id"];
            variantIndex = _json["variant"];
            Renderer.sprite = Data.Sprites[variantIndex];
        }

        public string ID { get; set; }
    }
}
