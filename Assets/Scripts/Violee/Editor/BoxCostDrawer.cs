using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Violee.Editor
{
    [CustomEditor(typeof(BoxModelManager))]
    public class BoxCostDrawer : UnityEditor.Editor
    {
        static GUIStyle labelStyle;
        static List<BoxPointData> cachedPoints;
        void OnEnable()
        {
            labelStyle = new GUIStyle()
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState { textColor = Color.red },
                alignment = TextAnchor.MiddleCenter
            };
            
        }
        static void PaintAll()
        {
            if(Configer.Instance.SettingsConfig.ShowBoxCost)
            {
                foreach(var point in cachedPoints)
                {
                    var str = point.CostWall > 1e5 ? "∞" : point.CostWall.ToString();
                    Handles.Label(point.Pos2D, str, labelStyle);
                }
            }
        }
    }
}