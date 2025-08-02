using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Violee;

public class PlayerMono : MonoBehaviour
{
    [NonSerialized][ShowInInspector]
    public PlayerData PlayerData = null!;
    [HideInInspector]
    public FirstPersonController Fpc = null!;
    void Awake()
    {
        Fpc = GetComponent<FirstPersonController>();
        gameObject.SetActive(false);
    }
}