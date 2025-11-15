using UnityEngine;
using UnityEngine.UI;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace RSTS;
public class BuffModel : MonoBehaviour
{
    public Text TxtName;
    public Text TxtCount;

    public void ReadData(BuffDataBase data)
    {
        TxtName.text = data.Name;
        TxtCount.text = data.HasStack ? data.StackCount.ToString() : string.Empty;
    }
    
    public void ChangeCount(int count)
    {
        TxtCount.text = count.ToString();
    }
}