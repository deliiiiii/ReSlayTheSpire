using System.Collections.Generic;
using UnityEngine;

namespace SubstanceP
{
    public class TransitionExecutor : MonoBehaviour
    {
        static TransitionExecutor() => DontDestroyOnLoad(new GameObject("TransitionExecutor").AddComponent<TransitionExecutor>());

        private readonly static List<Transition> anims = new();
        private readonly static Stack<Transition> disabled = new(), enabling = new();

        public static void Register(Transition anim) => enabling.Push(anim);
        public static void Unregister(Transition anim) => disabled.Push(anim);

        private void Update()
        {
            while (enabling.TryPop(out var e)) anims.Add(e);
            anims.ForEach(anim => anim.Tick(Time.deltaTime));
            while (disabled.TryPop(out var d)) anims.Remove(d);
        }
    }
}