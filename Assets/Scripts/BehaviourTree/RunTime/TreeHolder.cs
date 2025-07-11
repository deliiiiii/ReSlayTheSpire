#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace BehaviourTree
{
    [ExecuteInEditMode][Serializable][JsonObject(MemberSerialization = MemberSerialization.Fields)]
    public class TreeHolder : MonoBehaviour
    {
        [CanBeNull]
        public RootNode Root;
        public bool ShowStartTick = true;
        public bool RunOnAwake;
        float saveTimer;
        public float SaveTime = 1f;

        BindDataAct<float> b;

        void Awake()
        { 
            if (Root == null)
                return;
            MyDebug.Log($"Awake Tree: {Root.Name}", LogType.Tick);
            Load();
        }

        void Update()
        {
            if (!RunOnAwake)
                return;
            saveTimer += Time.deltaTime;
            if (saveTimer > SaveTime)
            {
                Save();
                saveTimer = 0f;
            }
            if (!Application.isPlaying)
                return;
            Tick(Time.deltaTime);
        }

        void Save()
        {
            MyDebug.Log($"Saving Tree: {Root?.Name}", LogType.Tick);
            Saver.Save($"Assets/DataTree", $"{Root!.Name}", Root);
        }

        void Load()
        {
            MyDebug.Log($"LoadOnRunning: {Root?.Name}", LogType.Tick);
            Root = Saver.Load<RootNode>($"Assets/DataTree", $"{Root!.Name}");
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        public EState Tick(float dt)
        {
            if (ShowStartTick)
            {
                MyDebug.Log("----------Start Tick----------", LogType.Tick);
            }
            return Root.Tick(dt);
        }
    }
}