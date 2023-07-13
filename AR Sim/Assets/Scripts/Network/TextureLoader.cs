using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Jobs;

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using UnityEngine.UI;



public class TextureLoader : MonoBehaviour
{
    #region Singleton
    public static TextureLoader instance;
    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }
    #endregion

    [SerializeField] public RenderTexture fpSrc;
    [SerializeField] public RenderTexture tpSrc;

    [SerializeField] public RawImage taskPageDst;
    [SerializeField] public RawImage monitorPageDst;

    // [SerializeField] public ToggleMenu toggleMenu;

    [SerializeField] private byte[] frameData;

    private int _width;
    private int _height;
    private bool _frameDataInitialized = false;

    [System.Serializable]
    public class SerializableColor
    {
        public float _r;
        public float _g;
        public float _b;
        // public float _a;

        public Color Color
        {
            get
            {
                // return new Color(_r, _g, _b, _a);
                return new Color(_r, _g, _b);
            }
            set
            {
                _r = value.r;
                _g = value.g;
                _b = value.b;
                // _a = value.a;
            }
        }

        public SerializableColor()
        {
            // (Optional) Default to white with an empty initialisation
            _r = 1f;
            _g = 1f;
            _b = 1f;
            // _a = 1f;
        }

        public SerializableColor(float r, float g, float b, float a = 0f)
        {
            _r = r;
            _g = g;
            _b = b;
            // _a = a;
        }

        public SerializableColor(Color color)
        {
            _r = color.r;
            _g = color.g;
            _b = color.b;
            // _a = color.a;
        }
    }    

    public SerializableColor[] ConvertToSerializableColorList(Color[] input)
    {
        SerializableColor[] retV = new SerializableColor[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            retV[i] = new SerializableColor(input[i]);
        }

        return retV;
    }

    public Color ConvertToUnityColor(SerializableColor input)
    {
        Color color = new Color();
        color.r = input._r;
        color.g = input._g;
        color.b = input._b;
        color.a = 1;
        return color;
    }

    public Color[] ConvertToUnityColorList(SerializableColor[] input)
    {
        Color[] retV = new Color[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            retV[i] = ConvertToUnityColor(input[i]);
        }

        return retV;
    }

    private void Start()
    {
        StartCoroutine(UpdateRawImage());
        MyTransmission.instance.streamUpdated += UpdateRender;
    }

    private void OnDestroy()
    {
        MyTransmission.instance.streamUpdated -= UpdateRender;
    }


    IEnumerator UpdateRawImage()
    {
        while (true)
        {
            Texture2D receivedTex = new Texture2D(_width, _height);
            try
            {
                Color[] receivedColours = ConvertToUnityColorList(DeserializeObject<SerializableColor[]>(frameData));

                receivedTex.SetPixels(receivedColours);
                receivedTex.Apply();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // if (toggleMenu.curToggleIndex == 0)
            // {
            //     monitorPageDst.texture = receivedTex;
            // }
            // else if (toggleMenu.curToggleIndex == 1)
            // {
            //     taskPageDst.texture = receivedTex;
            // }
            
            yield return new WaitForSeconds(0.5f);
            yield return null;
        }
    }

    struct FrameDataDeserializationJob : IJob
    {
        public void Execute()
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    void UpdateRender(StreamChunkMetaData metaData, byte[] chunk)
    {
        if (!_frameDataInitialized)
        {
            frameData = new byte[metaData.frameSize];
            _frameDataInitialized = true;

            _width = metaData.frameWidth;
            _height = metaData.frameHeight;
        }

        // List<JobHandle> jobHandles = new List<JobHandle>(Allocator.Temp);

        // Debug.Log($"chunk {metaData.index}, data length = {chunk.Length}, offset = {metaData.offset}");
        Buffer.BlockCopy(chunk, 0, frameData, metaData.offset, chunk.Length);

    }

    public SerializableColor[] GetRenderTexturePixels(RenderTexture tex)
    {
        RenderTexture.active = tex;
        Texture2D tempTex = new Texture2D(tex.width, tex.height);
        tempTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tempTex.Apply();

        return ConvertToSerializableColorList(tempTex.GetPixels());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="viewType"></param>
    /// <returns></returns>
    public byte[] GetSerializedRenderTexture(int viewType)
    {
        if (viewType == 0)
        {
            return SerializeObject<SerializableColor[]>(GetRenderTexturePixels(fpSrc));
        }
        else
        {
            return SerializeObject<SerializableColor[]>(GetRenderTexturePixels(tpSrc));
        }        
    }

    static byte[] SerializeObject<T>(T objectToSerialize)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream memStr = new MemoryStream();

        bf.Serialize(memStr, objectToSerialize);
        memStr.Position = 0;

        return memStr.ToArray();
    }

    static T DeserializeObject<T>(byte[] dataStream)
    {
        MemoryStream stream = new MemoryStream(dataStream);
        stream.Position = 0;
        BinaryFormatter bf = new BinaryFormatter();
        bf.Binder = new VersionFixer();
        T retV = (T)bf.Deserialize(stream);
        return retV;
    }

    sealed class VersionFixer : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            // For each assemblyName/typeName that you want to deserialize to
            // a different type, set typeToDeserialize to the desired type.
            String assemVer1 = Assembly.GetExecutingAssembly().FullName;

            if (assemblyName != assemVer1)
            {
                // To use a type from a different assembly version, 
                // change the version number.
                // To do this, uncomment the following line of code.
                assemblyName = assemVer1;

                // To use a different type from the same assembly, 
                // change the type name.
            }

            // The following line of code returns the type.
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

            return typeToDeserialize;
        }

    }
}
