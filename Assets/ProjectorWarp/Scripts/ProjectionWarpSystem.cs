using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Xml;
using SimpleJSON;

[ExecuteInEditMode]
public class ProjectionWarpSystem : MonoBehaviour {
    public enum CameraArragement {
        HORIZONTAL_ORTHOGRAPHIC = 1,
        VERTICAL_ORTHOGRAPHIC = 2,
        HORIZONTAL_PERSPECTIVE = 3,
        VERTICAL_PERSPECTIVE = 4
    }

    [Header("Projection Settings")]
    public string defaultCalibrationFile;
    public CameraArragement arrangement;
    public Vector2 renderTextureSize;
    public int xDivisions;
    public int yDivisions;
    public int cameraCount;

    [Header("Debug")]
    public bool disableMouseCursor;   
    public bool showProjectionWarpGUI;

    [Header("Reference Game Objects")]
    public Transform projectionCamerasController;
    public Transform panoCamerasController;
    public RectTransform projectionUIController;
    public Canvas calibrationCanvas;
    public GameObject fileIOContainer;
    
    public List<Material> fadeMaterials;

    [Header("Cameras & UI")]
    public KeyCode uiToggleKey;
    public List<Camera> panoCameras;
    public List<ProjectionMesh> projectionCameras;
    public List<ProjectionUI> projectionUIs;
    public List<int> targetDisplays;

    [Header("Distances")]
    public Vector2 overlap;
    public float viewportSize;
    public float near;
    public float far;
    public float fieldOfView;

    float aspectRatio;

    public float projectionCameraSpace;

    [Header("File IO")]
    public string saveCalibrationFile;

    public InputField filename;


    public void UpdateFilename(){
        saveCalibrationFile = filename.text;
    }

    void OnEnable(){
        UpdateCursor();
        UpdateProjectionWarpGUI();

        if (filename != null)
        {
            filename.onEndEdit.AddListener(val =>
                {
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                    {
                        saveCalibrationFile = filename.text;
                    }
                });
        }

        if (cameraCount != panoCameras.Count)
        {
            DestroyCameras();
            InitCameras();

        }
        LinkUI();
    }

    public void AssignReferences()
    {
        for (int i = 0; i < cameraCount; i++)
        {
            if (panoCameras[i] == null || projectionCameras[i]==null) continue;
            projectionCameras[i].meshRenderer.sharedMaterial.mainTexture = panoCameras[i].targetTexture;
        }
    }
    void Start()
    {
        if (defaultCalibrationFile.Length > 0)
        {
            LoadCalibration(defaultCalibrationFile);
        }
        else if (File.Exists(Application.dataPath + "/../default_calibration.json"))
        {
        

            LoadCalibration(Application.streamingAssetsPath + "/default_calibration.json");
        }
        AssignReferences();

    }

 
    void UpdateCursor(){
        Cursor.visible = !disableMouseCursor;
    }

    public void UpdateProjectionWarpGUI(){
        
        for (int i = 0; i < projectionCameras.Count; i++)
        {
            if (projectionCameras[i] == null || projectionUIs[i] == null) continue;
            projectionCameras[i].showControlPoints = showProjectionWarpGUI;
            projectionUIs[i].gameObject.SetActive(showProjectionWarpGUI);
        }
        fileIOContainer.SetActive(showProjectionWarpGUI);
    }
    
    void Update()
    {
#if UNITY_EDITOR
        AssignReferences();
#endif
        aspectRatio = (float)Screen.width / (float)Screen.height;

        if (cameraCount != panoCamerasController.childCount)
        {
            DestroyCameras();
            InitCameras();
            LinkUI();
        }

        if (Input.GetKeyDown(uiToggleKey))
        {
            showProjectionWarpGUI = !showProjectionWarpGUI;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray;
            RaycastHit hitInfo;
            for (int i = 0; i < projectionCameras.Count; i++)
            {
                ray = projectionCameras[i].targetCamera.ScreenPointToRay(Input.mousePosition);
                hitInfo = new RaycastHit();
                
                if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
                {
                    int editVertexIndex = -1;
                    if (hitInfo.collider.name.Substring(0, 3) == "Top")
                    {
                        //top collider
                        editVertexIndex = int.Parse(hitInfo.collider.name.Substring(11, hitInfo.collider.name.Length-11));
                    }
                    else
                    {
                        //bottom collider
                        editVertexIndex = (xDivisions+1) + int.Parse(hitInfo.collider.name.Substring(14, hitInfo.collider.name.Length-14));
                    }
                    projectionCameras[i].SetEditVertex(editVertexIndex);
                    projectionUIs[i].controlPointIndexSlider.value = editVertexIndex;
                    projectionCameras[i].OffsetRefresh();

                }
            }
        }

        UpdateCursor();
        UpdateProjectionWarpGUI();
        UpdatePanoCameras();
        UpdateProjectionCameras();
     
    }

    void UpdateProjectionCameras()
    {
        
        for (int i = 0; i < projectionCameras.Count; i++)
        {
            if (projectionCameras[i] == null) continue;
            projectionCameras[i].transform.parent.localPosition = new Vector3((float)i * (projectionCameras[i].width + projectionCameraSpace), 0f, 0f);
            projectionCameras[i].width = renderTextureSize.x/100f;
            projectionCameras[i].height = renderTextureSize.y/100f;
            projectionCameras[i].xDivisions = xDivisions;
            projectionCameras[i].yDivisions = yDivisions;
            projectionCameras[i].targetCamera.orthographicSize = viewportSize;
        }
    
    }

    public float HorizontalToVerticalFOV(float hFov)
    {
        float hFovRad = hFov * Mathf.Deg2Rad;
        float vFovRad = Mathf.Atan(Mathf.Tan(hFovRad * 0.5f) / aspectRatio) * 2f;
        float vFov = vFovRad * Mathf.Rad2Deg;
        return vFov;

    }
    void UpdatePanoCameras(){
        
        float viewportHeight = viewportSize * 2;
        float viewportWidth = viewportHeight * aspectRatio;
        
        float singleFieldOfViewH;
        float singleFieldOfViewV;
        float startAngle;
        float compressedArcAngle;

        switch (arrangement)
        {
            case CameraArragement.HORIZONTAL_ORTHOGRAPHIC:
            case CameraArragement.VERTICAL_ORTHOGRAPHIC:
                for (int i = 0; i < panoCameras.Count; i++)
                {
                    
                    panoCameras[i].nearClipPlane = near;
                    panoCameras[i].farClipPlane = far;

                    //ortho cameras will be locked into target resolution
                    panoCameras[i].orthographic = true;
                    panoCameras[i].orthographicSize = viewportSize;
                }
                break;

            case CameraArragement.HORIZONTAL_PERSPECTIVE:


                break;
            case CameraArragement.VERTICAL_PERSPECTIVE:


                break;
            default:
                break;
        }
        
        switch (arrangement)
        {
            case CameraArragement.HORIZONTAL_ORTHOGRAPHIC:
                
                float startX = (-(panoCameras.Count / 2f) * viewportWidth) + (viewportWidth / 2f) + ((overlap.x/2f) * (panoCameras.Count-1));
                float offsetX = 0f;
                for (int i = 0; i < panoCameras.Count; i++)
                {
                    offsetX = startX + (i * viewportWidth) - (i * overlap.x);
                    panoCameras[i].transform.localPosition = new Vector3(
                        offsetX + projectionCameras[i].referenceCameraOffset.x, 
                        projectionCameras[i].referenceCameraOffset.y, 
                        0);
                    panoCameras[i].transform.localEulerAngles = Vector3.zero;
                }
                break;
            case CameraArragement.VERTICAL_ORTHOGRAPHIC:
                
                float startY = (-(panoCameras.Count / 2f) * viewportHeight) + (viewportHeight / 2f) + ((overlap.y/2f)*(panoCameras.Count-1));
                float offsetY = 0f;
                for (int i = 0; i < panoCameras.Count; i++)
                {
                    offsetY = startY + (i * viewportHeight) - (i * overlap.y);
                    panoCameras[i].transform.localPosition = new Vector3(
                        projectionCameras[i].referenceCameraOffset.x, 
                        offsetY+projectionCameras[i].referenceCameraOffset.y, 
                        0);
                    panoCameras[i].transform.localEulerAngles = Vector3.zero;
                }
                break;
            case ProjectionWarpSystem.CameraArragement.HORIZONTAL_PERSPECTIVE:
                singleFieldOfViewH = (fieldOfView / panoCameras.Count) + overlap.x;
                singleFieldOfViewV = HorizontalToVerticalFOV(singleFieldOfViewH);

                startAngle = -(fieldOfView / 2f) + (singleFieldOfViewH / 2f) ;
                compressedArcAngle = -startAngle * 2f;

                for (int i = 0; i < panoCameras.Count; i++)
                {
                    if (panoCameras[i] == null) continue;
                    panoCameras[i].nearClipPlane = near;
                    panoCameras[i].farClipPlane = far;

                    //calculate from field of view total
                    panoCameras[i].orthographic = false;
                    panoCameras[i].fieldOfView = singleFieldOfViewV;

                    panoCameras[i].transform.localPosition = Vector3.zero;
                    
                    if (cameraCount > 1)
                    {
                        panoCameras[i].transform.localEulerAngles = new Vector3(0, startAngle + (i * (compressedArcAngle / (cameraCount - 1))), 0);
                    }
                    else {
                        panoCameras[i].transform.localEulerAngles = new Vector3(0, startAngle, 0);
                    }
                    

                }
                break;
            case ProjectionWarpSystem.CameraArragement.VERTICAL_PERSPECTIVE:
                singleFieldOfViewV = (fieldOfView / panoCameras.Count) + overlap.y;
                startAngle = -(fieldOfView / 2f) + (singleFieldOfViewV / 2f);
                compressedArcAngle = -startAngle * 2f;

                for (int i = 0; i < panoCameras.Count; i++)
                {
                    panoCameras[i].nearClipPlane = near;
                    panoCameras[i].farClipPlane = far;

                    //calculate from field of view total
                    panoCameras[i].orthographic = false;
                    panoCameras[i].fieldOfView = singleFieldOfViewV;

                    panoCameras[i].transform.localPosition = Vector3.zero;
                    if (cameraCount > 1)
                    {
                        panoCameras[i].transform.localEulerAngles = new Vector3(startAngle + (i * (compressedArcAngle / (cameraCount - 1))), 0, 0);
                    }
                    else
                    {
                        panoCameras[i].transform.localEulerAngles = new Vector3(startAngle, 0, 0);
                    }
                }
                break;
            default:
                break;
        }
    }


    public void DestroyCameras(){

        int count;

        count = projectionUIController.childCount;
        for(int i=0;i<count;i++)
        {
            DestroyImmediate(projectionUIController.GetChild(0).gameObject);
        }

        projectionUIs = new List<ProjectionUI>();

        count = panoCamerasController.childCount;
        for(int i=0;i<count;i++)
        {
            DestroyImmediate(panoCamerasController.GetChild(0).gameObject);
        }
        panoCameras = new List<Camera>();

        targetDisplays = new List<int>();

        count = projectionCamerasController.childCount;
        for(int i=0;i<count;i++)
        {
            DestroyImmediate(projectionCamerasController.GetChild(0).gameObject);
        }
        projectionCameras = new List<ProjectionMesh>();
    }

    public void LinkUI()
    {
        for (int i = 0; i < cameraCount; i++)
        {
            projectionUIs[i].LinkUI();
        }

        //align UI to first display
        if(projectionCameras.Count>0){
            calibrationCanvas.worldCamera = projectionCameras[0].targetCamera;
        }

    }

    public void InitCameras()
    {
        GameObject ui;
        RectTransform uiRectTransform;
        ProjectionUI projectionUI;
        
        //build UI Cameras first for reference
        for (int i = 0; i < cameraCount; i++)
        {
            ui = Instantiate(Resources.Load("Prefabs/Projection UI", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
            uiRectTransform = ui.GetComponent<RectTransform>();
            projectionUI = ui.GetComponent<ProjectionUI>();
            projectionUIs.Add(projectionUI);
            ui.name = "Projection UI " + (i + 1);
            projectionUI.displayIDLabel.text = "Display "+(i);
            ui.transform.SetParent(projectionUIController);
            uiRectTransform.anchoredPosition3D = Vector3.zero;
            uiRectTransform.localScale = Vector3.one;
        }

        //build source render texture cameras array
        GameObject panoCamera;
        Camera camera;
        RenderTexture renderTexture;

        for (int i = 0; i < cameraCount; i++)
        {
            panoCamera = Instantiate(Resources.Load("Prefabs/Pano Camera", typeof(GameObject))) as GameObject;
            camera = panoCamera.GetComponent<Camera>();
            panoCameras.Add(camera);
            panoCamera.name = "Pano Camera " + (i + 1);
            panoCamera.transform.SetParent(panoCamerasController);
            renderTexture = new RenderTexture((int)renderTextureSize.x, (int)renderTextureSize.y, 24, RenderTextureFormat.ARGB32);
            renderTexture.antiAliasing = 8;
            renderTexture.Create();
            targetDisplays.Add(i);
            camera.targetTexture = renderTexture;
            camera.targetDisplay = targetDisplays[i];
        }

        //build final render cameras
        GameObject projectionCamera;
        ProjectionMesh projectionMesh;

        for (int i = 0; i < cameraCount; i++)
        {
            projectionCamera = Instantiate(Resources.Load("Prefabs/Projection Camera", typeof(GameObject))) as GameObject;
            projectionMesh = projectionCamera.transform.GetChild(0).GetComponent<ProjectionMesh>();

            projectionCameras.Add(projectionMesh);
            projectionCamera.name = "Projection Camera " + (i + 1);
            projectionCamera.transform.SetParent(projectionCamerasController);
            projectionCamera.transform.localPosition = new Vector3((float)i * (projectionMesh.width + projectionCameraSpace), 0f, 0f);
            projectionMesh.projectionUI = projectionUIs[i];
            
            projectionMesh.xDivisions = xDivisions;
            projectionMesh.yDivisions = yDivisions;
            projectionMesh.prevXDivision = xDivisions;
            projectionMesh.prevYDivision = yDivisions;
            projectionMesh.width = renderTextureSize.x / 100f;
            projectionMesh.height = renderTextureSize.y / 100f;
            projectionMesh.topOffset = new Vector2[xDivisions + 1];
            projectionMesh.bottomOffset = new Vector2[xDivisions + 1];
            projectionCamera.GetComponent<Camera>().targetDisplay = targetDisplays[i];

            projectionMesh.CreateMesh();
                        
            Material projectionImage = Instantiate(Resources.Load("Materials/Projection Image", typeof(Material))) as Material;
            projectionImage.name = "Projection Image " + (i + 1);
            projectionImage.mainTexture = panoCameras[i].targetTexture;

            projectionMesh.meshRenderer.material = projectionImage;
                        
        }

        //create mutual link
        for (int i = 0; i < cameraCount; i++)
        {
            projectionUIs[i].referenceCamera = projectionCameras[i];
        }
        
    }


    public void SaveCalibrationUsingInput(GameObject input){
        SaveCalibration(input.GetComponent<InputField>().text);
    }
    public void SaveCalibration(string path)
    {
        if (path == null || path.Length == 0) return;
//        Debug.Log(Application.dataPath+"/"+path);
        string json = "";
        json += "{";
        json += "\"Version\": \""+"2.0.0"+"\",";
        json += "\"Arrangement\":" + (int)arrangement + ",";
        json += "\"TextureWidth\":" + (int)renderTextureSize.x + ",";
        json += "\"TextureHeight\":" + (int)renderTextureSize.y + ",";
        json += "\"XDivisions\":" + xDivisions + ",";
        json += "\"YDivisions\":" + yDivisions + ",";
        json += "\"OverlapX\":" + overlap.x + ",";
        json += "\"OverlapY\":" + overlap.y + ",";
        json += "\"ViewportSize\":" + viewportSize + ",";
        json += "\"FieldOfView\":" + fieldOfView + ",";
        json += "\"Near\":" + near + ",";
        json += "\"Far\":" + far + ",";
        json += "\"Spacing\":" + projectionCameraSpace + ",";
        json += "\"Cameras\":";
        
        json += "[";
        
        for (int i = 0; i < cameraCount; i++)
        {
            json += "{";
            json += "\"OffsetX\":" + projectionCameras[i].referenceCameraOffset.x + ",";
            json += "\"OffsetY\":" + projectionCameras[i].referenceCameraOffset.y + ",";
            json += "\"LeftFadeRange\":" + projectionCameras[i].leftFadeRange + ",";
            json += "\"LeftFadeChoke\":" + projectionCameras[i].leftFadeChoke + ",";
            json += "\"RightFadeRange\":" + projectionCameras[i].rightFadeRange + ",";
            json += "\"RightFadeChoke\":" + projectionCameras[i].rightFadeChoke + ",";

            json += "\"TopFadeRange\":" + projectionCameras[i].topFadeRange + ",";
            json += "\"TopFadeChoke\":" + projectionCameras[i].topFadeChoke + ",";
            json += "\"BottomFadeRange\":" + projectionCameras[i].bottomFadeRange + ",";
            json += "\"BottomFadeChoke\":" + projectionCameras[i].bottomFadeChoke + ",";

            json += "\"TrapezoidAnchor\":" + (int)projectionCameras[i].trapezoidAnchor + ",";
            json += "\"TrapezoidAngle\":" + projectionCameras[i].trapezoidAngle + ",";
            json += "\"SkewAnchor\":" + (int)projectionCameras[i].skewAnchor + ",";
            json += "\"SkewAngle\":" + projectionCameras[i].skewAngle + ",";
            json += "\"TopOffset\":";
            json += "[";
            for (int j = 0; j < projectionCameras[i].topOffset.Length; j++)
            {
                json+="{";
                json+="\"x\":"+ projectionCameras[i].topOffset[j].x+",";
                json+="\"y\":"+ projectionCameras[i].topOffset[j].y;
                json+="}";
                if(j< projectionCameras[i].topOffset.Length-1) json+=",";
            }
            json +="]";
            json+=",";
            json += "\"BottomOffset\":";
            json += "[";
            for (int j = 0; j < projectionCameras[i].bottomOffset.Length; j++)
            {
                json+="{";
                json+="\"x\":"+ projectionCameras[i].bottomOffset[j].x+",";
                json+="\"y\":"+ projectionCameras[i].bottomOffset[j].y;
                json+="}";
                if(j < projectionCameras[i].bottomOffset.Length-1) json+=",";
            }
            json +="]";


            json += "}";
            if(i<cameraCount-1) json+=",";
        }
        json+="]";
        json+="}";
        var sr = File.CreateText(Application.streamingAssetsPath + "/" + path);
        sr.WriteLine(json);
        sr.Close();


    }
    public void LoadCalibration(string path)
    {
        if (path == null || path.Length == 0) return;
        
        string json = "";
        try
        {
            string line;

            StreamReader theReader = new StreamReader(path, Encoding.Default);
            using (theReader)
            {

                do
                {
                    line = theReader.ReadLine();
                     
                    if (line != null)
                    {
                        json += line;
                    }
                }
                while (line != null);
                theReader.Close();

            }
        }
        catch (Exception e)
        {
            //Console.WriteLine("{0}\n", e.Message);
            Debug.Log(e.Message);
            return;

        }
        var N = JSON.Parse(json);
        fieldOfView = N["FieldOfView"].AsFloat;
        cameraCount = N["Cameras"].Count;
        renderTextureSize = new Vector2(N["TextureWidth"].AsInt, N["TextureHeight"].AsInt);
        xDivisions = N["XDivisions"].AsInt;
        yDivisions = N["YDivisions"].AsInt;
        arrangement = (CameraArragement)N["Arrangement"].AsInt;
        overlap = new Vector2(N["OverlapX"].AsFloat, N["OverlapY"].AsFloat);
        viewportSize = N["ViewportSize"].AsFloat;
        near = N["Near"].AsFloat;
        far = N["Far"].AsFloat;
        projectionCameraSpace = N["Spacing"].AsFloat;

        DestroyCameras();
        InitCameras();


        for (int i = 0; i < cameraCount; i++)
        {
            ProjectionMesh projectionMesh = projectionCameras[i];

            projectionMesh.referenceCameraOffset.x = N["Cameras"][i]["OffsetX"].AsFloat;
            projectionMesh.referenceCameraOffset.y = N["Cameras"][i]["OffsetY"].AsFloat;

            projectionMesh.leftFadeRange = N["Cameras"][i]["LeftFadeRange"].AsFloat;
            projectionMesh.leftFadeChoke = N["Cameras"][i]["LeftFadeChoke"].AsFloat;
            projectionMesh.rightFadeRange = N["Cameras"][i]["RightFadeRange"].AsFloat;
            projectionMesh.rightFadeChoke = N["Cameras"][i]["RightFadeChoke"].AsFloat;

            projectionMesh.topFadeRange = N["Cameras"][i]["TopFadeRange"].AsFloat;
            projectionMesh.topFadeChoke = N["Cameras"][i]["TopFadeChoke"].AsFloat;
            projectionMesh.bottomFadeRange = N["Cameras"][i]["BottomFadeRange"].AsFloat;
            projectionMesh.bottomFadeChoke = N["Cameras"][i]["BottomFadeChoke"].AsFloat;


            projectionMesh.trapezoidAnchor = (ProjectionMesh.AnchorPosition)N["Cameras"][i]["TrapezoidAnchor"].AsInt;
            projectionMesh.trapezoidAngle = N["Cameras"][i]["TrapezoidAngle"].AsFloat;
            projectionMesh.skewAnchor = (ProjectionMesh.AnchorPosition)N["Cameras"][i]["SkewAnchor"].AsInt;
            projectionMesh.skewAngle = N["Cameras"][i]["SkewAngle"].AsFloat;

            for(int j=0;j<(xDivisions+1);j++){
                projectionMesh.topOffset[j] = new Vector2(N["Cameras"][i]["TopOffset"][j]["x"].AsFloat,N["Cameras"][i]["TopOffset"][j]["y"].AsFloat);
                projectionMesh.bottomOffset[j] = new Vector2(N["Cameras"][i]["BottomOffset"][j]["x"].AsFloat,N["Cameras"][i]["BottomOffset"][j]["y"].AsFloat);
            }

            projectionMesh.CreateMesh();

            projectionMesh.BlendRefresh();
            projectionMesh.OffsetRefresh();
            projectionMesh.UpdateUI();
        }

        LinkUI();
        defaultCalibrationFile = path;

    }


}
