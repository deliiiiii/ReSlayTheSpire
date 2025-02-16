/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Package1
{
    public partial class UI_Component1 : GComponent
    {
        public UI_TextC m_textCurHP;
        public UI_TextC m_textHPSlash;
        public GButton m_buttonHit;
        public UI_TextC m_textMaxHP;
        public UI_TextC m_textDefendC;
        public UI_TextC m_textDefend;
        public UI_TextC m_textAttackC;
        public UI_TextC m_textAttack;
        public UI_TextC m_textCoinC;
        public UI_TextC m_textCoin;
        public GButton m_buttonRefine;
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
            m_buttonHit = (GButton)GetChildAt(2);
            m_textMaxHP = (UI_TextC)GetChildAt(3);
            m_textDefendC = (UI_TextC)GetChildAt(4);
            m_textDefend = (UI_TextC)GetChildAt(5);
            m_textAttackC = (UI_TextC)GetChildAt(6);
            m_textAttack = (UI_TextC)GetChildAt(7);
            m_textCoinC = (UI_TextC)GetChildAt(8);
            m_textCoin = (UI_TextC)GetChildAt(9);
            m_buttonRefine = (GButton)GetChildAt(10);
        }
    }
}