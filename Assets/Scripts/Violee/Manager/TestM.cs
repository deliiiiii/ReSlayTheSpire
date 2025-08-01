using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee;

public class TestM : MonoBehaviour
{
    
    public EBoxDir D;
    SceneItemConfig c => Configer.SceneItemConfigList.SceneItemConfigs[0];

    [Button]
    public void Test()
    {
        MapManager.FirstBoxModel.CreateSceneItemModel(D, c);
    }
}