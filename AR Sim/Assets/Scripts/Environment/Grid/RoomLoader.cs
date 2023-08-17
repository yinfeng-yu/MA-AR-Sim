using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    [SerializeField] Transform _floorParent;
    [SerializeField] GameObject _floorPrefab;

    [SerializeField] Transform _wallParent;
    [SerializeField] List<GameObject> _wallPrefabs;

    [SerializeField] RoomSaveFile _saveFile;

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
        int width = _saveFile.width;
        int height = _saveFile.height;
        float gridSize = _saveFile.gridSize;


        // Load Floors
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var go = Instantiate(_floorPrefab, _floorParent);
                go.transform.position = new Vector3(i * gridSize, 0, j * gridSize) + (Vector3.right * gridSize * 0.5f) + (Vector3.forward * gridSize * 0.5f);
            }
        }

        // Load Walls
        foreach (var wd in _saveFile.walls)
        {
            var go = Instantiate(_wallPrefabs[wd.type], _wallParent);
            go.transform.position = wd.position;
            go.transform.forward = wd.forward;
        }
    }
}
