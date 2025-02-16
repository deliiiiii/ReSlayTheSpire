/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Package1
{
    public partial class UI_Component1 : GComponent
    {
        public UI_TextC m_textCurHP;
        public UI_TextC m_textHPSlash;
        public UI_TextC m_textMaxHP;
        public const string URL = "ui://8sxnaau1fdxq0";

        public static UI_Component1 CreateInstance()
        {
            return (UI_Component1)UIPackage.CreateObject("Package1", "Component1");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_textCurHP = (UI_TextC)GetChildAt(0);
            m_textHPSlash = (UI_TextC)GetChildAt(1);
            m_textMaxHP = (UI_TextC)GetChildAt(3);
        }
    }
}