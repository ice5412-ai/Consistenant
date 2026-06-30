using System.Collections;
using System.Collections.Generic;
using JimmysUnityUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class OriginalValue : MonoBehaviour
    {
        [SerializeField] Button button;
        [SerializeField] List<GameObject> original;
        [SerializeField] List<GameObject> revealed;
        private void OnEnable()
        {
            button.onClick.AddListener(Revealing);
            foreach (GameObject b in revealed)
            {
                b.SetActive(false);
            }
        }

        public void Revealing()
        {
            foreach (GameObject a in original)
            {
                a.SetActive(false);
            }
            foreach (GameObject b in revealed)
            {
                b.SetActive(true);
            }
        }

        // Update is called once per frame
        private void OnDisable()
        {
            button.onClick.RemoveListener(Revealing);
        }
    }
}
