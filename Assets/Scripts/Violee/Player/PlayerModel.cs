using System;
using UnityEngine;

namespace Violee.Player
{
    public class PlayerModel : Singleton<PlayerModel>
    {
        Rigidbody rg;
        MeshRenderer mr;
        FirstPersonController fpc;

        protected override void Awake()
        {
            base.Awake();
            rg = GetComponent<Rigidbody>();
            mr = GetComponent<MeshRenderer>();
            fpc = GetComponent<FirstPersonController>();
            gameObject.SetActive(false);
        }

        public static void OnEnterPlaying(Vector3 pos3D)
        {
            Instance.transform.position = pos3D;
            Instance.gameObject.SetActive(true);
        }
        
        public static void OnExitPlaying()
        {
            Instance.gameObject.SetActive(false);
        }

        public static void Tick(float dt)
        {
            Instance.fpc.Tick();
        }
    }
}