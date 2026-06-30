using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class ToggleNumericSelectionBox : MonoBehaviour
    {
        [SerializeField] private Button TextMode;
        [SerializeField] private Button SelectionMode;
        [SerializeField] private GameObject SelectionContainer;
        [SerializeField] private bool IsToggled;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private TextMeshProUGUI textmodeText;
        [SerializeField] private TextMeshProUGUI selectionText;
        [SerializeField] private int BeginWithNumber;
        [SerializeField] private int currentNumber;
        [SerializeField] public SelectionEvent OnChangePreviousEvent = new SelectionEvent();
        [SerializeField] public SelectionEvent OnChangeNextEvent = new SelectionEvent();
        [Serializable] public class SelectionEvent : UnityEngine.Events.UnityEvent { }

        void Start()
        {
            IsToggled = false;
            currentNumber = BeginWithNumber;
            UpdateText();
        }
        private void OnEnable()
        {
            TextMode.onClick.AddListener(ToggleMode);
            SelectionMode.onClick.AddListener(ToggleMode);
            previousButton.onClick.AddListener(ChangePrevious);
            nextButton.onClick.AddListener(ChangeNext);
        }
        private void OnDisable()
        {
            TextMode.onClick.RemoveListener(ToggleMode);
            SelectionMode.onClick.RemoveListener(ToggleMode);
            previousButton.onClick.RemoveListener(ChangePrevious);
            nextButton.onClick.RemoveListener(ChangeNext);
        }
        private void ToggleMode()
        {
            IsToggled = !IsToggled;
            TextMode.gameObject.SetActive(!IsToggled);
            SelectionContainer.SetActive(IsToggled);
        }

        private void ChangePrevious()
        {
            if (currentNumber <= 1)
                return;

            currentNumber -= 1;

            UpdateText();

            if (OnChangePreviousEvent != null)
            {
                OnChangePreviousEvent.Invoke();
            }
        }
        private void ChangeNext()
        {
            if (currentNumber >= 9999)
                return;

            currentNumber += 1;

            UpdateText();

            if (OnChangeNextEvent != null)
            {
                OnChangeNextEvent.Invoke();
            }
        }

        private void UpdateText()
        {
            textmodeText.text = string.Format("{0:0000}", currentNumber);
            selectionText.text = string.Format("{0:0000}", currentNumber);
        }

        public int CurrentTextNumber()
        {
            return (int)currentNumber;
        }

        public void ForceSetInitialBeginWithNumber(int value)
        {
            currentNumber = BeginWithNumber = value;
            UpdateText();
        }

        public void ForceChangeNumber(int value)
        {
            currentNumber += value;
            UpdateText();
        }
    }
}

