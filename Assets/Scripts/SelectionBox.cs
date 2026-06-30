using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class SelectionBox : MonoBehaviour
    {
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private TextMeshProUGUI selectionText;
        [SerializeField] private int BeginWithTextNumber;
        [SerializeField] private int currentText;
        [SerializeField] private string[] texts;
        [SerializeField] public SelectionEvent OnChangePreviousEvent = new SelectionEvent();
        [SerializeField] public SelectionEvent OnChangeNextEvent = new SelectionEvent();
        [Serializable] public class SelectionEvent : UnityEngine.Events.UnityEvent { }

        void Start()
        {
            currentText = BeginWithTextNumber;
            selectionText.text = texts[currentText];
        }
        private void OnEnable()
        {
            previousButton.onClick.AddListener(ChangePrevious);
            nextButton.onClick.AddListener(ChangeNext);
        }
        private void OnDisable()
        {
            previousButton.onClick.RemoveListener(ChangePrevious);
            nextButton.onClick.RemoveListener(ChangeNext);
        }
        private void ChangePrevious()
        {
            if (currentText <= 0)
            {
                currentText = texts.Length - 1;
            }
            else
            {
                currentText -= 1;
            }
            UpdateText();
            if (OnChangePreviousEvent != null)
            {
                OnChangePreviousEvent.Invoke();
            }
        }
        private void ChangeNext()
        {
            if (currentText >= texts.Length - 1)
            {
                currentText = 0;
            }
            else
            {
                currentText += 1;
            }
            UpdateText();
            if (OnChangeNextEvent != null)
            {
                OnChangeNextEvent.Invoke();
            }
        }

        private void UpdateText()
        {
            selectionText.text = texts[currentText];
        }


        public int CurrentTextNumber()
        {
            return (int)currentText;
        }

        public string currentTextString()
        {
            return (string)texts[currentText];
        }

        public void ForceSetInitialBeginWithTextNumber(int value)
        {
            currentText = BeginWithTextNumber = value;
            UpdateText();
        }
        public void ForceChangeTextNumber(int value)
        {
            currentText += value;
            UpdateText();
        }
    }
}
