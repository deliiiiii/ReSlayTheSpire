using UnityEngine;

namespace SubstanceP
{
    [TransitionTarget(typeof(SpriteRenderer))]
    public partial class SpriteRendererTransition : Transition
    {
        [TransitionField("color")] private Color col;

        private partial void InitCol(SpriteRenderer target) => target.color = col;

        private partial void ProcessCol(SpriteRenderer target, Color delta) => target.color += delta;
    }
}
