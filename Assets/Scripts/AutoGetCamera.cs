using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Habillage
{
    [RequireComponent(typeof(Canvas))]
    public class AutoGetCamera : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        private void Update()
        {
            if (!canvas) 
                canvas = GetComponent<Canvas>();

            if (!canvas.worldCamera)
            {
                canvas.worldCamera = Camera.main;
            }
        }
    }
}
