using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboyComponentsAccess : MonoBehaviour
{
    public RoboyHands roboyHands;

    public static RoboyComponentsAccess instance;

    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
