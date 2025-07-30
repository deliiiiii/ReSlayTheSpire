using System;
using System.Diagnostics.CodeAnalysis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    public class PlayerModel : Singleton<PlayerModel>
    {
        public MiniItemData Stamina => playerData.Stamina;
        public MiniItemData Energy => playerData.Energy;
        public MiniItemData Gloves => playerData.Gloves;
        public MiniItemData Dice => playerData.Dice;
        
        FirstPersonController fpc = null!;
        [SerializeField]
        PlayerData playerData = null!;
        
        protected override void Awake()
        {
            base.Awake();
            fpc = GetComponent<FirstPersonController>();
            playerData = PlayerData.Create();
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