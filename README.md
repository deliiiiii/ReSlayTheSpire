

### 创建根节点

在这个目录创建根节点RootNode x.asset![image-20250706193435324](https://raw.githubusercontent.com/deliiiiii/PictureRep/main/25/7/image-20250706193435324.png)



### 创建数据黑板

可以自定义脚本继承BahaviourTree.BlackBoard类（BackBoard继承ScriptableObject）。

示例：

```C#
[Serializable]
[CreateAssetMenu(fileName = nameof(BlackboardTest), menuName = "BehaviourTree/" + nameof(BlackboardTest))]
public class BlackboardTest : Blackboard
{
    public int Intttt;
    public float Floatttt;
    public EState Stateeee;
    public bool Boollll;
    public string Stringggg;
    public Vector3 Vector3333;
}
```

支持序列化以下数据类型：

```C#
int
enum //会被认作int
long
float
double
bool
string
Vector3
```

然后根据[CreateAssetMenu]指定的路径生成so对应的.asset文件即可，稍后可以拖入妙妙节点真是妙蛙种子吃了妙脆角给他妈开门妙到家了。



### 编辑

双击RootNode x.asset即可打开节点编辑器，不同文件对应的窗口可以多开。当图发生修改（<u>逆天GraphView的bug太多。拖动节点是一定可以触发“发生修改”事件的</u>），节点编辑器中的内容会**<u>立即更新</u>**到文件。

节点状态有三种：

```
 Failed,
 Succeeded,
 Running,
```

节点有五种：

**ActionNode**：动作节点 仅执行一些函数，返回Running或Succeeded

​	--ActionNodeDebug： 输出Content

​	--ActionNodeDelay：延迟执行，返回Running

​	--ActionNodeSetValue：拖入数据黑板 且 选择字段名后，将黑板上的指定值改为ToValue

**CompositeNode**：组合节点 按一定逻辑依次执行子节点；按一定逻辑返回值

​	--SequenceNode：直到有子节点返回Failed就返回Failed

​	--SelectorNode：直到有子节点返回Succeeded就返回Succeeded

**DecorateNode**：装饰节点 执行子节点后，对返回值产生修改

​	--InverseNode：反转子节点返回值

​	--NothingNode：什么都不做，返回子节点返回值

**GuardNode**：条件节点 如果不成立则使被修饰的节点立即返回Failed

​	--GuardNodeCompare：拖入数据黑板 且 选择字段名 且选择比较方法后，按指定方法比较黑板上的指定值FromValue和ToValue

​	--GuardNodeMoreThan：一个测试Guard，输入Value和Threshold，返回Value > Threshold

**RootNode**：根节点 全局唯一，多次点击创建根节点是没用的



### 运行

打开场景Scenes/BehaviourTree.unity，找到TreeTest物体上的脚本TestTree.cs，把RootNode x.asset拖进Root



### 源码

Assets\Scripts\BehaviourTree



### 引用

Odin

UniRx

GraphView

以及自己项目中的Assets\Scripts\General（命名空间没写警告）

