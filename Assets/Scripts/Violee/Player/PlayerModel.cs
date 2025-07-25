using System;
using UnityEngine;

namespace Violee
{
    public class PlayerModel : Singleton<PlayerModel>
    {
        FirstPersonController fpc = null!;

        protected override void Awake()
        {
            base.Awake();
            fpc = GetComponent<FirstPersonController>();
            gameObject.SetActive(false);
        }

        public static void OnEnterPlaying(Vector3 pos3D)
        {
            Instance.transform.position = pos3D + Vector3.up * (1.5f * Instance.transform.position.y);
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