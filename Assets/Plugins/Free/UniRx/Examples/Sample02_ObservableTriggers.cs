using UnityEngine;
using UniRx.Triggers; // Triggers Namepsace
using System;
using System.Collections.Generic;

namespace UniRx.Examples
{
    public class Sample02_ObservableTriggers : MonoBehaviour
    {
        void Start()
        {
            // Get the plain object
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Add ObservableXxxTrigger for handle MonoBehaviour's event as Observable
            cube.AddComponent<ObservableUpdateTrigger>()
                .UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.A))
                .SampleFrame(1000)
                .Subscribe(x => Debug.Log("cube"), () => Debug.Log("destroy"));
 
            // destroy after 3 second:)
            // GameObject.Destroy(cube, 3f);
        }
    }
}