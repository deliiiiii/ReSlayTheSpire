using System;
using System.Linq;
using UnityEngine;

namespace RSTS;
[Serializable]
public class BuffCom
{
    [SerializeReference] MyList<BuffDataBase> buffList = [];
    public bool HasBuff<T>(out T buffData) where T : BuffDataBase
    {
        buffData = buffList.OfType<T>().First();
        return buffData == null;
    }
    public void AddBuff(BuffDataBase buffData)
    {
        buffList.MyAdd(buffData);
    }

    public void RemoveBuff(BuffDataBase buffData)
    {
        buffList.MyRemove(buffData);
    }
}