using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transmitter : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        ReceiveMessages();
    }

    public abstract void Initialize();
    public abstract void Send(Message message);
    public abstract void Receive();     
    public abstract void ReceiveMessages();

}
