using UnityEngine;

namespace Violee
{
    [CreateAssetMenu(fileName = nameof(SettingsConfig), menuName = "Violee/" + nameof(SettingsConfig))]
    public class SettingsConfig : ScriptableObject
    {
        public bool ShowBoxWhenCreated = true;
    }
}