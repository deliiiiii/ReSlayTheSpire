using SubstanceP;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Flow.Create()
        //     .Then(() => GetComponent<SpriteRenderer>().ToCol(Color.black).Easing(Transition.InOutElastic).During(5).Play())
        //     .Then(() => transform.FromPos(Vector3.left * 5 + Vector3.up * 2).ToPos(Vector3.right * 5 + Vector3.up * 2)
        //                          .ToRot(Vector3.forward * -360)
        //                          .During(2).Play())
        //     .Delay(() => transform.AddPos(Vector3.down * 4).During(2).Easing(Transition.OutBounce))
        //     .Then(() => transform.AddRot(Vector3.forward * 1440).During(3).Easing(Transition.InQuad).Play())
        //     .Then(() => transform.AddScale(Vector3.one * 2).During(3).Play())
        //     .Run();
        
        
        Flow.Create()
            .Then(() => GetComponent<SpriteRenderer>().ToCol(Color.black).Easing(Transition.InOutElastic).During(5).Play())
            // .Then(() => transform.FromPos(Vector3.left * 5 + Vector3.up * 2).ToPos(Vector3.right * 5 + Vector3.up * 2)
            //     .ToRot(Vector3.forward * -360)
            //     .During(2).Play())
            .Delay(() => transform.AddPos(Vector3.down * 4).During(10).Easing(Transition.OutBounce))
            // .Then(() => transform.AddRot(Vector3.forward * 1440).During(3).Easing(Transition.InQuad).Play())
            .Then(() => transform.AddScale(Vector3.one * 2).During(3).Play())
            .Run();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
