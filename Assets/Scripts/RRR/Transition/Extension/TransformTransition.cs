using UnityEngine;

namespace SubstanceP
{
    [TransitionTarget(typeof(Transform))]
    public partial class TransformTransition : Transition
    {
        [TransitionField("localPosition")] private Vector3 pos;
        [TransitionField("localRotation.eulerAngles")] private Vector3 rot;
        [TransitionField("localScale")] private Vector3 scale;

        private partial void InitPos(Transform target) => target.localPosition = pos;
        private partial void InitRot(Transform target) => target.localRotation = Quaternion.Euler(rot);
        private partial void InitScale(Transform target) => target.localScale = scale;

        private partial void ProcessPos(Transform target, Vector3 delta) => target.localPosition += delta;
        private partial void ProcessRot(Transform target, Vector3 delta) => target.Rotate(delta);
        private partial void ProcessScale(Transform target, Vector3 delta) => target.localScale += delta;
    }
}
