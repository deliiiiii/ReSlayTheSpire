using System.Collections.Generic;
using UnityEngine;

namespace Violee;
[CreateAssetMenu(fileName = nameof(ConBuffConfigList), menuName = "Violee/" + nameof(ConBuffConfigList))]
public class ConBuffConfigList : ScriptableObject
{
    public SerializableDictionary<EBuffType, ConBuffConfig> BuffConfigDic = [];
}
