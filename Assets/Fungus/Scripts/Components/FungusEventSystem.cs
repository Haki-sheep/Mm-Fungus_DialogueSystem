using UnityEngine;
using UnityEngine.EventSystems;

namespace Fungus
{
    [AddComponentMenu("")]
    public sealed class FungusEventSystem : MonoBehaviour
    {
        private void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            foreach (var Module in GetComponents<BaseInputModule>())
                if (!(Module is UnityEngine.InputSystem.UI.InputSystemUIInputModule))
                    Module.enabled = false;
            var InputModule = GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            if (InputModule == null)
            {
                InputModule = gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                InputModule.AssignDefaultActions();
            }
#endif
        }
    }
}
