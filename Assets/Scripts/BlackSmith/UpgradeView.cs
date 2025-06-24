using UnityEngine.UI;

namespace BlackSmith
{
    public class UpgradeView : ViewBase
    {
        public Button Btn1;


        public override void IBL()
        {
            Bind();
        }

        void Bind()
        {
            Binder.From(Btn1).To(() => Configer.MainConfig.ClickValue.ImposeFinalMulti(new Buff(0.5f, "ccc")));
        }
    }
}