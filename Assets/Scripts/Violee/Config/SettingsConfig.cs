using UnityEngine;

namespace Violee
{
    [CreateAssetMenu(fileName = nameof(SettingsConfig), menuName = "Violee/" + nameof(SettingsConfig))]
    public class SettingsConfig : ScriptableObject
    {
        public bool RefreshConfigOnAwake;
        public bool ShowBoxCost = true;
        public float BoxCostPosOffset = 0.7f;
    }
}