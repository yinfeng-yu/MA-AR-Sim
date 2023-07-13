using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    #region Singleton
    public static NotificationManager instance;
    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NotifyArrived(string a_siteEnumStr)
    {
        TransportManager.instance.SendArrivedNotification(a_siteEnumStr);
    }
}
