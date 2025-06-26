using System; // require keep for Windows Universal App
using UnityEngine;

namespace UniRx.Triggers
{
    [DisallowMultipleComponent]
    public class ObservableUpdateTrigger : ObservableTriggerBase
    {
        Subject<Unit> update;

        /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
        void Update()
        {
            update?.OnNext(Unit.Default);
        }

        /// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
        public IObservable<Unit> UpdateAsObservable()
        {
            return update ??= new Subject<Unit>();
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            update?.OnCompleted();
        }
    }
}