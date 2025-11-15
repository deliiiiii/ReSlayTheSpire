using Newtonsoft.Json;
using RSTS.CDMV;
using UnityEngine;

namespace RSTS;

[SerializeField]
public abstract class DataBase<TConfig> where TConfig : ConfigBase
{
    [SerializeReference][JsonIgnore] 
    public required TConfig Config;

    protected abstract void LoadConfig();
}