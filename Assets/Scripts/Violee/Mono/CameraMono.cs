using Cinemachine;
using UnityEngine;

namespace Violee;

public class CameraMono : Singleton<CameraMono>
{
    public float DefaultEaseTime = 0.5f;
    public float SceneItemEaseTime = 2f;
    public float PlayerEaseTime = 2f;
    public required Camera PlayerCameraIns;
    public required CinemachineVirtualCamera PlayerVirtualCameraIns;
    public required CinemachineVirtualCamera MinimapVirtualCameraIns;
    public required CinemachineVirtualCamera SceneItemVirtualCameraIns;
    public required CinemachineVirtualCamera TitleVirtualCameraIns;
    
    public static int DefaultEase => (int)(Instance.DefaultEaseTime * 1000);
    public static int PlayerEase => (int)(Instance.PlayerEaseTime * 1000);
    public static int SceneItemEase => (int)(Instance.SceneItemEaseTime * 1000);
    public static Camera PlayerCamera => Instance.PlayerCameraIns;
    public static CinemachineVirtualCamera PlayerVirtualCamera => Instance.PlayerVirtualCameraIns;
    public static CinemachineVirtualCamera MinimapVirtualCamera => Instance.MinimapVirtualCameraIns;
    public static CinemachineVirtualCamera SceneItemVirtualCamera => Instance.SceneItemVirtualCameraIns;
    public static CinemachineVirtualCamera TitleVirtualCamera => Instance.TitleVirtualCameraIns;
}