using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using JimmysUnityUtilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Habillage
{
    public class CameraController : MonoBehaviour
    {
        
        /// <summary>Require this area to be visible at all times</summary>
        [Tooltip("Require this area to be visible at all times")]
        public Collider VisibleArea;
        
        /// <summary>Dampening for velocity when pan is released, lower values reduce velocity faster.</summary>
        [Tooltip("Dampening for velocity when pan is released, lower values reduce velocity faster.")]
        [Range(0.0f, 1.0f)]
        public float Dampening = 0.8f;
        
        /// <summary>The threshold scale gesture must change in units before executing</summary>
        [Tooltip("The threshold scale gesture must change in units before executing")]
        [Range(0.0f, 1.0f)]
        public float ScaleThreshold = 0.15f;
        
        /// <summary>The layers that can be tapped on for objects to center the camera on them</summary>
        [Tooltip("The layers that can be tapped on for objects to center the camera on them")]
        public LayerMask TapToCenterLayerMask = -1;
        
        [SerializeField] private Vector3 cameraAnimationTargetPosition;
        private Vector3 velocity;
        [SerializeField] private Camera camera;

        public bool LockX, LockY, LockZ;
        
        
        private IEnumerator AnimationCoRoutine()
        {
            Vector3 start = transform.position;

            // animate over 1/2 second
            for (float accumTime = Time.deltaTime; accumTime <= 0.5f; accumTime += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(start, cameraAnimationTargetPosition, accumTime / 0.5f);
                yield return null;
            }
        }

        private void Awake()
        {
            if (!camera) 
                camera = GetComponentInChildren<Camera>();

            camera.GetOrAddComponent<PhysicsRaycaster>();
            camera.GetOrAddComponent<Physics2DRaycaster>();
            
        }

        private void OnEnable()
        {
            InputManager.Instance.ScaleGesture.ThresholdUnits = ScaleThreshold;
            InputManager.Instance.ScaleGesture.ZoomSpeed = 6;

            InputManager.Instance.ScaleGesture.StateUpdated += ScaleGesture_Updated;
            InputManager.Instance.PanGesture.StateUpdated += PanGesture_Updated;
            InputManager.Instance.TapGesture.StateUpdated += TapGesture_Updated;
            
            InputManager.Instance.ScaleGesture.AllowSimultaneousExecution(InputManager.Instance.PanGesture);
        }

        private void OnDisable()
        {
            InputManager.Instance.ScaleGesture.StateUpdated -= ScaleGesture_Updated;
            InputManager.Instance.PanGesture.StateUpdated -= PanGesture_Updated;
            InputManager.Instance.TapGesture.StateUpdated -= TapGesture_Updated;
            
            InputManager.Instance.ScaleGesture.DisallowSimultaneousExecution(InputManager.Instance.PanGesture);
        }

        public void Focus(GameObject gameObject)
        {
            cameraAnimationTargetPosition = new Vector3(camera.transform.position.x, gameObject.transform.position.y, camera.transform.position.z);
            StopAllCoroutines();
            StartCoroutine(AnimationCoRoutine());
            //LockY = true;
        }

        public void UnFocus()
        {
            //LockY = false;
        }
        
        private void TapGesture_Updated(DigitalRubyShared.GestureRecognizer gesture)
        {
            if (gesture.State != GestureRecognizerState.Ended)
            {
                return;
            }

            Ray ray = camera.ScreenPointToRay(new Vector3(gesture.FocusX, gesture.FocusY, 0.0f));
            RaycastHit hit;
            if (Physics.Raycast(ray,  out hit, float.MaxValue, TapToCenterLayerMask))
            {
                // adjust camera x, y to look at the tapped / clicked sphere
                cameraAnimationTargetPosition = new Vector3(hit.transform.position.x, hit.transform.position.y, camera.transform.position.z);
                StopAllCoroutines();
                StartCoroutine(AnimationCoRoutine());
            }
        }

        private void PanGesture_Updated(DigitalRubyShared.GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                StopAllCoroutines();

                // convert pan coordinates to world coordinates
                // get z position, orthographic this is 0, otherwise it's the z coordinate of all the spheres
                float z = (camera.orthographic ? 0.0f : 10.0f);
                var x = LockX ? 0f : gesture.DeltaX;
                var y = LockY ? 0f : gesture.DeltaY;
                Vector3 pan = new Vector3(x, y, z);
                Vector3 zero = camera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, z));
                Vector3 panFromZero = camera.ScreenToWorldPoint(pan);
                Vector3 panInWorldSpace = zero - panFromZero;

                if (LockX)
                {
                    panInWorldSpace.x = 0;
                }

                if (LockY)
                {
                    panInWorldSpace.y = 0;
                }

                if (LockZ)
                {
                    panInWorldSpace.z = 0;
                }
                
                camera.transform.position += panInWorldSpace;
            }
            else if (gesture.State == GestureRecognizerState.Ended)
            {
                float z = (camera.orthographic ? 0.0f : 10.0f);
                Vector3 zero = camera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, z));
                Vector3 one = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, z));
                float worldWidth = one.x - zero.x;
                float worldHeight = one.y - zero.y;
                float worldWidthRatio = Screen.width / worldWidth;
                float worldHeightRatio = Screen.height / worldHeight;
                float velocityX = LockX ? 0f : gesture.VelocityX / -worldWidthRatio;
                float velocityY = LockY ? 0f : gesture.VelocityY / -worldHeightRatio;
                velocity = new Vector3(velocityX, velocityY, 0.0f);
            }
        }
        
        private void ScaleGesture_Updated(DigitalRubyShared.GestureRecognizer gesture)
        {
            var scaleGesture = (ScaleGestureRecognizer)gesture;
            if (scaleGesture.State != GestureRecognizerState.Executing || scaleGesture.ScaleMultiplier == 1.0f)
            {
                return;
            }

            // invert the scale so that smaller scales actually zoom out and larger scales zoom in
            float scale = 1.0f + (1.0f - scaleGesture.ScaleMultiplier);

            if (camera.orthographic)
            {
                float newOrthographicSize = Mathf.Clamp(camera.orthographicSize * scale, 1.0f, 100.0f);
                camera.orthographicSize = newOrthographicSize;
            }
            else
            {
                // get camera look vector
                Vector3 forward = camera.transform.forward;

                // set the target to the camera x,y and 0 z position
                Vector3 target = transform.position;
                target.z = 0.0f;

                // get distance between camera target and camera position
                float distance = Vector3.Distance(target, transform.position);

                // come up with a new distance based on the scale gesture
                float newDistance = Mathf.Clamp(distance * scale, 1.0f, 100.0f);

                // set the camera position at the new distance
                transform.position = target - (forward * newDistance);
            }
        }
    }
}
