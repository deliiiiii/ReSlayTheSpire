using System;
using UnityEngine;

namespace RSTS.Test;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

public abstract class ModelBase<TData> : MonoBehaviour
	where TData : DataBase, new()
{
	[SerializeReference]
	protected TData Data;
	
	[Obsolete("At least ONE config is required.")]
	protected static TData CreateData() => DataBase.Create<TData>([]);
	protected static TData CreateData(params ConfigBase[] configParams) => DataBase.Create<TData>(configParams);
}