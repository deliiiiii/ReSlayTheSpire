using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View
{
    public class MinimapView : ViewBase
    {
        public required Button ReGenerateBtn;
        public required RenderTexture TarTexture;
        public required CinemachineVirtualCamera MinimapCameraVirtual;
        public required Camera MinimapCamera;
        public required RawImage MinimapImg;
        public required RawImage FullScreenImg;
        public float ChangeSpeed = 1.2f;
        public float MiniSize = 12f;

        [Header("Load")]
        public required GameObject PnlLoad;
        
        bool isMinimap => MinimapImg.enabled;
        protected override void IBL()
        {
            Binder.Update(SwitchMap);
            GameManager.GeneratingMapState.OnEnter(() => PnlLoad.SetActive(true));
            GameManager.GeneratingMapState.OnExit(() => PnlLoad.SetActive(false));

            ShowMinimap();
        }



        void SwitchMap(float dt)
        {
            ChangeFOV(dt);
            if (!Input.GetKeyDown(KeyCode.Tab))
                return;
            if (isMinimap)
            {
                ShowFullScreenMap();
                return;
            }
            ShowMinimap();
        }

        void ChangeFOV(float dt)
        {
            var tarSize = isMinimap ? MiniSize : BoxModelManager.MaxSize / 1.616f;
            if (!Mathf.Approximately(MinimapCameraVirtual.m_Lens.OrthographicSize, tarSize))
            {
                MinimapCameraVirtual.m_Lens.OrthographicSize = Mathf.Lerp(MinimapCameraVirtual.m_Lens.OrthographicSize,
                    tarSize,
                    ChangeSpeed * dt);
            }
        }
        void ShowMinimap()
        {
            RefreshTexture(256, 256);
            FullScreenImg.enabled = false;
            MinimapImg.enabled = true;
        }

        void ShowFullScreenMap()
        {
            // 设置gameObject的 RectTransform长宽
            FullScreenImg.gameObject.GetComponent<RectTransform>().sizeDelta  = new Vector2(Screen.height, Screen.height);
            RefreshTexture(Screen.height, Screen.height);
            FullScreenImg.enabled = true;
            MinimapImg.enabled = false;
        }
        
        void RefreshTexture(int width, int height)
        {
            TarTexture.Release();
            TarTexture.width = width;
            TarTexture.height = height;
            TarTexture.Create();
            MinimapCamera.targetTexture = TarTexture;
            MinimapImg.texture = TarTexture;
            FullScreenImg.texture = TarTexture;
        }
    }
}