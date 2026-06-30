using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Habillage
{
    public class HabitSettingChecklist : HabitModeSettingUI
    {
        [SerializeField] private CheckListSettingElement checkListPrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private ContentSizeFitter root;
        [SerializeField] private List<CheckListSettingElement> checkLists;

        public UnityEvent OnElementChanged;

        public List<string> GetCheckList()
        {
            return checkLists.Where(_c => !string.IsNullOrWhiteSpace(_c.InputField.text))
                .Select(_c => _c.InputField.text).ToList();
        }

        public void SyncCheckList(List<string> _checkList)
        {
            while (checkLists.Count > 0)
            {
                var _button = checkLists[0];
                Destroy(_button.gameObject);
                checkLists.Remove(_button);
            }

            foreach (var _text in _checkList)
            {
                var _element = AddNewCheckList();
                _element.InputField.SetTextWithoutNotify(_text);
            }
            AddNewCheckList();
            OnElementChanged?.Invoke();
        }

        public void OnEndEdit(string _value)
        {
            if (!string.IsNullOrWhiteSpace(_value))
            {
                AddNewCheckList();
                OnElementChanged?.Invoke();
            }
            Canvas.ForceUpdateCanvases();
        }

        public CheckListSettingElement AddNewCheckList()
        {
            var _newCheckList = Instantiate(checkListPrefab, content, false);
            _newCheckList.gameObject.SetActive(true);
            _newCheckList.habitSettingChecklist = this;
            ClearCheckListEvent();
            _newCheckList.InputField.onEndEdit.AddListener(OnEndEdit);
            checkLists.Add(_newCheckList);
            RefreshContentSize();
            Canvas.ForceUpdateCanvases();

            return _newCheckList;
        }

        public void RemoveEmptyCheckList(CheckListSettingElement _emptyChecklist)
        {
            if (_emptyChecklist != checkLists[checkLists.Count - 1])
            {
                checkLists.Remove(_emptyChecklist);
                Destroy(_emptyChecklist.gameObject);
                if (!string.IsNullOrWhiteSpace(checkLists[checkLists.Count - 1].InputField.text))
                {
                    AddNewCheckList();
                }
                else
                {
                    RefreshContentSize();
                    Canvas.ForceUpdateCanvases();
                }
            }
        }

        private void RefreshContentSize()
        {
            if (this.gameObject.activeSelf)
            {
                System.Collections.IEnumerator Routine()
                {
                    root.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    yield return null;
                    root.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                }
                this.StartCoroutine(Routine());
            }
            else
            {
                root.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        private void ClearCheckListEvent()
        {
            foreach (var _checkList in checkLists)
            {
                _checkList.InputField.onEndEdit.RemoveAllListeners();
            }
        }

        private void OnEnable()
        {
            AddNewCheckList();
        }

        private void OnDisable()
        {
            while (checkLists.Count > 0)
            {
                var _button = checkLists[0];
                Destroy(_button.gameObject);
                checkLists.Remove(_button);
            }
        }

        public override GameModeType Type => GameModeType.CheckList;
        public override ModeData GetModeData()
        {
            return new CheckListData(GetNotifyData(), GetCheckList());
        }

        public override bool IsValid()
        {
            return GetCheckList().Any();
        }
    }
}
