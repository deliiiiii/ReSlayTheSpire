# 1 创建（或打开）配置文件(.asset)

### 1.1 创建根节点(referred)

推荐在Assets/DataTree/中，右键Create-BahaviourTree-RootNode，创建根节点RootNode x.asset（资源名字任意）

![image-20250706193435324](https://raw.githubusercontent.com/deliiiiii/PictureRep/main/25/7/image-20250706193435324.png)

用法：详见[3 运行](#3 运行(referred))



### 1.2 创建数据黑板(referred)

可以自定义脚本继承BahaviourTree.Blackboard类（Backboard继承ScriptableObject）。

CreateAssetMenu特性中的menuName强烈建议写BehaviourTree/数据类名，示例：

```C#
using System;
using UnityEngine;

namespace BehaviourTree
{
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

        public Observable<int> IntObservable;
    }
}
```

支持序列化以下数据类型：

```C#
int
enum // 会被认作int
long
float
double
bool
string
Vector3
Observable<T> // 一个包装类（其中T也是以上列出来的数据类型），定义了event Action<T> OnValueChangedAfter（值的变化事件）（其他更复杂的事件如【累计增加】【每间隔xx时间】咕咕），与Binder（自研数据绑定类）联合有妙妙效果，会让你感觉再也不用关注数据变化，写代码从此变成了体力活！
```

用法：可被拖入GuardNodeCompare、ActionNodeSetValue节点的Blackboard字段，详见[2 编辑行为树](##2 编辑行为树(referred))



### 1.3 节点发送的事件名(referred)

事件的键采用了双重字典的形式：

k1：EEventK1

k2：string

即：先选择一个EEventK1，一个EEventK1会对应List\<string\>

k1可扩充（只需在Assets/Scripts/BahaviourTree/RunTime/Event/BTEvent.cs的EEventK1中添加一行，不用加其他任何代码），不要删。 

k2可扩充（在Assets/DataTree/BTEventConfig.asset文件中，在某个Key的Value列表上点击+），不要删。

用法：可在ActionNodeEvent节点上选择键名，详见[2 编辑行为树](##2 编辑行为树(referred))

# 2 编辑行为树(referred)

在Project窗口中双击在[1.1](##1.1 创建根节点(referred))中创建的.asset文件，即可打开节点编辑器，不同文件对应的编辑器窗口可以多开，窗口标题显示为文件名。



节点状态有三种：

```
 Failed,	// 对应节点背景红色
 Succeeded,	// 对应节点背景绿色
 Running,	// 对应节点背景青色
```

上面的XXXNode按钮代表创建节点（的默认第一个具体实现子类）。

节点上有：输入输出端口、同一个基类下的所有子节点选项、对应.asset文件（双击可打开对应的Inspector窗口）、信息栏。节点右下角可拖动来改变大小。



节点的基类共有五种：

### 动作节点（叶子节点）

| 基类名         | Inspector                                                    | 执行                                           | 返回                                                         |
| -------------- | ------------------------------------------------------------ | ---------------------------------------------- | ------------------------------------------------------------ |
| **ActionNode** | 指定Debug、Delay是否执行，若执行则追加显示Debug类型（Log，Warning，Error）与内容、Delay类型（按帧or按时间）与时长 | 一些函数。顺序：Debug->Delay->执行子类具体函数 | 正在Delay => **Running**；被Guard => **Failed**；否则：子类的返回值 |

| 子类名                 | Inspector                                                    | 执行                            | 返回          |
| ---------------------- | ------------------------------------------------------------ | ------------------------------- | ------------- |
| **ActionNodeEmpty**    | 什么其他的都没有                                             | 只是基类的Debug、Delay          | **Succeeded** |
| **ActionNodeEvent**    | 指定[1.3](##1.3 节点发送的事件名(referred))中定义的双重键（Odin序列化了两个下拉框，无需手动输入k2字符串，键的扩充方法已在上文描述） | 发送一次名为k2的事件            | **Succeeded** |
| **ActionNodeSetValue** | 拖入[1.2](##1.2 创建数据黑板(referred))中创建的数据黑板，指定黑板类中的字段名（反射自动拿到名字）、指定目标值（反射自动拿到类型） | 将黑板上的指定字段值改为ToValue | **Succeeded** |
| **ActionNodeTree**     | 拖入一个[1.1](##1.1 创建根节点(referred))中创建的根节点      | 子树                            | 子树的返回值  |

### 组合节点

| 基类名            | Inspector  | 执行                     | 返回                                                      |
| ----------------- | ---------- | ------------------------ | --------------------------------------------------------- |
| **CompositeNode** | 什么都没有 | 按一定逻辑依次执行子节点 | 被Guard => **Failed**；否则：按一定逻辑返回子节点的返回值 |

| 子类名           | Inspector                                                    | 执行                                                         | 返回                                                         |
| ---------------- | ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| **SequenceNode** | 什么都没有                                                   | 从左往右依次执行子节点                                       | 直到有子节点返回Failed就返回Failed                           |
| **SelectorNode** | IsRandom：配置权重List（长度为子节点个数，不可增减），执行时随机ran | 从左往右依次执行子节点。如果勾选了IsRandom，只执行ran的节点，失败不会尝试下一个 | 直到有子节点返回**Succeeded**就返回**Succeeded**，如果勾选了IsRandom，返回ran到的节点的返回值 |

### **装饰节点**

| 基类名           | Inspector  | 执行       | 返回                                                         |
| ---------------- | ---------- | ---------- | ------------------------------------------------------------ |
| **DecorateNode** | 什么都没有 | 执行子节点 | 被Guard => **Failed**；否则：按一定逻辑对子节点的返回值产生修改 |

| 子类名             | Inspector                                          | 执行                                                 | 返回                                                         |
| ------------------ | -------------------------------------------------- | ---------------------------------------------------- | ------------------------------------------------------------ |
| **InverseNode**    | 什么都没有                                         | 执行子节点                                           | 反转子节点返回值                                             |
| **LimitTimesNode** | LimitTimes：子节点执行次数；LimitTimer：已执行次数 | 执行子节点LimitTimes次。如果返回**Failed**则清空次数 | LimitTimes次执行结束后，返回**Succeeded**；否则大概率返回**Running**，子节点返回**Failed**则返回**Failed** |

### **条件节点**

| 基类名        | Inspector  | 执行     | 返回                                                         |
| ------------- | ---------- | -------- | ------------------------------------------------------------ |
| **GuardNode** | 什么都没有 | 一个条件 | 自身条件不成立 OR 被Guard => **Failed**，并使子节点立即返回**Failed**；否则**Succeeded** |

| 子类名                   | Inspector                                                  | 执行                                           | 返回             |
| ------------------------ | ---------------------------------------------------------- | ---------------------------------------------- | ---------------- |
| **GuardNodeCompare**     | 拖入数据黑板，指定黑板类中字段名，指定比较方法，指定目标值 | 按指定方法比较黑板上的指定值FromValue和ToValue | 反转子节点返回值 |
| **GuardNodeAlwaysFalse** | 什么都没有                                                 | 什么都没有                                     | **Failed**       |

### 根节点

全局唯一，多次点击创建根节点是没用的。



PS：当图发生修改（除了新增节点），节点编辑器中的内容会立即更新到文件，并输出一个Warning。这时代表修改已经保存了。<u>如果发现实在没保存（毕竟GraphView的申必bug不少），**可以多拖拖节点位置**，每次拖动之后是一定会保存的。</u>

PPS：节点的端口限制（输入or输出、连单个or多个、连接目标类型、端口名）保存在文件Assets/DataTree/PortToDrawConfig.asset，非开发者不要修改。



# **3 运行(referred)**

打开场景Scenes/BehaviourTree.unity，找到TreeHolder物体上的脚本TreeHolder.cs

| TreeHolder.cs的Inspector上字段名 | 解释                                                 |
| -------------------------------- | ---------------------------------------------------- |
| Root                             | 该Mono脚本对应的根节点，把RootNode x.asset拖进来即可 |
| RunOnStart                       | 生命周期函数的Start里启动行为树                      |
| EnableEvents                     | 是否在启动后接受事件                                 |
| TypeToEvents                     | 双重键所决定的事件字典，值是List\<UnityEvent\>       |

<u>为避免k2这个删掉而懒得查配置文件，Inspector提供以下两个bool（其实相当于按钮）</u>

| TreeHolder.cs的Inspector上字段名 | 解释                                                         |
| -------------------------------- | ------------------------------------------------------------ |
| IWannaRefillTheWholeDiccccc      | 点击即可重填所有缺失的k1及其对应的所有k2                     |
| IWannaRefillTheEventType         | 在Inspector里选好一个EventType，点击即可重填该k1对应的所有k2 |
| EventType                        | IWannaRefillTheEventType想重填的k1                           |

# **其他 ...**

### **源码**

**Assets\Scripts\BehaviourTree**



### **引用**

**Odin**

**UniRx**

**GraphView**

**以及自己项目中的Assets\Scripts\General（命名空间没写警告）**

