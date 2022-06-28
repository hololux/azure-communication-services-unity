using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Hololux.Acs
{
    /// <summary>
    /// Helper class which sends the render texture data to the remote clients
    /// Currently only supported on android devices
    /// </summary>
    public class AcsAndroidFrameSender : MonoBehaviour
    {
        #region private serialized variables
        [Header("Video")]
        [SerializeField] private float sendInterval = .1f;
        [Header("RenderTexture")]
        [SerializeField] private bool useMipMaps = true;
        [SerializeField] private int antiAliasingCount = 2;
        [SerializeField] private bool autoGenerateMips = true;
        [SerializeField] private Camera targetRenderCamera;
        [SerializeField] private Button frameSenderButton;
        #endregion
        
        #region  private variables
        private float _lastSendTime;
        private RenderTexture _renderSourceTexture;
        private bool _sendFrames = false;
        private Vector2Int _textureSize = new Vector2Int(1280,720);
        private IAzureCommunication _azureCommunication;
        private NativeArray<byte> _gpuBuffer;
        private bool _canSendNewFrame = true;
        #endregion
        
        #region unity callbacks
        private void Start()
        {
            #if PLATFORM_ANDROID
            _gpuBuffer = new NativeArray<byte>(1280 * 720 * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);    
            SetUpFrameSender();
            #else
            frameSenderButton.gameObject.SetActive(false);
            #endif
        }

        private void OnDestroy()
        {
            _gpuBuffer.Dispose();
        }

        private void Update()
        {
            if(!_sendFrames) return;
            
            if (Time.time >= _lastSendTime + sendInterval)
            {
                SendFrames();
                _lastSendTime = Time.time;
            }
        }
        #endregion

        #region public methods
        public void StartSendingFrames()
        {
            _sendFrames = true;
        }
        #endregion
        
        #region  private methods
        private void SetUpFrameSender()
        {
            var renderTextureDescriptor = new RenderTextureDescriptor(_textureSize.x, _textureSize.y, RenderTextureFormat.ARGB32, 24)
            {
                sRGB = true // Render texture contains sRGB (color) data
            };
            
            _renderSourceTexture = CreateRenderTexture(renderTextureDescriptor);
            _renderSourceTexture.useMipMap = useMipMaps;
            _renderSourceTexture.antiAliasing = antiAliasingCount;
            _renderSourceTexture.autoGenerateMips = autoGenerateMips;
            
            targetRenderCamera.targetTexture = _renderSourceTexture;
            
            _lastSendTime = Time.time;
        }
        
        private void SendFrames()
        {
            if (_canSendNewFrame)
            {
                SendFrameAsync();
            } 
        }

        private void SendFrameAsync()
        {
            _canSendNewFrame = false;
            
            var temp = RenderTexture.GetTemporary(_renderSourceTexture.descriptor);
            Graphics.Blit(_renderSourceTexture, temp, new Vector2(1, -1), new Vector2(0, 1));
            Graphics.Blit(temp, _renderSourceTexture);
            RenderTexture.ReleaseTemporary(temp);
            
            AsyncGPUReadback.RequestIntoNativeArray(ref _gpuBuffer, _renderSourceTexture, 0, (request)=>
            {
                _canSendNewFrame = true;
                if (request.hasError)
                {
                    Debug.Log("GPU readback error detected.");
                    return;
                }
                
                byte[] imageBytes = _gpuBuffer.ToArray();
                
                _azureCommunication = AcsFactory.GetCommunicationInstance();
                _azureCommunication.SendFrame(imageBytes);
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
