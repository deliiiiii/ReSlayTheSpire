using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    [CreateAssetMenu(fileName = nameof(SettingsConfig), menuName = "Violee/" + nameof(SettingsConfig))]
    public class SettingsConfig : ScriptableObject
    {
        public bool RefreshConfigOnAwake;
        public bool QuickKey;
        public bool ShowBoxCost = true;
        [MinValue(0)][MaxValue(0.5f)]
        public float BoxCostPosOffset = 0.35f;

        public float PointSpriteFlashTime = 1f;
        public float PointSpriteAlpha = 166f;

        [Tooltip("用这个乘以边长，等于交互范围半径")][SerializeField]
        [MinValue(0.1f)][MaxValue(1f)]
        float interactCasterRadius = 0.3f;
        public float InteractCasterRadius => BoxHelper.BoxSize * interactCasterRadius;
        
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