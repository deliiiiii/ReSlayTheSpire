using System;
using System.Threading.Tasks;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Violee.Interact;
using Violee.Violee.Interact;

namespace Violee;

public class SceneItemModel : ModelBase<SceneItemData>
{
    [SerializeReference] public required GameObject HideAfterRunOut;
    [SerializeReference] public required GameObject ShowAfterRunOut;
    [SerializeReference] public required InteractReceiver Ir;
    
    InteractHasCamera? iCamera;

    protected override void OnReadData()
    {
        iCamera = GetComponent<InteractHasCamera>();
        if (Data.HasCount)
        {
            Data.OnRunOut += () =>
            {
                HideAfterRunOut.SetActive(false);
                ShowAfterRunOut.SetActive(true);
            };
        }

        Ir.GetInteractInfo = GetCb;
    }

    InteractInfo? GetCb()
    {
        if (!Data.CanUse())
            return null;
        var ret = new InteractInfo
        {
            Act = Data.Use,
            Description = Data.GetInteractDes(), 
            Color = Data.DesColor(),
            
            IsSleep = Data is PurpleSceneItemData,
            SleepTime = 2.89f,
        };
        if (iCamera != null)
        {
            ret.HasCamera = true;
            ret.Act += () =>
            {
                CameraMono.SceneItemVirtualCamera.Follow = transform;
                // CameraMono.SceneItemVirtualCamera.Follow.localPosition = iCamera.transform.position - transform.position;
                CameraMono.SceneItemVirtualCamera.LookAt = transform;
                CameraMono.SceneItemVirtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset =
                    // iCamera.CameraTransform.position - transform.position;
                    new Vector3(iCamera.CameraTransform.localPosition.x * transform.lossyScale.x,
                                iCamera.CameraTransform.localPosition.y * transform.lossyScale.y,
                                iCamera.CameraTransform.localPosition.z * transform.lossyScale.z);
            };
        }

        return ret;
    }
}


