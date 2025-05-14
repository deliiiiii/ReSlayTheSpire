using UnityEngine.UI;

namespace SubstanceP
{
    [TransitionTarget(typeof(Image))]
    public partial class ImageTransition : Transition
    {
        [TransitionField("fillAmount")] private float fill;

        private partial void InitFill(Image target) => target.fillAmount = fill;

        private partial void ProcessFill(Image target, float delta) => target.fillAmount += delta;
    }
}
