using UnityEngine;

namespace Violee
{
    public class BoxPointModel : ModelBase<BoxPointData>
    {
        static float flashTime => Configer.SettingsConfig.PointSpriteFlashTime;
        static float alpha => Configer.SettingsConfig.PointSpriteAlpha;
        protected override void OnReadData()
        {
            BindDataUpdate? bFlash = null;
            var sr = GetComponent<SpriteRenderer>();
            Binder.From(Data.Visited).To(gameObject.SetActive).Immediate();
            Binder.From(Data.IsFlash).To(b =>
            {
                if (b)
                {
                    bFlash = Binder.Update(_ =>
                    {
                        if (sr == null)
                            return;
                        sr.color = sr.color.SetAlpha(Mathf.PingPong(Time.time / flashTime, 1));
                    }, EUpdatePri.Sprite);
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