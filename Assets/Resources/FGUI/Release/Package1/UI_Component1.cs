/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Package1
{
    public partial class UI_Component1 : GComponent
    {
        public GTextField m_text0;
        public const string URL = "ui://8sxnaau1fdxq0";

        public static UI_Component1 CreateInstance()
        {
            return (UI_Component1)UIPackage.CreateObject("Package1", "Component1");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_text0 = (GTextField)GetChildAt(0);
        }
    }
}