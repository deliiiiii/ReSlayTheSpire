using UnityEngine;

namespace Violee
{
    public class BoxPointModel : ModelBase<BoxPointData>
    {
        protected override void ReadDataInternal()
        {
            Binder.From(data.Visited).To(gameObject.SetActive).Immediate();
        }
    }
}