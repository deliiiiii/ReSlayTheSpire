using UnityEngine;
using UnityEngine.Events;

namespace Violee.Interact
{
    public class InteractReceiver : MonoBehaviour
    {
        public UnityEvent OnInteract;

        // event
        public void Interact()
        {
            OnInteract?.Invoke();
        }
    }
}