using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Violee.View
{
    public class GameView : ViewBase<GameView>
    {
        protected override void IBL()
        {
            
            GameManager.GeneratingMapState
                .OnEnter(() => LoadPnl.SetActive(true))
                .OnExit(() => LoadPnl.SetActive(false));
            
            GameManager.PlayingState.OnEnter(() =>
            {
                MiniItemPnl.SetActive(true);
                ShowMinimap();
                Binder.From(PlayerManager.Stamina.Count).ToTxt(StaminaTxt).Immediate();
                Binder.From(PlayerManager.Energy.Count).ToTxt(EnergyTxt).Immediate();
                Binder.From(PlayerManager.Gloves.Count).ToTxt(GlovesTxt).Immediate();
                Binder.From(PlayerManager.Dice.Count).ToTxt(DiceTxt).Immediate();
            }).OnUpdate(dt =>
            {
                var cb = PlayerManager.ReticleCb;
                NormalReticle.SetActive(cb == null);
                FindReticle.SetActive(cb != null);
                SceneItemInfoPnl.SetActive(cb != null);
                SceneItemInfoTxt.text = cb?.Des ?? "";
                SceneItemInfoTxt.color = cb?.Color ?? Color.black;

                SwitchMap(dt);
            }).OnExit(() => MiniItemPnl.SetActive(false));
            
            GameManager.PausedState
                .OnEnter(() => PausePnl.SetActive(true))
                .OnExit(() => PausePnl.SetActive(false));
            Binder.From(ContinueBtn).To(GameManager.UnpauseWindow);
        }

        [Header("Load & Pause")]
        public required GameObject LoadPnl;
        public required GameObject PausePnl;
        public required Button ContinueBtn;

        #region Minimap

        [Header("Minimap")]
        public required RenderTexture TarTexture;
        public required CinemachineVirtualCamera MinimapCameraVirtual;
        public required Camera MinimapCamera;
        public required RawImage MinimapImg;
        public required RawImage FullScreenImg;
        public float ChangeSpeed = 1.2f;
        public float MiniSize = 12f;
        bool isMinimap => MinimapImg.enabled;
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
            var tarSize = isMinimap ? MiniSize : MapManager.MaxSize / 1.616f;
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
            FullScreenImg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height, Screen.height);
            // MyDebug.Log("ShowFullScreenMap " + Screen.height);
            RefreshTexture(Screen.width, Screen.height);
            FullScreenImg.enabled = true;
            MinimapImg.enabled = false;
        }
        
        void RefreshTexture(int width, int height)
        {
            TarTexture.Release();
            TarTexture.width = height;
            TarTexture.height = height;
            TarTexture.Create();
            FullScreenImg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(height, height);
            MinimapCamera.targetTexture = TarTexture;
            MinimapImg.texture = TarTexture;
            FullScreenImg.texture = TarTexture;
        }
        

        #endregion
        
        
        #region MiniItem
        [Header("MiniItem")] 
        public required GameObject MiniItemPnl;
        public required Text StaminaTxt;
        public required Text EnergyTxt;
        public required Text GlovesTxt;
        public required Text DiceTxt;
        #endregion


        #region SceneItem
        [Header("SceneItem")]
        public required GameObject NormalReticle;
        public required GameObject FindReticle;
        public required GameObject SceneItemInfoPnl;
        public required Text SceneItemInfoTxt;
        #endregion
    }
}