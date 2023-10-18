using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Door
{
    public float position;
    public float width;
    public float height;

    public Door(float a_position, float a_width, float a_height)
    {
        position = a_position;
        width = a_width;
        height = a_height;
    }
}

[Serializable]
public class Window
{
    public float position;
    public float width;
    public float height;

    public Window(float a_position, float a_width, float a_height)
    {
        position = a_position;
        width = a_width;
        height = a_height;
    }
}


