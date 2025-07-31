using System;
using UnityEngine;
using UnityEngine.Events;

namespace Violee.Interact
{
    public class InteractReceiver : MonoBehaviour
    {
        public event Action? OnEnterInteract;
        public event Action? OnLeaveInteract;

        // event
        public void EnterInteract()
        {
            OnEnterInteract?.Invoke();
        }

        public void LeaveInteract()
        {
            OnLeaveInteract?.Invoke();
        }
    }
}