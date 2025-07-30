using UnityEngine;

namespace Violee
{
    public class BoxPointModel : ModelBase<BoxPointData>
    {
        SpriteRenderer? sr;
        static float flashTime => Configer.SettingsConfig.PointSpriteFlashTime;
        static float alpha => Configer.SettingsConfig.PointSpriteAlpha;
        BindDataUpdate? bFlash;
        protected override void OnReadData()
        {
            sr ??= GetComponent<SpriteRenderer>();
            Binder.From(data.Visited).To(gameObject.SetActive).Immediate();
            Binder.From(data.IsFlash).To(b =>
            {
                if (b)
                {
                    bFlash = Binder.Update(_ => sr.color = sr.color.SetAlpha(Mathf.PingPong(Time.time / flashTime, 1)), EUpdatePri.Sprite);
                }
                else
                {
                    bFlash?.UnBind();
                    sr.color = sr.color.SetAlpha(alpha / 255f);
                }
            }).Immediate();
        }
    }
}