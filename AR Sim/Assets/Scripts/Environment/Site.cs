using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Site : MonoBehaviour
{
    public SiteEnum siteEnum;

    private void Start()
    {
        SiteManager.instance.AddSite(siteEnum.ToString(), transform);
    }
}
