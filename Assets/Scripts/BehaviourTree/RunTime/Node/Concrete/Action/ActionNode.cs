using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class ActionNode : NodeBase
    {
        protected override EChildCountType childCountType { get; set; } = EChildCountType.None;
        protected Action OnEnter;
        protected override async Task<EState> OnTickChild(CancellationTokenSource cts)
        { 
            OnEnter?.Invoke();
            State.Value = EState.Running;
            await OnContinueAsync();
            return EState.Succeeded;
        }

        protected virtual async Task<EState> OnContinueAsync()
        {
            return EState.Succeeded;
        }
    }

    

    
}