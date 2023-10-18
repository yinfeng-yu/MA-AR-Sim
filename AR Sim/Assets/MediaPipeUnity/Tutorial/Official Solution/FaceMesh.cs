using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Mediapipe.Unity.Tutorial
{
    public class FaceMesh : MonoBehaviour
    {
        [SerializeField] private TextAsset _configAsset;
        [SerializeField] private RawImage _screen;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private int _fps;

        private CalculatorGraph _graph;

        private WebCamTexture _webCamTexture;
        private Texture2D _inputTexture;
        private Color32[] _pixelData;

        private IEnumerator Start()
        {
            if (WebCamTexture.devices.Length == 0)
            {
                throw new System.Exception("Web Camera devices are not found");
            }
            var webCamDevice = WebCamTexture.devices[0];
            _webCamTexture = new WebCamTexture(webCamDevice.name, _width, _height, _fps);
            _webCamTexture.Play();

            yield return new WaitUntil(() => _webCamTexture.width > 16);

            _screen.rectTransform.sizeDelta = new Vector2(_width, _height);
            _screen.texture = _webCamTexture;

            _inputTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            _pixelData = new Color32[_width * _height];

            _graph = new CalculatorGraph(_configAsset.text);
            _graph.StartRun().AssertOk();

            while (true)
            {
                _inputTexture.SetPixels32(_webCamTexture.GetPixels32(_pixelData));
                var imageFrame = new ImageFrame(ImageFormat.Types.Format.Srgba, _width, _height, _width * 4, _inputTexture.GetRawTextureData<byte>());
                _graph.AddPacketToInputStream("input_video", new ImageFramePacket(imageFrame)).AssertOk();

                yield return new WaitForEndOfFrame();
            }
        }

        private void OnDestroy()
        {
            if (_webCamTexture != null)
            {
                _webCamTexture.Stop();
            }

            if (_graph != null)
            {
                try
                {
                    _graph.CloseInputStream("input_video").AssertOk();
                    _graph.WaitUntilDone().AssertOk();
                }
                finally
                {

                    _graph.Dispose();
                }
            }
        }
    }
}