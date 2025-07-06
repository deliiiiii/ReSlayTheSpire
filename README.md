

### 创建

在这个目录创建根节点RootNode x.asset![image-20250706193435324](https://raw.githubusercontent.com/deliiiiii/PictureRep/main/25/7/image-20250706193435324.png)





### 编辑

双击RootNode x.asset即可打开节点编辑器，不同文件对应的窗口可以多开。当图发生修改，节点编辑器中的内容会**立即更新**到文件。

节点状态有三种：

```
 Failed,
 Succeeded,
 Running,
```

节点有五种：

ActionNode：动作节点 仅执行一些函数，返回Running或Succeeded

CompositeNode：组合节点 按一定逻辑依次执行子节点；按一定逻辑返回值

​	--SequenceNode：直到有子节点返回Failed就返回Failed

​	--SelectorNode：直到有子节点返回Succeeded就返回Succeeded

DecorateNode：装饰节点 执行子节点后，对返回值产生修改

​	--InverseNode：反转子节点返回值

​	--NothingNode：返回子节点返回值

GuardNode：条件节点 如果不成立则使被修饰的节点立即返回Failed

RootNode：根节点 全局唯一，多次点击创建根节点是没用的



### 运行

打开场景Scenes/BehaviourTree.unity，找到TreeTest物体上的脚本TestTree.cs，把RootNode x.asset拖进Root



### 源码

Assets\Scripts\BehaviourTree



### 引用

Odin

UniRx

GraphView

以及自己项目中的Assets\Scripts\General（命名空间没写警告）

