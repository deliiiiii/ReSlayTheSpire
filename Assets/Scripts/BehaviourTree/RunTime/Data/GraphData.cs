using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BehaviourTree
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(GraphData), menuName = "BehaviourTree/" + nameof(GraphData))]
    public class GraphData : ScriptableObject
    {
        public IEnumerable<NodeData> NodeDataList => GetAllSubAssets().OfType<NodeData>();
        
        IEnumerable<Object> GetAllSubAssets()
        {
            var ret = new List<Object>();
            string assetPath = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(assetPath)) 
                return ret;
            ret.AddRange(AssetDatabase.LoadAllAssetsAtPath(assetPath));
            return ret;
        }
    }
}