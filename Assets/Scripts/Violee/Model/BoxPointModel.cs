using UnityEngine;

namespace Violee
{
    public class BoxPointModel : ModelBase<BoxPointData>
    {
        protected override void OnReadData()
        {
            Binder.From(data.Visited).To(gameObject.SetActive).Immediate();
        }
    }
}