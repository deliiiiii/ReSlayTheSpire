using UnityEngine;

namespace BlackSmith
{
    [CreateAssetMenu(fileName = "MainConfig", menuName = "ScriptableObjects/MainConfig", order = 1)]
    public class MainConfig : ScriptableObject
    {
        public BuffFloat ClickValue;
        public BuffFloat MineAutoValue;
        public BuffFloat WeaponAutoValue;
        public BuffFloat EnchantAutoValue; 
        
        public SerializableDictionary<EMine, float> MineCostDic;
        public SerializableDictionary<EMine, SerializableDictionary<EWeapon, float>> WeaponCostDic;
        public SerializableDictionary<EMine, SerializableDictionary<EWeapon, SerializableDictionary<EEnchant, float>>> EnchantCostDic;
        public SerializableDictionary<EMine, SerializableDictionary<EWeapon, SerializableDictionary<EEnchant, int>>> EnchantPriceDic;
    }
}