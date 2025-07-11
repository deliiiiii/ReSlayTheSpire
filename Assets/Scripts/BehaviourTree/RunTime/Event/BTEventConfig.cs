using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    [CreateAssetMenu(fileName = nameof(BTEventConfig), menuName = "BehaviourTree/" + nameof(BTEventConfig))]
    public class BTEventConfig : ScriptableObject
    {
        public SerializableDictionary<EEventK1, List<string>> K1ToK2s = new ();
        

        static SerializableDictionary<EEventK1, List<string>> k1Tok2DicField;
        public static SerializableDictionary<EEventK1, List<string>> K1Tok2Dic
        {
            get
            {
                if ((k1Tok2DicField?.Count ?? 0) == 0)
                {
                    k1Tok2DicField = Resourcer.LoadAsset<BTEventConfig>(nameof(BTEventConfig))?.K1ToK2s;
                    if (k1Tok2DicField == null)
                    {
                        MyDebug.LogError($"No {nameof(BTEventConfig)} found");
                        k1Tok2DicField = new();
                    }
                }

                return k1Tok2DicField;
            }
        }
    }
}