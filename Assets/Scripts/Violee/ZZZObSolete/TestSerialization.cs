using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Violee;

public class TestSerialization : SerializedMonoBehaviour
{
    [OdinSerialize][NonSerialized]
    public required TestS TestS;
}

