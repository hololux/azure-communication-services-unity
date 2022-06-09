using System;
using Hololux.Acs;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using  UnityEngine.Rendering;

namespace HoloSpaces
{
    public class FrameSender : MonoBehaviour
    {
        #region private serialized variables
        [Header("Video")]
        [SerializeField] private float sendInterval = .1f;
        [Header("RenderTexture")]
        [SerializeField] private bool useMipMaps = true;
        [SerializeField] private int antiAliasingCount = 2;
        [SerializeField] private bool autoGenerateMips = true;
        [SerializeField] private Camera targetRenderCamera;
        [SerializeField] private RawImage targetRawImage;
        #endregion
        
        
        #region  private variables
        private float lastSendTime;
        private RenderTexture renderSourceTexture;
        private int sendTimeStamp = 0;
        private bool spectatorReady = false;
        private Texture2D readableTexture;
        private Vector2Int textureSize = new Vector2Int(1280,720);
        #endregion
        protected IAzureCommunication AzureCommunication;
        
        private NativeArray<byte> _gpuBuffer;
        private bool _canSendNewFrame = true;
        
        #region unity callbacks
        private void Start()
        {
            _gpuBuffer = new NativeArray<byte>(1280 * 720 * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);    
            SetUpRenderClient();
        }

        private void OnDestroy()
        {
            _gpuBuffer.Dispose();
        }

        private void Update()
        {
            if(!spectatorReady) return;
            
            if (Time.time >= lastSendTime + sendInterval)
            {
                SendCurrentCameraFrame();
                lastSendTime = Time.time;
            }
        }
        #endregion
        
        #region  private methods
        private void SetUpRenderClient()
        {
            var renderTextureDescriptor = new RenderTextureDescriptor(textureSize.x, textureSize.y, RenderTextureFormat.ARGB32, 24);
            renderTextureDescriptor.sRGB = true; // Render texture contains sRGB (color) data
            renderSourceTexture = CreateRenderTexture(renderTextureDescriptor);
            
            renderSourceTexture.useMipMap = useMipMaps;
            renderSourceTexture.antiAliasing = antiAliasingCount;
            renderSourceTexture.autoGenerateMips = autoGenerateMips;
            
            readableTexture = new Texture2D(renderTextureDescriptor.width, renderTextureDescriptor.height, TextureFormat.RGBA32, renderTextureDescriptor.useMipMap,QualitySettings.activeColorSpace == ColorSpace.Linear);
            
            Camera spectatorCamera = targetRenderCamera;
            spectatorCamera.targetTexture = renderSourceTexture;
            //spectatorReady = true;
            lastSendTime = Time.time;
        }
        
        public void SendCurrentCameraFrame()
        {
            if (_canSendNewFrame)
            {
                SendFrameAsync();
            }
            
            spectatorReady = true;
        }


        private void SendFrameAsync()
        {
            _canSendNewFrame = false;
            
            var temp = RenderTexture.GetTemporary(renderSourceTexture.descriptor);
            Graphics.Blit(renderSourceTexture, temp, new Vector2(1, -1), new Vector2(0, 1));
            Graphics.Blit(temp, renderSourceTexture);
            RenderTexture.ReleaseTemporary(temp);
            
            AsyncGPUReadback.RequestIntoNativeArray(ref _gpuBuffer, renderSourceTexture, 0, (request)=>
            {
                _canSendNewFrame = true;
                if (request.hasError)
                {
                    Debug.Log("GPU readback error detected.");
                    return;
                }
                
                byte[] imageBytes = _gpuBuffer.ToArray();
                
                AzureCommunication = AcsFactory.GetCommunicationInstance();
                AzureCommunication.SendFrame(imageBytes);
            });
        }
        
        private RenderTexture CreateRenderTexture(RenderTextureDescriptor renderTextureDescriptor)
        {
            var renderTexture = new RenderTexture(renderTextureDescriptor);
            renderTexture.DiscardContents();
            return renderTexture;
        }
        #endregion
    }
}
