using UnityEngine;

namespace Habillage
{
    public abstract class BaseRecord : MonoBehaviour
    {
        public GameObject Parent;
        public abstract void SaveData(bool _complete = false);
        public abstract bool MarkAsCompleted();
    }
}