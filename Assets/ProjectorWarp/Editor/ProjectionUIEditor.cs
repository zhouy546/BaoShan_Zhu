using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

using System.Text;

[CustomEditor(typeof(ProjectionUI))]
[CanEditMultipleObjects]
public class ProjectionUIEditor : Editor
{
    ProjectionUI myScript;
    
    void OnEnable()
    {
        myScript = (ProjectionUI)target;
    }
    public void OnSceneGUI()
    {
        myScript = (ProjectionUI)target;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        myScript = (ProjectionUI)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Names", EditorStyles.boldLabel);
        myScript.referenceCamera = (ProjectionMesh)EditorGUILayout.ObjectField("Reference Camera", myScript.referenceCamera, typeof(ProjectionMesh), true);
        myScript.displayIDLabel = (Text)EditorGUILayout.ObjectField("Display Label", myScript.displayIDLabel, typeof(Text), true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Reference Camera UI", EditorStyles.boldLabel);
        myScript.referenceCameraOffsetXInput = (InputField)EditorGUILayout.ObjectField("RC Offset X", myScript.referenceCameraOffsetXInput, typeof(InputField), true);
        myScript.referenceCameraOffsetXSlider = (Slider)EditorGUILayout.ObjectField("RC Offset X Slider", myScript.referenceCameraOffsetXSlider, typeof(Slider), true);
        myScript.referenceCameraOffsetYInput = (InputField)EditorGUILayout.ObjectField("RC Offset Y", myScript.referenceCameraOffsetYInput, typeof(InputField), true);
        myScript.referenceCameraOffsetYSlider = (Slider)EditorGUILayout.ObjectField("RC Offset Y Slider", myScript.referenceCameraOffsetYSlider, typeof(Slider), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Control Point UI", EditorStyles.boldLabel);
        myScript.controlPointIndexInput = (InputField)EditorGUILayout.ObjectField("CP Index", myScript.controlPointIndexInput, typeof(InputField), true);
        myScript.controlPointIndexSlider = (Slider)EditorGUILayout.ObjectField("CP Index Slider", myScript.controlPointIndexSlider, typeof(Slider), true);
        
        myScript.offsetXInput = (InputField)EditorGUILayout.ObjectField("CP Offset X", myScript.offsetXInput, typeof(InputField), true);
        myScript.offsetXSlider = (Slider)EditorGUILayout.ObjectField("CP Offset X Slider", myScript.offsetXSlider, typeof(Slider), true);
        myScript.offsetYInput = (InputField)EditorGUILayout.ObjectField("CP Offset Y", myScript.offsetYInput, typeof(InputField), true);
        myScript.offsetYSlider = (Slider)EditorGUILayout.ObjectField("CP Offset Y Slider", myScript.offsetYSlider, typeof(Slider), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Fade Adjustment UI", EditorStyles.boldLabel);
        myScript.topFadeRangeInput = (InputField)EditorGUILayout.ObjectField("Top Fade Range Input", myScript.topFadeRangeInput, typeof(InputField), true);
        myScript.topFadeChokeInput = (InputField)EditorGUILayout.ObjectField("Top Fade Choke Input", myScript.topFadeChokeInput, typeof(InputField), true);
        myScript.bottomFadeRangeInput = (InputField)EditorGUILayout.ObjectField("Bottom Fade Range Input", myScript.bottomFadeRangeInput, typeof(InputField), true);
        myScript.bottomFadeChokeInput = (InputField)EditorGUILayout.ObjectField("Bottom Fade Choke Input", myScript.bottomFadeChokeInput, typeof(InputField), true);
        myScript.leftFadeRangeInput = (InputField)EditorGUILayout.ObjectField("Left Fade Range Input", myScript.leftFadeRangeInput, typeof(InputField), true);
        myScript.leftFadeChokeInput = (InputField)EditorGUILayout.ObjectField("Left Fade Choke Input", myScript.leftFadeChokeInput, typeof(InputField), true);
        myScript.rightFadeRangeInput = (InputField)EditorGUILayout.ObjectField("Right Fade Range Input", myScript.rightFadeRangeInput, typeof(InputField), true);
        myScript.rightFadeChokeInput = (InputField)EditorGUILayout.ObjectField("Right Fade Choke Input", myScript.rightFadeChokeInput, typeof(InputField), true);
      

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}