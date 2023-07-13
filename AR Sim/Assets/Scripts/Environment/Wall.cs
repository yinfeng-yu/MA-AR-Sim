using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HERATouch.RoomCreator
{
    public enum WallType
    {
        Normal,
        WithWindow,
        WithDoor,
    }

    // [ExecuteInEditMode]
    public class Wall : MonoBehaviour
    {
        public float width = 2f;
        public float height = 0.5f;
        public float thickness = 0.1f;

        public bool withDoor = false;
        public WallType wallType = WallType.Normal;

        public GameObject fullWall;
        public GameObject wallWithDoorOrWindow;

        public GameObject leftWall;
        public GameObject rightWall;
        public GameObject upperWall;
        public GameObject bottomWall;

        public Door door;

        // Start is called before the first frame update
        void Start()
        {
            // AddDoor(1.4f, 0.15f, 0.3f);
        }

        // Update is called once per frame
        void Update()
        {
            
            UpdateDoorOrWindow();
        }

        public void AddDoor(float a_doorPos, float a_doorWidth, float a_doorHeight)
        {
            if (wallType != WallType.Normal)
            {
                Debug.Log("Cannot have one more door in this wall!");
            }
            else
            {
                wallType = WallType.WithDoor;

                // Switch the walls
                fullWall.SetActive(false);
                wallWithDoorOrWindow.SetActive(true);
                bottomWall.SetActive(false);

                door = new Door(a_doorPos, a_doorWidth, a_doorHeight);

                float lw = a_doorPos - (a_doorWidth / 2);
                float rw = width - a_doorPos - (a_doorWidth / 2);
                float uh = height - a_doorHeight;

                leftWall.transform.localScale = new Vector3(lw, height, thickness);
                rightWall.transform.localScale = new Vector3(rw, height, thickness);
                upperWall.transform.localScale = new Vector3(a_doorWidth, uh, thickness);

                leftWall.transform.localPosition = new Vector3(-(lw + a_doorWidth) / 2 - (width / 2 - a_doorPos), height / 2, 0);
                rightWall.transform.localPosition = new Vector3((rw + a_doorWidth) / 2 - (width / 2 - a_doorPos), height / 2, 0);
                upperWall.transform.localPosition = new Vector3(-(width / 2 - a_doorPos), a_doorHeight + uh / 2, 0);
            }
        }

        // public void AddWindow(float a_windowPos, float a_windowWidth, float a_windowHeight)
        // {
        //     if (wallType != WallType.Normal)
        //     {
        //         Debug.Log("Cannot have one more window in this wall!");
        //     }
        //     else
        //     {
        //         wallType = WallType.WithWindow;
        // 
        //         // Switch the walls
        //         fullWall.SetActive(false);
        //         wallWithDoorOrWindow.SetActive(true);
        //         bottomWall.SetActive(true);
        // 
        //         door = new Window(a_doorPos, a_doorWidth, a_doorHeight);
        // 
        //         float lw = a_doorPos - (a_doorWidth / 2);
        //         float rw = width - a_doorPos - (a_doorWidth / 2);
        //         float uh = height - a_doorHeight;
        //         float bh =
        // 
        //         leftWall.transform.localScale = new Vector3(lw, height, thickness);
        //         rightWall.transform.localScale = new Vector3(rw, height, thickness);
        //         upperWall.transform.localScale = new Vector3(a_doorWidth, uh, thickness);
        // 
        //         leftWall.transform.localPosition = new Vector3(-(lw + a_doorWidth) / 2 - (width / 2 - a_doorPos), height / 2, 0);
        //         rightWall.transform.localPosition = new Vector3((rw + a_doorWidth) / 2 - (width / 2 - a_doorPos), height / 2, 0);
        //         upperWall.transform.localPosition = new Vector3(-(width / 2 - a_doorPos), a_doorHeight + uh / 2, 0);
        //     }
        // }

        void UpdateDoorOrWindow()
        {
            if (wallType == WallType.WithDoor)
            {
                fullWall.SetActive(false);
                wallWithDoorOrWindow.SetActive(true);
                bottomWall.SetActive(false);

                float lw = door.position - (door.width / 2);
                float rw = width - door.position - (door.width / 2);
                float uh = height - door.height;

                leftWall.transform.localScale = new Vector3(lw, height, thickness);
                rightWall.transform.localScale = new Vector3(rw, height, thickness);
                upperWall.transform.localScale = new Vector3(door.width, uh, thickness);

                leftWall.transform.localPosition = new Vector3(-(lw + door.width) / 2 - (width / 2 - door.position), height / 2, 0);
                rightWall.transform.localPosition = new Vector3((rw + door.width) / 2 - (width / 2 - door.position), height / 2, 0);
                upperWall.transform.localPosition = new Vector3(-(width / 2 - door.position), door.height + uh / 2, 0);
            }
            else if (wallType == WallType.WithWindow)
            {

            }
            else
            {
                fullWall.SetActive(true);
                wallWithDoorOrWindow.SetActive(false);
                bottomWall.SetActive(false);

                fullWall.transform.localPosition = new Vector3(fullWall.transform.localPosition.x, height / 2, fullWall.transform.localPosition.z);
                fullWall.transform.localScale = new Vector3(width, height, thickness);
            }
        }
    }

}
