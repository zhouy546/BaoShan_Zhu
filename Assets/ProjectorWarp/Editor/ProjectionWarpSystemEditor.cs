using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Text;


[CustomEditor(typeof(ProjectionWarpSystem))]
[CanEditMultipleObjects]
public class ProjectionWarpSystemEditor : Editor
{
    const int MAX_CAMERAS = 8;
    ProjectionWarpSystem myScript;
    public bool showReferenceGameObjects = false;
    public bool showDebug = false;
    public bool showStats = false;

    ProjectionWarpSystem.CameraArragement prevCameraArrangement;
    
    Vector2 prevProjectorResolution;
    int prevXDivisions;
    int prevYDivisions;
    int prevCameraCount;
    float prevNear;
    float prevFar;
    float prevOverlap;
    float prevProjectionCameraSpace;
    float prevFieldOfView;
    
    void OnEnable()
    {
        myScript = (ProjectionWarpSystem)target;
    }

    public void Refresh()
    {
        //determine if critical things are changed
        if (myScript.cameraCount > MAX_CAMERAS)
            myScript.cameraCount = MAX_CAMERAS;

        //target camera count is different from current list of cameras
        if (myScript.cameraCount != myScript.panoCamerasController.childCount)
        {
            myScript.DestroyCameras();
            myScript.InitCameras();
        }
        //texture size changed
        if (myScript.projectionCameras.Count > 0)
        {
            if (Mathf.RoundToInt(myScript.renderTextureSize.x) != Mathf.RoundToInt(myScript.projectionCameras[0].width * 100f) ||
                Mathf.RoundToInt(myScript.renderTextureSize.y) != Mathf.RoundToInt(myScript.projectionCameras[0].height * 100f))
            {
                myScript.DestroyCameras();
                myScript.InitCameras();
            }
        }
    }
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        serializedObject.Update();

        //EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Projection Settings", EditorStyles.boldLabel);
        ProjectionWarpSystem.CameraArragement cameraArrangement = (ProjectionWarpSystem.CameraArragement)EditorGUILayout.EnumPopup("Arrangement", myScript.arrangement);

        myScript.arrangement = cameraArrangement;
        myScript.uiToggleKey = (KeyCode)EditorGUILayout.EnumPopup("UI Toggle Key", myScript.uiToggleKey);
        
        Vector2 projectorResolution = EditorGUILayout.Vector2Field("Projector Resolution", myScript.renderTextureSize);
        if (projectorResolution.x < 1f) projectorResolution.x = 1f;
        else if (projectorResolution.x > 16384f) projectorResolution.x = 16384f;
        if (projectorResolution.y < 1f) projectorResolution.y = 1f;
        else if (projectorResolution.y > 16384f) projectorResolution.y = 16384f;

        myScript.renderTextureSize = projectorResolution;
        myScript.viewportSize = projectorResolution.y / 200f;
        
        int xDivisions = EditorGUILayout.IntField("X Divisions", myScript.xDivisions);
        xDivisions = Mathf.Clamp(xDivisions, 1, 100);
        myScript.xDivisions = xDivisions;

        int yDivisions = EditorGUILayout.IntField("Y Divisions", myScript.yDivisions);
        yDivisions = Mathf.Clamp(yDivisions, 1, 100);
        myScript.yDivisions = yDivisions;

        int cameraCount = EditorGUILayout.IntField("Camera Count", myScript.cameraCount);
        cameraCount = Mathf.Clamp(cameraCount, 1, 8);
        myScript.cameraCount = cameraCount;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Distances", EditorStyles.boldLabel);
        
        float near = EditorGUILayout.FloatField("Near", myScript.near);
        if (near <= 0f) near = 0.001f;
        myScript.near = near;

        float far = EditorGUILayout.FloatField("Far", myScript.far);
        if (far <= near) far = near + 0.001f;
        myScript.far = far;

        float overlap = 0f;

        float projectionCameraSpace = 0f;

        if (myScript.cameraCount > 1)
        {
            switch (cameraArrangement)
            {
                case ProjectionWarpSystem.CameraArragement.HORIZONTAL_ORTHOGRAPHIC:
                    overlap = EditorGUILayout.FloatField("Overlap", myScript.overlap.x);
                    overlap = Mathf.Clamp(overlap, 0f, projectorResolution.x / 100f);
                    myScript.overlap = new Vector2(overlap, 0f);
                    break;
                case ProjectionWarpSystem.CameraArragement.VERTICAL_ORTHOGRAPHIC:
                    overlap = EditorGUILayout.FloatField("Overlap", myScript.overlap.y);
                    overlap = Mathf.Clamp(overlap, 0f, projectorResolution.y / 100f);
                    myScript.overlap = new Vector2(0f, overlap);
                    break;
                case ProjectionWarpSystem.CameraArragement.HORIZONTAL_PERSPECTIVE:
                    overlap = EditorGUILayout.FloatField("Overlap", myScript.overlap.x);
                    overlap = Mathf.Clamp(overlap, 0f, myScript.fieldOfView - (myScript.fieldOfView / myScript.panoCameras.Count));
                    myScript.overlap = new Vector2(overlap, 0f);
                    break;
                case ProjectionWarpSystem.CameraArragement.VERTICAL_PERSPECTIVE:
                    overlap = EditorGUILayout.FloatField("Overlap", myScript.overlap.y);
                    overlap = Mathf.Clamp(overlap, 0f, myScript.fieldOfView - (myScript.fieldOfView / myScript.panoCameras.Count));
                    myScript.overlap = new Vector2(0f, overlap);
                    break;
                default:
                    break;
            }

            projectionCameraSpace = EditorGUILayout.FloatField("Projection Camera Space", myScript.projectionCameraSpace);
            if (projectionCameraSpace < 0) projectionCameraSpace = 0f;
            myScript.projectionCameraSpace = projectionCameraSpace;
        }
        else
        {
            myScript.overlap = Vector2.zero;
        }


        float fieldOfView = 0f;

        switch (cameraArrangement)
        {
            case ProjectionWarpSystem.CameraArragement.HORIZONTAL_PERSPECTIVE:       
            case ProjectionWarpSystem.CameraArragement.VERTICAL_PERSPECTIVE:
                fieldOfView = EditorGUILayout.FloatField("Field Of View", myScript.fieldOfView);
                myScript.fieldOfView = fieldOfView;
                break;
            default:
                break;
        }
        

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("File IO", EditorStyles.boldLabel);
        myScript.defaultCalibrationFile = EditorGUILayout.TextField("Startup Calibration File", myScript.defaultCalibrationFile);
        EditorGUILayout.BeginHorizontal ();

        if(GUILayout.Button("Load Calibration"))
        {
            string [] filters = {"JSON Calibration File","json"};
            string path = EditorUtility.OpenFilePanelWithFilters("Load Calibration","",filters);
            myScript.LoadCalibration(path);
        }

        if(GUILayout.Button("Save Calibration"))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Calibration",
                "projector_calibration.json",
                "json",
                "Enter a filename for the calibration file");
            myScript.SaveCalibration(path);

            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
        foldoutStyle.fontStyle = FontStyle.Bold;
        showDebug = EditorGUILayout.Foldout(showDebug, "Debug",foldoutStyle);
        if (showDebug)
        {
            myScript.disableMouseCursor = EditorGUILayout.Toggle("Disable Mouse Cursor", myScript.disableMouseCursor);
            myScript.showProjectionWarpGUI = EditorGUILayout.Toggle("Show Projection Warp GUI", myScript.showProjectionWarpGUI);
            EditorGUILayout.Space();
        }

        EditorGUILayout.Space();

        showReferenceGameObjects = EditorGUILayout.Foldout(showReferenceGameObjects, "Reference Game Objects",foldoutStyle);
        if (showReferenceGameObjects)
        {
            myScript.panoCamerasController = (Transform)EditorGUILayout.ObjectField("Pano Cameras Controller", myScript.panoCamerasController, typeof(Transform), true);
            myScript.projectionCamerasController = (Transform)EditorGUILayout.ObjectField("Projection Cameras Controller", myScript.projectionCamerasController, typeof(Transform), true);
            myScript.projectionUIController = (RectTransform)EditorGUILayout.ObjectField("Projection UI Controller", myScript.projectionUIController, typeof(RectTransform), true);
            myScript.calibrationCanvas = (Canvas)EditorGUILayout.ObjectField("Calibration Canvas", myScript.calibrationCanvas, typeof(Canvas), true);
            myScript.fileIOContainer = (GameObject)EditorGUILayout.ObjectField("File IO UI", myScript.fileIOContainer, typeof(GameObject), true);
            myScript.filename = (InputField)EditorGUILayout.ObjectField("Filename Input", myScript.filename, typeof(InputField), true);
        }
        
        /*
        EditorGUILayout.Space();
        showStats = EditorGUILayout.Foldout(showStats, "Stats");
        if(showStats){
            string stats = "";
            stats+="Vertex Total: "+"\n";
            stats+="Triangle Total: "+"\n";
            EditorGUILayout.TextArea(stats);
        }
        */
/*
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myScript, "Changed Camera Count");
            //Refresh();

        }
        */
        if (GUI.changed)
        {
            string[] res = UnityStats.screenRes.Split('x');
            
            if(int.Parse(res[0]) != (int)projectorResolution.x || 
                int.Parse(res[1]) != (int)projectorResolution.y)
            {
                Debug.LogWarning("One of your Game windows set at "+ 
                    int.Parse(res[0]) +"x"+ int.Parse(res[1]) + 
                    " does not match the specified projector resolution " + 
                    (int)projectorResolution.x + "x" + (int)projectorResolution.y + 
                    ". Please update your Game window resolution or adjust your desired projector resolution to match.");
            }
                        
            //determine if critical things are changed
            if (myScript.cameraCount > MAX_CAMERAS)
                myScript.cameraCount = MAX_CAMERAS;

            bool rebuildCameras = false;

            //camera arrangement changed
            if (prevCameraArrangement != cameraArrangement)
            {
                rebuildCameras = true;
            }

            //projection camera space changed
            if (prevProjectorResolution != projectorResolution)
            {
                rebuildCameras = true;
            }

            //X/Y divisions changed
            if (prevXDivisions != xDivisions ||
                prevYDivisions != yDivisions)
            {
                rebuildCameras = true;
            }
            
            //Camera count changed
            if (prevCameraCount != cameraCount)
            {
                rebuildCameras = true;
            }

            //near/far changed
            if (prevNear != near ||
                prevFar != far)
            {
                rebuildCameras = true;
            }

            //overlap changed
            if (prevOverlap != overlap)
            {
                rebuildCameras = true;
            }

            //projection camera space changed
            if (prevProjectionCameraSpace != projectionCameraSpace)
            {
                rebuildCameras = true;
            }

            //field of view changed
            if (prevFieldOfView != fieldOfView)
            {
                rebuildCameras = true;
            }

            if (rebuildCameras)
            {
                myScript.DestroyCameras();
                myScript.InitCameras();
            }

            myScript.UpdateProjectionWarpGUI();
            //EditorUtility.SetDirty(myScript);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            

        }

        prevCameraArrangement = cameraArrangement;
        prevProjectorResolution = projectorResolution;
        prevXDivisions = xDivisions;
        prevYDivisions = yDivisions;
        prevCameraCount = cameraCount;
        prevNear = near;
        prevFar = far;
        prevOverlap = overlap;
        prevProjectionCameraSpace = projectionCameraSpace;
        prevFieldOfView = fieldOfView;
        
        serializedObject.ApplyModifiedProperties();
    }
}
