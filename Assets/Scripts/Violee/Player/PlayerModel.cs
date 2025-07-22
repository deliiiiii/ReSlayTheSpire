using System;
using UnityEngine;

namespace Violee.Player
{
    public class PlayerModel : Singleton<PlayerModel>
    {
        Rigidbody rg;
        MeshRenderer mr;

        protected override void Awake()
        {
            base.Awake();
            rg = GetComponent<Rigidbody>();
            mr = GetComponent<MeshRenderer>();
        }

        public static void OnEnterPlaying(Vector3 pos3D)
        {
            Instance.transform.position = pos3D;
            Instance.rg.useGravity = true;
            Instance.mr.enabled = true;s
        }
        
        public static void OnExitPlaying()
        {
            Instance.rg.useGravity = false;
            Instance.mr.enabled = false;
        }
    }
}