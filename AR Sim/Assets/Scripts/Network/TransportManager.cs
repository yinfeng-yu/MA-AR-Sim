using UnityEngine;

public class TransportManager : MonoBehaviour
{
    #region Singleton
        public static TransportManager instance;
        private void Awake()
        {
            if (instance != this)
            {
                instance = this;
            }
        }
    #endregion

    private UDPSend m_udpSend;
    private UDPReceive m_udpReceive;

    private void Start()
    {
        m_udpSend = GetComponent<UDPSend>();
        m_udpReceive = GetComponent<UDPReceive>();
    }

    public void SendPosition(Vector3 a_position)
    {
        string msg = "[P] " + a_position.ToString();
        m_udpSend.SendString(msg);
    }

    public void SendArrivedNotification(string a_siteEnumStr)
    {
        string msg = "[AN] " + a_siteEnumStr;
        m_udpSend.SendString(msg);
    }    
}


