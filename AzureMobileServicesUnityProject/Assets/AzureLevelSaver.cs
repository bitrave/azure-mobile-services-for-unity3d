using System.Collections.Generic;
using Bitrave.Azure;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections;

public class AzureLevelSaver : MonoBehaviour
{

    void SaveLevel()
    {
        var azure = new AzureMobileServices(_azureEndPoint, _applicationKey);
        var data = new LevelSaveData() { SaveData = SavePositions(), Id = 1 };
        azure.Update<LevelSaveData>(data);
    }

    void LoadLevel()
    {
        var azure = new AzureMobileServices(_azureEndPoint, _applicationKey);
        
        azure.Lookup<LevelSaveData>(1, azureResponse =>
        {
            if (azureResponse.Status == AzureResponseStatus.Success)
            {
                var data = azureResponse.ResponseData;
                var cubeData = JsonConvert.DeserializeObject<List<Vector3>>(data.SaveData);
                for (var i = 0; i < cubeData.Count && i < cubes.Count; i++)
                {
                    cubes[i].position = cubeData[i];
                }
            }
        });
    }

    // Use this for initialization
    void Start()
    {

    }

    private Ray ray;
    private bool dragEnable;
    private float distance;
    private GameObject dragObject;

    public void OnGUI()
    {
        var screenScale = Screen.width / 820.0f;
        var scaledMatrix = Matrix4x4.Scale(new Vector3(screenScale, screenScale, screenScale));
        GUI.matrix = scaledMatrix;

        GUILayout.BeginVertical();
        GUILayout.MaxWidth(300);
        GUILayout.Width(300);


        SaveLoadCubes();


        GUILayout.EndVertical();
    }



    string SavePositions()
    {
        var list = new List<Vector3>();
        foreach (var t in cubes)
        {
            if (t == null)
                break;
            list.Add(t.position);
        }
        var s = JsonConvert.SerializeObject(list, Formatting.Indented,
            new JsonSerializerSettings { ContractResolver = new AzureMobileServicesUI.ShouldSerializeContractResolver() });
        return s;
    }

    void LoadPositions()
    {
        try
        {
            LoadLevel();
        }
        catch (System.Exception ex)
        {

            string s91113 = "[\r\n  {\r\n    \"x\": 1032.202,\r\n    \"y\": 32.82989,\r\n    \"z\": 1314.85547,\r\n    \"magnitude\": 1671.93408,\r\n    \"sqrMagnitude\": 2795363.75\r\n  },\r\n  {\r\n    \"x\": 1032.202,\r\n    \"y\": 32.82989,\r\n    \"z\": 1314.85547,\r\n    \"magnitude\": 1671.93408,\r\n    \"sqrMagnitude\": 2795363.75\r\n  },\r\n  {\r\n    \"x\": 1032.202,\r\n    \"y\": 32.82989,\r\n    \"z\": 1314.85547,\r\n    \"magnitude\": 1671.93408,\r\n    \"sqrMagnitude\": 2795363.75\r\n  },\r\n  {\r\n    \"x\": 1034.47632,\r\n    \"y\": 32.8721848,\r\n    \"z\": 1310.3761,\r\n    \"magnitude\": 1669.82251,\r\n    \"sqrMagnitude\": 2788307.25\r\n  },\r\n  {\r\n    \"x\": 1029.17468,\r\n    \"y\": 35.4239,\r\n    \"z\": 1311.92139,\r\n    \"magnitude\": 1667.81079,\r\n    \"sqrMagnitude\": 2781593.0\r\n  },\r\n  {\r\n    \"x\": 1032.75891,\r\n    \"y\": 35.2984238,\r\n    \"z\": 1310.73193,\r\n    \"magnitude\": 1669.08813,\r\n    \"sqrMagnitude\": 2785855.25\r\n  }\r\n]";
            var cubeData = JsonConvert.DeserializeObject<List<Vector3>>(s91113);
            for (var i = 0; i < cubeData.Count && i < cubes.Count; i++)
            {
                cubes[i].position = cubeData[i];
            }
        }
    }

    private class LevelSaveData
    {
        public int? Id { get; set; }
        public string SaveData { get; set; }
    }
    public List<Transform> cubes;
    private void SaveLoadCubes()
    {
        GUILayout.BeginArea(new Rect(650, 10, 150, 300));


        if (GUILayout.Button("Save Layout", GUILayout.Width(150), GUILayout.Height(80)))
        {
            SaveLevel();
        }
        if (GUILayout.Button("Load Layout", GUILayout.Width(150), GUILayout.Height(80)))
        {
            LoadPositions();
        }


        GUILayout.EndArea();
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            TouchPhase phase = TouchPhase.Moved;
            Vector2 position;

            if (Input.touchCount > 0)
            {
                phase = Input.GetTouch(0).phase;
                position = Input.GetTouch(0).position;
            }
            else
            {
                position = Input.mousePosition;
            }

            switch (phase)
            {
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    {
                        dragEnable = false;
                        break;
                    }
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                case TouchPhase.Began:
                    {
                        ray = Camera.main.ScreenPointToRay(position);
                        RaycastHit hit;
                        LayerMask mask = 1 << 8;

                        if (!dragEnable && Physics.Raycast(ray, out hit, 1000f, mask))
                        {
                            dragEnable = true;
                            distance = hit.distance;
                            dragObject = hit.collider.gameObject;
                        }
                        if (dragEnable)
                        {
                            Ray newRay = Camera.main.ScreenPointToRay(position);
                            newRay.origin = Camera.main.transform.position;
                            var pos = newRay.GetPoint(10f);
                            dragObject.transform.position = pos;
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragEnable = false;
        }
    }

    private Vector3 screenPoint;
    private Vector3 offset;
    private string _azureEndPoint = "http://bitraveservices.azure-mobile.net/";
    private string _applicationKey = "ePYhutMVmiUPWhFAJaRYTJsPFTiuAB20";

}
