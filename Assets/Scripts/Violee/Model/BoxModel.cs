using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Violee
{
    public class BoxModel : MonoBehaviour
    {
        [ShowInInspector]
        BoxData boxData;

        public SerializableDictionary<EWallType, WallModel> WallDic;

        public void InitData(BoxData fBoxData)
        {
            boxData = fBoxData;
            name = $"Box {fBoxData.Pos.x} {fBoxData.Pos.y}";
            WallDic?.ForEach(pair =>
            {
                if (fBoxData.HasWall(pair.Key))
                    pair.Value.gameObject.SetActive(true);
            });
        }
    }
}