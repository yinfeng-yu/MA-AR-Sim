using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] float _gridSize = 3.5f; // Default

    [SerializeField] Transform _floorParent;
    [SerializeField] GameObject _floorPrefab;


    [SerializeField] Transform _gridVisuals;
    [SerializeField] GameObject _pivotPrefab;
    [SerializeField] GameObject _edgePrefab;

    public void GenerateGrid()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                var pgo = Instantiate(_pivotPrefab, _gridVisuals);
                pgo.transform.position = new Vector3(i * _gridSize, 0, j * _gridSize);
                pgo.transform.position += (Vector3.right * _gridSize * 0.5f) + (Vector3.forward * _gridSize * 0.5f);

                if (i == 0)
                {
                    var ego = Instantiate(_edgePrefab, _gridVisuals);
                    ego.GetComponent<EdgeHandle>().pivotPos = pgo.transform.position;
                    ego.GetComponent<EdgeHandle>().index = 3;

                    SetUpEdgeHandleTransform(ego);
                }

                if (j == 0)
                {
                    var ego = Instantiate(_edgePrefab, _gridVisuals);
                    _edgePrefab.GetComponent<EdgeHandle>().pivotPos = pgo.transform.position;
                    _edgePrefab.GetComponent<EdgeHandle>().index = 0;

                    SetUpEdgeHandleTransform(ego);
                }

                for (int k = 0; k < 2; k++)
                {
                    var ego = Instantiate(_edgePrefab, _gridVisuals);
                    _edgePrefab.GetComponent<EdgeHandle>().pivotPos = pgo.transform.position;
                    _edgePrefab.GetComponent<EdgeHandle>().index = k + 1;

                    SetUpEdgeHandleTransform(ego);
                }
                
            }
        }
    }

    void SetUpEdgeHandleTransform(GameObject ego)
    {
        Vector3[] edgeOrientations = { Vector3.forward, Vector3.left, Vector3.back, Vector3.right };

        Vector3 pivotPos = ego.GetComponent<EdgeHandle>().pivotPos;
        int index = ego.GetComponent<EdgeHandle>().index;

        ego.transform.position = pivotPos + (-edgeOrientations[index] * _gridSize * 0.5f);
        ego.transform.forward = edgeOrientations[index];
    }

    public void ClearGrid()
    {
        foreach (Transform t in _gridVisuals)
        {
            DestroyImmediate(t.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ClearGrid();
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
