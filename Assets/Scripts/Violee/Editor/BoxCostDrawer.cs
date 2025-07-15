using UnityEngine;
using UnityEditor;
namespace Violee.Editor
{
    [CustomEditor(typeof(BoxModel))]
    public class BoxCostDrawer : UnityEditor.Editor
    {
        static GUIStyle labelStyle;
        void OnEnable()
        {
            labelStyle = new GUIStyle()
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState { textColor = Color.red },
                alignment = TextAnchor.MiddleCenter

            };
            SceneView.duringSceneGui += _ => OnSceneGUI();
        }
        void OnSceneGUI()
        {
            if(Configer.SettingsConfig.ShowBoxCost)
            {
                foreach(var point in MapModel.GetAllPoints())
                {
                    Handles.Label(point.Pos, "9", labelStyle);
                }
            }
            SceneView.RepaintAll();
        }
    }
}