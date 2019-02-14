using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ControlPoint : MonoBehaviour {
    public bool selected;

    public Material selectedMaterial;
    public Material unselectedMaterial;

    MeshRenderer meshRenderer;

    void Start () {
        meshRenderer = GetComponent<MeshRenderer>();
	}
	
	void Update () {
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();

        if (selected)
        {
            meshRenderer.sharedMaterial = selectedMaterial;
        }
        else
        {
            meshRenderer.sharedMaterial = unselectedMaterial;
        }


	}


}
