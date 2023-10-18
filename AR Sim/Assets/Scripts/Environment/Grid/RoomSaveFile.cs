using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomSaveFile
{
    [Serializable]
    public struct WallData
    {
        public Vector3 position;
        public Vector3 forward;
        public int type;
    }

    public List<WallData> walls;
    public int width;
    public int height;
    public float gridSize;
}
