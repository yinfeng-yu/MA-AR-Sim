using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiteManager : MonoBehaviour
{
    #region Singleton
    public static SiteManager instance;
    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }

        m_siteTransforms = new Dictionary<string, Transform>();

    }
    #endregion

    private Dictionary<string, Transform> m_siteTransforms;

    public void AddSite(string a_siteEnumStr, Transform a_transform)
    {
        m_siteTransforms.Add(a_siteEnumStr, a_transform);
    }

    public Transform GetTransform(string a_siteEnumStr)
    {
        return m_siteTransforms[a_siteEnumStr];
    }
}
