using UnityEngine;

namespace Habillage
{
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void BeforeFirstSceneLoad()
        {
            PlayerData.ReadSave();
            RuntimeData.Initialize();
        }
    }
}
