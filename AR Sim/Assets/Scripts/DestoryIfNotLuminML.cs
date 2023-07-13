using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryIfNotLuminML : MonoBehaviour
{
    
#if PLATFORM_LUMIN

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

#endif
    
}
