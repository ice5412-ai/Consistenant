using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class CheckListObjectProperties : MonoBehaviour
    {
        [SerializeField] public Toggle Toggle;
        [SerializeField] public bool IsDone = false;
        [SerializeField] public string task = "";
        [SerializeField] public ParticleSystem ConfettiFX_PS;
        [SerializeField] public MMF_Player soundFX_confetti;
        [SerializeField] public MPImage CheckBox;
        [SerializeField] public MPImage CheckMark;
        [SerializeField] public MPImage TextBoxBG;
        [SerializeField] public MPImage TextBoxBD;
        [SerializeField] public TextMeshProUGUI Text;
        [SerializeField] public Color uiColor = Color.clear;
        [SerializeField] public Color defaultBG = new Color(.20f, .20f, .20f, 1);
        [SerializeField] public Color tintColor;
        [SerializeField] public Checklist checklist;
        [SerializeField] public int numberInList {get; private set;}

        public void SetList(bool _isDone, string _task, Color _color, Checklist _checklistController, int _numberInList)
        {
            IsDone = _isDone;
            Toggle.isOn = IsDone;
            Text.text = task = _task;
            uiColor = _color;
            numberInList =_numberInList;

            float H, S, V;
            Color.RGBToHSV(uiColor, out H, out S, out V);

            tintColor = Color.HSVToRGB(H, S, 0.4f);

            if (IsDone)
            {
                OnDone();
            }
            else
            {
                OnUndone();
            }
        }

        public void ToggleBool()
        {
            IsDone = Toggle.isOn;
            if (IsDone)
            {
                OnDone();
                ConfettiFX_PS.Play();
                //soundFX_confetti.PlayFeedbacks();
            }
            else
            {
                OnUndone();
            }
            checklist.UpdateCondition(numberInList, IsDone);
        }

        public void OnDone()
        {
            CheckBox.color = uiColor;
            CheckMark.color = uiColor;
            TextBoxBG.color = defaultBG;
            TextBoxBD.color = uiColor;
            Text.color = uiColor;
        }

        public void OnUndone()
        {
            CheckBox.color = Color.white;
            CheckMark.color = Color.white;
            TextBoxBG.color = tintColor; ;
            TextBoxBD.color = Color.white;
            Text.color = Color.white;
        }
    }
}
