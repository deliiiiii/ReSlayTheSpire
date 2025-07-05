using System;
using System.Collections.Generic;

namespace BehaviourTree
{
    public static class TypeCache
    {
        /// <summary>
        /// 缓存每个NodeBaseEditor的DropDownFieldData
        /// ActionNodeEditor, [ActionNodeDebug, ActionNodeDelay, ...]
        /// </summary>
        public static readonly Dictionary<Type, List<Type>> EditorToSubNodeDic = new();

        public static Type GetEditorByConcreteSubType(Type subType)
        {
            foreach (var kvp in EditorToSubNodeDic)
            {
                if (kvp.Value.Contains(subType))
                {
                    return kvp.Key;
                }
            }
            return null;
        }
    }
}