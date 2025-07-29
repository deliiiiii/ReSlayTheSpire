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

        public void OnEnterPlaying(Vector3 pos3D)
        {
            transform.position = pos3D + Vector3.up * (1.5f * transform.position.y);
            gameObject.SetActive(true);
        }
        
        public void OnExitPlaying()
        {
            gameObject.SetActive(false);
        }

        public void Tick(float dt)
        {
            fpc.Tick();
        }
    }
}