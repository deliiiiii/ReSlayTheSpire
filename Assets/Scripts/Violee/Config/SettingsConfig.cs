using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    [CreateAssetMenu(fileName = nameof(SettingsConfig), menuName = "Violee/" + nameof(SettingsConfig))]
    public class SettingsConfig : ScriptableObject
    {
        public bool RefreshConfigOnAwake;
        public bool ShowBoxCost = true;
        public float BoxCostPosOffset = 0.7f;
        
        
        #region Yield Control
        [Header("Yield Control")]
        [MinValue(0)]
        public float YieldCount;
        int countNotYieldFrame;
        public async Task YieldFrames(float multi = 1)
        {
            var trueY = YieldCount * multi;
            if (trueY >= 1)
            {
                for (int y = 0; y < trueY; y++)
                {
                    await Task.Yield();
                }
                return;
            }

            countNotYieldFrame++;
            if (1f / countNotYieldFrame <= trueY)
            {
                countNotYieldFrame = 0;
                await Task.Yield();
            }
        }
        #endregion
    }
}