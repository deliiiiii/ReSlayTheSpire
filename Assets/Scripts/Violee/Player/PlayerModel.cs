using System;
using UnityEngine;

namespace Violee.Player
{
    public class PlayerModel : Singleton<PlayerModel>
    {
        protected override void Awake()
        {
            base.Awake();
            MapModel.OnEndDij += pos3D => transform.position = pos3D;
        }
    }
}