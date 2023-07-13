using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] float _spacing = 3.5f; // Default

    [SerializeField] Transform _floorParent;
    [SerializeField] GameObject _floorPrefab;

    public void GenerateFloor()
    {
        for (int i = 0; i < _width; i ++)
        {
            for (int j = 0; j < _height; j ++)
            {
                var go = Instantiate(_floorPrefab, _floorParent);
                go.transform.position = new Vector3(i * _spacing, 0, j * _spacing);
            }
        }
    }

    public void ClearFloor()
    {
        foreach (Transform f in _floorParent)
        {
            DestroyImmediate(f.gameObject);
        }
    }
}
