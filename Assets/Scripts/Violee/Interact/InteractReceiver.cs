using System;
using UnityEngine;
using UnityEngine.Events;

namespace Violee.Interact
{
    public class InteractReceiver : MonoBehaviour
    {
        public event Action? OnEnterInteract;
        public event Action? OnExitInteract;

        // event
        public void EnterInteract()
        {
            OnEnterInteract?.Invoke();
        }

        public void ExitInteract()
        {
            OnExitInteract?.Invoke();
        }
    }
}