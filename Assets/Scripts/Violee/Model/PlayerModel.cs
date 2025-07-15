using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Violee
{
    [Serializable]
    public class PlayerData
    {
        public int X;
        public int Y;
    }

    public class PlayerModel : MonoBehaviour
    {
        void Awake()
        {
            MapModel.OnGenerateMap += OnGenerateMap;
            MapModel.OnInputEnd += OnInputEnd;
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                OnInputMove?.Invoke(playerData.X, playerData.Y, 0, 1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                OnInputMove?.Invoke(playerData.X, playerData.Y, 0, -1);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnInputMove?.Invoke(playerData.X, playerData.Y, -1, 0);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnInputMove?.Invoke(playerData.X, playerData.Y, 1, 0);
            }
        }

        public static event Action<int, int, int, int> OnInputMove;

        [SerializeField]
        PlayerData playerData;

        void OnGenerateMap()
        {
            playerData.X = playerData.Y = 0;
            OnInputMove?.Invoke(playerData.X, playerData.Y, 0, 0);
        }

        void OnInputEnd(Loc loc)
        {
            playerData.X = loc.X;
            playerData.Y = loc.Y;
            transform.position = new Vector3(loc.X, loc.Y, 0);
        }

        
    }
        
        
}