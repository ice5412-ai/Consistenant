using System;
using System.Collections.Generic;
using DigitalRubyShared;
using JimmysUnityUtilities;
using TriInspector;
using UnityEngine;

namespace Habillage
{
    public class PlacementSystem : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        private GridData currentGrid => DormManager.SelectedRoom?.gridData;

        public DormManager DormManager;
        [SerializeField] private Furniture selectedObject;

        [Header("ObjectSelection")]
        [SerializeField] private LayerMask objectLayerMask;
        //[SerializeField] private float selectScale = 1.1f;

        [Header("Debug")] 
        [SerializeField] private bool isDebug;

        [SerializeField, ShowIf(nameof(isDebug))]
        private GameObject pointerIndicator;
        
        private InputManager InputManager => InputManager.Instance;
        
        private void OnEnable()
        {
            camera = Camera.main;
            InputManager.PanGesture.StateUpdated += PanGestureUpdated;
            InputManager.LongPressGesture.StateUpdated += PressGestureUpdated;
            InputManager.TapGesture.StateUpdated += TapGestureUpdated;
        }

        private void OnDisable()
        {
            Deselect();

            if (currentGrid)
            {
                currentGrid.ClearInvalidObjects();
            }
            
            InputManager.PanGesture.StateUpdated -= PanGestureUpdated;
            InputManager.LongPressGesture.StateUpdated -= PressGestureUpdated;
            InputManager.TapGesture.StateUpdated -= TapGestureUpdated;
        }

        public void Select(Furniture furniture)
        {
            Deselect();
            
            selectedObject = furniture;
            selectedObject.Select(true);
            //selectedObject.transform.localScale *= selectScale;
        }

        public void Deselect()
        {
            //Debug.Log("Deselect");
            if (selectedObject)
            {
                selectedObject.Select(false);
                //selectedObject.transform.localScale /= selectScale;
            }

            selectedObject = null;
        }

        public void ClearAllObjects()
        {
            if (currentGrid)
            {
                currentGrid.ClearAllObjects();
                Debug.Log("Cleared room");
            }
        }
        
        public void SpawnObject(FurnitureData data)
        {
            if (!currentGrid)
            {
                Debug.Log("No selected grid");
                return;
            }
            
            var newObject = currentGrid.SpawnObject(data);
            if (newObject)
            {
                Select(newObject);
            }

        }

        public void DeleteObject(string id)
        {
            currentGrid.DeleteObject(id);

            ValidateAllObjects();
        }

        private void PanGestureUpdated(GestureRecognizer gesture)
        {
            switch (gesture.State)
            {
                //Update pointer pos
                case GestureRecognizerState.Executing:
                    var inputScreenPoint = new Vector3(gesture.FocusX, gesture.FocusY);
                    var inputPos = InputManager.GetSelectedMapPosition(inputScreenPoint);
                    var gridCell = currentGrid.Grid.WorldToCell(inputPos);
                    var gridPos = currentGrid.Grid.CellToWorld(gridCell);
                    if (isDebug)
                    {
                        if (pointerIndicator)
                            pointerIndicator.transform.position = gridPos;
                    }

                    break;

                case GestureRecognizerState.Ended:

                    break;
            }
        }

        private void PressGestureUpdated(GestureRecognizer gesture)
        {
            switch (gesture.State)
            {
                case GestureRecognizerState.Began:
                    var ray = camera.ScreenPointToRay(new Vector3(gesture.FocusX, gesture.FocusY,
                        0.0f));
                    if (Physics.Raycast(ray, out var hit, float.MaxValue, objectLayerMask))
                    {
                        if (hit.transform.TryGetComponentInParent<Furniture>(out var furniture))
                        {
                            if (selectedObject != furniture)
                            {
                                Select(furniture);
                            }
                        }
                        else
                        {
                            Deselect();
                        }
                    }

                    break;
                case GestureRecognizerState.Executing:
                    var inputScreenPoint = new Vector3(gesture.FocusX, gesture.FocusY);
                    var inputPos = InputManager.GetSelectedMapPosition(inputScreenPoint);
                    var gridCell = currentGrid.Grid.WorldToCell(inputPos);
                    var gridPos = currentGrid.Grid.CellToWorld(gridCell);

                    if (selectedObject)
                    {
                        selectedObject.transform.position = gridPos;
                        
                        currentGrid.RemoveObject(selectedObject.ID);
                        
                        if (currentGrid.CanPlaceObjectAt(gridCell, selectedObject.Data.Size))
                        {
                            //PlaceObject
                            currentGrid.PlaceObjectAt(gridCell, selectedObject.Data.Size, selectedObject.Data.Key,
                                selectedObject.ID);
                            
                            selectedObject.UpdateState(FurnitureState.Valid);
                            //Debug.Log($"Placed {selectedObject.name} at {gridCell}");
                        }
                        else
                        {
                            //Show can't place
                            selectedObject.UpdateState(FurnitureState.Invalid);
                            //Debug.Log($"Can't place {selectedObject.name} at {gridCell}");
                        }
                    }

                    break;
                case GestureRecognizerState.Ended:
                case GestureRecognizerState.Failed:
                    ValidateAllObjects();
                    break;
            }
        }

        public void ValidateAllObjects()
        {
            currentGrid?.ValidateAllObjects();
        }

        public bool HaveInvalidObjects()
        {
            if (currentGrid)
            {
                foreach (var furniture in currentGrid.SpawnedObjects.Values)
                {
                    if (furniture.State == FurnitureState.Invalid)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private void TapGestureUpdated(GestureRecognizer gesture)
        {
            if (gesture.State != GestureRecognizerState.Ended) return;
            
            var ray = camera.ScreenPointToRay(new Vector3(gesture.FocusX, gesture.FocusY, 0.0f));
            if (Physics.Raycast(ray, out var hit, float.MaxValue, objectLayerMask))
            {
                //Debug.Log(hit.transform.name);
                if (hit.transform.TryGetComponentInParent<Furniture>(out var furniture))
                {
                    if (selectedObject != furniture)
                    {
                        Select(furniture);
                        return;
                    }
                }
            }

            Deselect();
        }
    }
}
