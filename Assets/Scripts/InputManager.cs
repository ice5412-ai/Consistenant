using System;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Habillage
{
    public class InputManager : MonoSingleton<InputManager>
    {
        [SerializeField] protected Camera sceneCamera;
        protected Vector3 LastPosition;

        [SerializeField] protected LayerMask layerMask;
        [SerializeField] private float pressDuration = 0.2f;
        public PanGestureRecognizer PanGesture { get; private set; }
        public LongPressGestureRecognizer LongPressGesture { get; private set; }
        public TapGestureRecognizer TapGesture { get; private set; }
        
        public ScaleGestureRecognizer ScaleGesture { get; private set; }
        public TapGestureRecognizer DoubleTapGesture;

        protected override void OnInitializing()
        {
            base.OnInitializing();
            
            layerMask = LayerMask.GetMask("Grid");
            sceneCamera = Camera.main;
            PanGesture = new PanGestureRecognizer();
        
            LongPressGesture = new LongPressGestureRecognizer();
            LongPressGesture.MinimumDurationSeconds = pressDuration;
        
            TapGesture = new TapGestureRecognizer();
            ScaleGesture = new ScaleGestureRecognizer();
            
            DoubleTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            TapGesture.RequireGestureRecognizerToFail = DoubleTapGesture;
            //FingersScript.Instance.AddGesture(DoubleTapGesture);
        
            FingersScript.Instance.AddGesture(PanGesture);
            FingersScript.Instance.AddGesture(LongPressGesture);
            FingersScript.Instance.AddGesture(TapGesture);
            FingersScript.Instance.AddGesture(ScaleGesture);
            
            FingersScript.Instance.CaptureGestureHandler = CaptureGestureHandler;
        }

        private void OnEnable()
        {
            //Debug.Log("Enable");
            
            
        }

        public static bool IsOverUI;
        
        private void Update()
        {
            IsOverUI = CheckIsOverUI();
        }

        //From: https://www.reddit.com/r/Unity3D/comments/6o9zsy/comment/dkgxa1i/
        public static bool CheckIsOverUI()
        {
            var _pointerData = new PointerEventData(EventSystem.current);

            switch (SystemInfo.deviceType)
            {
                case DeviceType.Desktop:
                    _pointerData.position = Input.mousePosition;
                    break;
                case DeviceType.Handheld:
                {
                    //Debug.Log("HandHeld");
                    if(Input.touchCount > 0){
                        _pointerData.position = Input.touches[0].position;
                    }
                    break;
                }
            }

            List<RaycastResult> _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_pointerData, _results);

            if (_results.Count > 0)
            {
                for (var _index = 0; _index < _results.Count; _index++)
                {
                    var _result = _results[_index];

                    if (_result.gameObject.TryGetComponent<CanvasRenderer>(out var _canvasRenderer))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnDisable()
        {
            if (!FingersScript.HasInstance) return;
        
            FingersScript.Instance.RemoveGesture(PanGesture);
            FingersScript.Instance.RemoveGesture(LongPressGesture);
            FingersScript.Instance.RemoveGesture(TapGesture);
            FingersScript.Instance.RemoveGesture(ScaleGesture);
            //FingersScript.Instance.RemoveGesture(DoubleTapGesture);
        }
        
        public Vector3 GetSelectedMapPosition(Vector3 inputPos)
        {
            inputPos.z = sceneCamera.nearClipPlane;
            Ray ray = sceneCamera.ScreenPointToRay(inputPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                LastPosition = hit.point;
            }
            return LastPosition;
        }
        
        private static bool? CaptureGestureHandler(GameObject obj)
        {
            // I've named objects PassThrough* if the gesture should pass through and NoPass* if the gesture should be gobbled up, everything else gets default behavior
            if (obj.name.StartsWith("PassThrough"))
            {
                // allow the pass through for any element named "PassThrough*"
                return false;
            }
            else if (obj.name.StartsWith("NoPass"))
            {
                // prevent the gesture from passing through, this is done on some of the buttons and the bottom text so that only
                // the triple tap gesture can tap on it
                return true;
            }

            // fall-back to default behavior for anything else
            return null;
        }
    }
}
