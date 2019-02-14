using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Text;

[CustomEditor(typeof(ProjectionMesh))]
[CanEditMultipleObjects]
public class ProjectionMeshEditor : Editor
{
    ProjectionMesh myScript;
    public bool showReferenceGameObjects = false;
    
    void OnEnable()
    {
        myScript = (ProjectionMesh)target;
    }

    public override void OnInspectorGUI()
    {

        //DrawDefaultInspector();

        serializedObject.Update();
        myScript = (ProjectionMesh)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Reference Camera", EditorStyles.boldLabel);

        myScript.referenceCameraOffset = EditorGUILayout.Vector2Field("Pano Camera Offset", myScript.referenceCameraOffset);
       
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Blending", EditorStyles.boldLabel);
        
        float topFadeRange = EditorGUILayout.FloatField("Top Fade Range", myScript.topFadeRange);
        topFadeRange = Mathf.Clamp(topFadeRange, 0f, 1f);
        myScript.topFadeRange = topFadeRange;
        
        float topFadeChoke = EditorGUILayout.FloatField("Top Fade Choke", myScript.topFadeChoke);
        topFadeChoke = Mathf.Clamp(topFadeChoke, 0f, 0.999f);
        myScript.topFadeChoke = topFadeChoke;

        float bottomFadeRange = EditorGUILayout.FloatField("Bottom Fade Range", myScript.bottomFadeRange); ;
        bottomFadeRange = Mathf.Clamp(bottomFadeRange, 0f, 1f);
        myScript.bottomFadeRange = bottomFadeRange;

        float bottomFadeChoke = EditorGUILayout.FloatField("Bottom Fade Choke", myScript.bottomFadeChoke);
        bottomFadeChoke = Mathf.Clamp(bottomFadeChoke, 0f, 0.999f);
        myScript.bottomFadeChoke = bottomFadeChoke;

        float leftFadeRange = EditorGUILayout.FloatField("Left Fade Range", myScript.leftFadeRange);
        leftFadeRange = Mathf.Clamp(leftFadeRange, 0f, 1f);
        myScript.leftFadeRange = leftFadeRange;

        float leftFadeChoke = EditorGUILayout.FloatField("Left Fade Choke", myScript.leftFadeChoke);
        leftFadeChoke = Mathf.Clamp(leftFadeChoke, 0f, 0.999f);
        myScript.leftFadeChoke = leftFadeChoke;

        float rightFadeRange = EditorGUILayout.FloatField("Right Fade Range", myScript.rightFadeRange);
        rightFadeRange = Mathf.Clamp(rightFadeRange, 0f, 1f);
        myScript.rightFadeRange = rightFadeRange;

        float rightFadeChoke = EditorGUILayout.FloatField("Right Fade Choke", myScript.rightFadeChoke);
        rightFadeChoke = Mathf.Clamp(rightFadeChoke, 0f, 0.999f);
        myScript.rightFadeChoke = rightFadeChoke;
        EditorGUILayout.Space();

      

        EditorGUILayout.LabelField("Custom Options", EditorStyles.boldLabel);
        switch (myScript.mode)
        {
            case ProjectionMesh.ProjectionMode.CUSTOM:

                myScript.trapezoidAngle = EditorGUILayout.FloatField("Trapezoid Angle", myScript.trapezoidAngle);
                myScript.trapezoidAnchor = (ProjectionMesh.AnchorPosition)EditorGUILayout.EnumPopup("Trapezoid Anchor", myScript.trapezoidAnchor);

                myScript.skewAngle = EditorGUILayout.FloatField("Skew Angle", myScript.skewAngle);
                myScript.skewAnchor = (ProjectionMesh.AnchorPosition)EditorGUILayout.EnumPopup("Skew Anchor", myScript.skewAnchor);

                if (myScript.topOffset == null || 
                    myScript.prevXDivision != myScript.xDivisions || 
                    myScript.prevYDivision != myScript.yDivisions)
                {
                    
                    myScript.topOffset = new Vector2[myScript.xDivisions + 1];
                    myScript.bottomOffset = new Vector2[myScript.xDivisions + 1];

                    for (int i = 0; i < myScript.xDivisions + 1; i++)
                    {
                        myScript.topOffset[i] = Vector2.zero;
                        myScript.bottomOffset[i] = Vector2.zero;
                    }
                }


                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Top Offset", EditorStyles.boldLabel);
                for (int i = 0; i < myScript.xDivisions + 1; i++)
                {
                    myScript.topOffset[i] = EditorGUILayout.Vector2Field(i + ":", myScript.topOffset[i]);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Bottom Offset", EditorStyles.boldLabel);
                for (int i = 0; i < myScript.xDivisions + 1; i++)
                {
                    myScript.bottomOffset[i] = EditorGUILayout.Vector2Field(i + ":", myScript.bottomOffset[i]);
                }
                EditorGUILayout.EndVertical();

                myScript.prevXDivision = myScript.xDivisions;
                myScript.prevYDivision = myScript.yDivisions;
                break;
            default:
                break;
        }


        EditorGUILayout.Space();
        GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
        foldoutStyle.fontStyle = FontStyle.Bold;
        showReferenceGameObjects = EditorGUILayout.Foldout(showReferenceGameObjects, "Reference Game Objects", foldoutStyle);
        if (showReferenceGameObjects)
        {
            myScript.targetCamera = (Camera)EditorGUILayout.ObjectField("Target Camera", myScript.targetCamera, typeof(Camera), true);
            myScript.projectionUI = (ProjectionUI)EditorGUILayout.ObjectField("Calibration UI", myScript.projectionUI, typeof(ProjectionUI), true);
            myScript.controlPointsContainer = (Transform)EditorGUILayout.ObjectField("Control Points Container", myScript.controlPointsContainer, typeof(Transform), true);
            myScript.meshFilter = (MeshFilter)EditorGUILayout.ObjectField("Mesh Filter", myScript.meshFilter, typeof(MeshFilter), true);
        }
        
        
        if (GUI.changed)
        {
            
            if (myScript.editVertexIndex < 0)
            {
                myScript.editVertexIndex = 0;
            }
            else if (myScript.editVertexIndex > (myScript.xDivisions + 1) * 2 - 1)
            {
                myScript.editVertexIndex = (myScript.xDivisions + 1) * 2 - 1;
            }
            myScript.ClearControlPoints();
            myScript.CreateMesh();
            myScript.BlendRefresh();

            //EditorUtility.SetDirty(myScript);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        serializedObject.ApplyModifiedProperties();
    }
}
