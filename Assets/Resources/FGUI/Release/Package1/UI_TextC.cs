/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Package1
{
    public partial class UI_TextC : GComponent
    {
        public GTextField m_text0;
        public const string URL = "ui://8sxnaau1eqxn1";

        public static UI_TextC CreateInstance()
        {
            return (UI_TextC)UIPackage.CreateObject("Package1", "TextC");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_text0 = (GTextField)GetChildAt(0);
        }
    }
}