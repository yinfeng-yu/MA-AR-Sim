using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GridBuildingSystem : MonoBehaviour
{
    [Header("Grid Basics")]
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] float _gridSize = 3.5f; // Default

    [Header("Container References")]
    [SerializeField] Transform _floorParent;
    [SerializeField] Transform _wallParent;

    [Header("Prefabs")]
    [SerializeField] GameObject _floorPrefab;
    [SerializeField] GameObject wallPrefab;


    [Header("Grid Visuals")]
    [SerializeField] Transform _gridVisuals;
    [SerializeField] GameObject _pivotPrefab;
    [SerializeField] GameObject _edgePrefab;

    [Header("Save File")]
    public string saveFileName = "Room";
    public bool forceWrite;

    [SerializeField] RoomSaveFile _saveFile;

    [Header("UI References")]
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] TextMeshProUGUI _chosenWalltext;

    private WallType currentWallType = WallType.Plain;

    public static GridBuildingSystem instance;

    private void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }

    public void GenerateGrid()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                var pgo = Instantiate(_pivotPrefab, _gridVisuals);
                pgo.transform.position = new Vector3(i * _gridSize, 0, j * _gridSize);
                pgo.transform.position += (Vector3.right * _gridSize * 0.5f) + (Vector3.forward * _gridSize * 0.5f);

                var fgo = Instantiate(_floorPrefab, _floorParent);
                fgo.transform.position = pgo.transform.position;

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

        foreach (Transform t in _floorParent)
        {
            DestroyImmediate(t.gameObject);
        }

        foreach (Transform t in _wallParent)
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


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // RoomVertexHandle hoveredVertexHandle = hit.collider.gameObject.GetComponent<RoomVertexHandle>();
            // selectedVertexHandle = hoveredVertexHandle == null ? selectedVertexHandle : hoveredVertexHandle;
            var edgeHandle = hit.collider.gameObject.GetComponent<EdgeHandle>();
            if (edgeHandle != null)
            {
                var pivotPos = edgeHandle.pivotPos - ((Vector3.right * _gridSize * 0.5f) + (Vector3.forward * _gridSize * 0.5f));
                _text.text = $"Grid: ({pivotPos.x / _gridSize}, {pivotPos.z / _gridSize}) \n\n" +
                    $"Index: {edgeHandle.index}";
                    
                if (Input.GetMouseButtonDown(0))
                {
                    var go = Instantiate(wallPrefab, _wallParent);
                    go.GetComponent<Wall>().wallType = currentWallType;
                    go.GetComponent<Wall>().UpdateWall();

                    go.transform.position = hit.collider.transform.position;
                    go.transform.forward = hit.collider.transform.forward;

                }
            }
        }
    }

    public void ChangeWallType(int wallType)
    {
        currentWallType = (WallType) wallType;
    }


    public void SaveRoom()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string localPath = "/Data";
        string fullPath = Application.dataPath + localPath;

        var info = new DirectoryInfo(fullPath);
        var fileInfo = info.GetFiles();

        string finalSaveFileName = saveFileName;

        if (!forceWrite)
        {
            List<string> fileNames = new List<string>();

            foreach (var file in fileInfo)
            {
                fileNames.Add(file.Name);
            }

            int i = 1;
            while (fileNames.Contains(finalSaveFileName))
            {
                finalSaveFileName = saveFileName + $" ({i})";
                i++;
            }
        }

        string fullFilePath = fullPath + $"/{finalSaveFileName}";

        FileStream stream = new FileStream(fullFilePath, FileMode.Create);

        RoomSaveFile saveFile = new RoomSaveFile();

        saveFile.width = _width;
        saveFile.height = _height;
        saveFile.gridSize = _gridSize;
        saveFile.walls = new List<RoomSaveFile.WallData>();

        foreach (Transform w in _wallParent)
        {
            RoomSaveFile.WallData wallData;
            wallData.position = w.position;
            wallData.forward = w.forward;
            wallData.type = (int)w.gameObject.GetComponent<Wall>().wallType;
            saveFile.walls.Add(wallData);
        }

        string serialized = JsonUtility.ToJson(saveFile);
        formatter.Serialize(stream, serialized);
        stream.Close();


        Debug.Log($"Saved to {"Assets" + localPath + $"/{finalSaveFileName}"}!");
    }

}
