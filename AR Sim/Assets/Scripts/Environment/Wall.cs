using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 public enum WallType
{
    Plain,
    Door,
    Window,
}

// [ExecuteInEditMode]
public class Wall : MonoBehaviour
{
    [Header("Wall Basics")]
    public WallType wallType = WallType.Plain;

    public float width = 1f;
    public float height = 2f;
    public float thickness = 0.05f;


    [Header("Door")]
    public float doorWidth = 0.6f;
    public float doorHeight = 1.5f;

    [Header("Window")]
    public float windowWidth = 0.6f;
    public float windowHeight = 0.6f;

    [Header("Baseboard")]
    public float baseboardHeight = 0.15f;
    public float baseboardThickness = 0.1f;

    [Header("Objects References")]
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject upperWall;
    public GameObject bottomWall;

    public GameObject leftBaseboard;
    public GameObject rightBaseboard;

    public void UpdateWall()
    {
        float lw = 0f;
        float rw = 0f;
        float uh = 0f;
        float bh = 0f;

        float hollowPos = 0.5f;
        float hollowWidth = 0f;
        float hollowHeight = 0f;

        switch (wallType)
        {
            case WallType.Plain:
                lw = width / 2;
                rw = width / 2;
                uh = 0f;
                bh = 0f;

                break;
            case WallType.Door:
                hollowWidth = doorWidth;
                hollowHeight = doorHeight;

                lw = hollowPos - (hollowWidth / 2);
                rw = width - hollowPos - (hollowWidth / 2);
                uh = height - hollowHeight;
                bh = 0f;

                break;
            case WallType.Window:
                hollowWidth = windowWidth;
                hollowHeight = windowHeight;

                lw = hollowPos - (hollowWidth / 2);
                rw = width - hollowPos - (hollowWidth / 2);
                uh = (height - hollowHeight) / 2;
                bh = (height - hollowHeight) / 2;

                break;
            default:
                break;
        }

        leftWall.transform.localScale = new Vector3(lw, height, thickness);
        rightWall.transform.localScale = new Vector3(rw, height, thickness);
        upperWall.transform.localScale = new Vector3(hollowWidth, uh, thickness);
        bottomWall.transform.localScale = new Vector3(hollowWidth, bh, thickness);

        leftBaseboard.transform.localScale = new Vector3(lw, baseboardHeight, baseboardThickness);
        rightBaseboard.transform.localScale = new Vector3(rw, baseboardHeight, baseboardThickness);

        leftWall.transform.localPosition = new Vector3(-(lw + hollowWidth) / 2 - (width / 2 - hollowPos), height / 2, 0);
        rightWall.transform.localPosition = new Vector3((rw + hollowWidth) / 2 - (width / 2 - hollowPos), height / 2, 0);
        upperWall.transform.localPosition = new Vector3(-(width / 2 - hollowPos), hollowHeight + bh + uh / 2, 0);
        bottomWall.transform.localPosition = new Vector3(-(width / 2 - hollowPos), bh / 2, 0);

        leftBaseboard.transform.localPosition = new Vector3(-(lw + hollowWidth) / 2 - (width / 2 - hollowPos), baseboardHeight / 2, 0);
        rightBaseboard.transform.localPosition = new Vector3((rw + hollowWidth) / 2 - (width / 2 - hollowPos), baseboardHeight / 2, 0);
    }

    public void ResetWall()
    {
        width = 1f;
        height = 2f;
        thickness = 0.05f;
        
        doorWidth = 0.6f;
        doorHeight = 1.5f;

        windowWidth = 0.6f;
        windowHeight = 0.6f;
    
        UpdateWall();
    }

    
}


