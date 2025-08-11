using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    [CreateAssetMenu(fileName = nameof(SettingsConfig), menuName = "Violee/" + nameof(SettingsConfig))]
    public class SettingsConfig : ScriptableObject
    {
        // public bool RefreshConfigOnAwake;
        public bool IsDevelop;

        [ShowIf(nameof(IsDevelop))][SerializeField]
        bool quickKey;
        public bool QuickKey => quickKey && IsDevelop;
        public void ReverseQuickKey() => quickKey = !quickKey;

        [ShowIf(nameof(IsDevelop))][SerializeField]
        bool showBoxCost;
        public bool ShowBoxCost => showBoxCost && IsDevelop;
        public void ReverseShowBoxCost() => showBoxCost = !showBoxCost;
        
        [ShowIf(nameof(IsDevelop))][SerializeField]
        bool disablePause;
        public bool DisablePause => disablePause && IsDevelop;
        public void ReverseDisablePause() => disablePause = !disablePause;

        [ShowIf(nameof(IsDevelop))] [SerializeField]
        bool dreamCatcherGachaUp;
        public bool DreamCatcherGachaUp => dreamCatcherGachaUp && IsDevelop;
        public void ReverseDreamCatcherGachaUp() => dreamCatcherGachaUp = !dreamCatcherGachaUp;
        [ShowIf(nameof(IsDevelop))] [SerializeField]
        bool addTiltWall;
        public bool AddTiltWall => addTiltWall && IsDevelop;
        public void ReverseAddTiltWall() => addTiltWall = !addTiltWall;

        [MinValue(0)][MaxValue(0.5f)]
        public float BoxCostPosOffset = 0.35f;

        public float TimeSpeed = 30;

        public float PointSpriteFlashTime = 1f;
        public float PointSpriteAlpha = 166f;

        [SerializeField]
        [MinValue(0.1f)][MaxValue(1f)]
        // 用这个乘以边长，等于交互范围半径
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