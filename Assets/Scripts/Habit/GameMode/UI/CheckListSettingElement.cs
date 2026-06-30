using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Habillage
{
    public class CheckListSettingElement : MonoBehaviour
    {
        public TMP_InputField InputField;
        public HabitSettingChecklist habitSettingChecklist;
        public void OnEndEdit()
        {
            if (string.IsNullOrWhiteSpace(InputField.text))
            {
                if (transform.parent.childCount > 1)
                {
                    habitSettingChecklist.RemoveEmptyCheckList(this);
                }
            }
            Canvas.ForceUpdateCanvases();
        }
    }
}