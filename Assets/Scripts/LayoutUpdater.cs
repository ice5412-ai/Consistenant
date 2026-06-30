using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class LayoutUpdater : MonoBehaviour
    {
        public void RebuildLayout()
        {
            StartCoroutine(ForceRebuildLayoutAll());
        }
        
        public IEnumerator ForceRebuildLayoutAll()
        {
            yield return new WaitForEndOfFrame();

            var _rectTransforms = GetComponentsInChildren<RectTransform>();

            foreach(var _rect in _rectTransforms)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
            }
        }
    }
}