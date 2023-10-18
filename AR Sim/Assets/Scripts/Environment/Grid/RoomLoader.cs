using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    [SerializeField] Transform _floorParent;
    [SerializeField] GameObject _floorPrefab;

    [SerializeField] Transform _wallParent;
    [SerializeField] GameObject wallPrefab;

    public string saveFileName = "Room";

    public void ClearRoom()
    {
        foreach (Transform t in _floorParent)
        {
            DestroyImmediate(t.gameObject);
        }

        foreach (Transform t in _wallParent)
        {
            DestroyImmediate(t.gameObject);
        }
    }

    public void LoadRoom()
    {
        string localPath = "/Data";
        string fullPath = Application.dataPath + localPath;

        string fullFilepath = fullPath + $"/{saveFileName}";

        if (File.Exists(fullFilepath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(fullFilepath, FileMode.Open);

            string serialized = formatter.Deserialize(stream) as string;
            RoomSaveFile data = JsonUtility.FromJson<RoomSaveFile>(serialized);
            stream.Close();


            int width = data.width;
            int height = data.height;
            float gridSize = data.gridSize;

            // Load Floors
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var go = Instantiate(_floorPrefab, _floorParent);
                    go.transform.localPosition = new Vector3(i * gridSize, 0, j * gridSize) + (Vector3.right * gridSize * 0.5f) + (Vector3.forward * gridSize * 0.5f);
                }
            }

            // Load Walls
            foreach (var wd in data.walls)
            {
                var go = Instantiate(wallPrefab, _wallParent);
                go.GetComponent<Wall>().wallType = (WallType) wd.type;
                go.GetComponent<Wall>().UpdateWall();

                go.transform.localPosition = wd.position;
                go.transform.forward = wd.forward;
            }
        }
        else
        {
            Debug.LogError("Save file not found in" + fullPath);
        }
    }
}
