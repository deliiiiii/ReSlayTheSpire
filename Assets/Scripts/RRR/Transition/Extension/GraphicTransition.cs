using UnityEngine;
using UnityEngine.UI;

namespace SubstanceP
{
    [TransitionTarget(typeof(Graphic))]
    public partial class GraphicTransition : Transition
    {
        [TransitionField("color")] private Color col;

        private partial void InitCol(Graphic target) => target.color = col;

        private partial void ProcessCol(Graphic target, Color delta) => target.color += delta;
    }
}
