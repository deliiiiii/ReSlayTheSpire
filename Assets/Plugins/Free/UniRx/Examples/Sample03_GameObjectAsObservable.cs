#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

using System;
using UniRx;
using UnityEngine;
using UniRx.Triggers; // for enable gameObject.EventAsObservbale()

namespace UniRx.Examples
{
    public class Sample03_GameObjectAsObservable : MonoBehaviour
    {
        void Start()
        {
            // All events can subscribe by ***AsObservable if enables UniRx.Triggers
            this.OnMouseDownAsObservable()
                .Select(_ => gameObject.UpdateAsObservable())
                .TakeUntil(gameObject.OnMouseUpAsObservable())
                // .TakeWhile(_ => Input.GetKeyDown(KeyCode.A))
                .Select(_ => Input.mousePosition)
                .RepeatUntilDestroy(this)
                .Subscribe(x => Debug.Log(x), ()=> Debug.Log("!!!" + "complete"));
        }
    }
}

#endif