using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class ProjectionMesh : MonoBehaviour {
    public const int PLANE_DISTANCE = 10;
    public enum ProjectionMode {
        CUSTOM
    }

    public enum AnchorPosition {
        TOP = 0,
        BOTTOM = 1,
        LEFT = 2,
        RIGHT = 3
    }

    public Vector2 referenceCameraOffset;
    public Camera targetCamera;
    public ProjectionUI projectionUI;
    public Transform controlPointsContainer;
    public MeshFilter meshFilter;
    
    
    public ProjectionMode mode;
    public int xDivisions;
    public int yDivisions;

    public float width;
    public float height;

    public float trapezoidAngle;
    public float skewAngle;

    public MeshRenderer meshRenderer;

    public float topFadeRange;
    public float topFadeChoke;
    public float bottomFadeRange;
    public float bottomFadeChoke;
    public float leftFadeRange;
    public float leftFadeChoke;
    public float rightFadeRange;
    public float rightFadeChoke;

    public AnchorPosition trapezoidAnchor;
    public AnchorPosition skewAnchor;

    public Vector2[] topOffset;
    public Vector2[] bottomOffset;

    [Header("Gizmos")]
    public float sphereRadius;

    public Vector3[] originalVertices;
    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector2[] uv;
    public int[] triangles;
    
    public int prevXDivision;
    public int prevYDivision;
    
    public bool showControlPoints;
    public int editVertexIndex;

    void Start() {

		editVertexIndex = 0;
        UpdateUI();
        Material fadeMaterial = meshRenderer.sharedMaterial;

        if (fadeMaterial)
        {
            topFadeRange = fadeMaterial.GetFloat("_TopFadeRange");
            topFadeChoke = fadeMaterial.GetFloat("_TopFadeChoke");
            bottomFadeRange = fadeMaterial.GetFloat("_BottomFadeRange");
            bottomFadeChoke = fadeMaterial.GetFloat("_BottomFadeChoke");
            leftFadeRange = fadeMaterial.GetFloat("_LeftFadeRange");
            leftFadeChoke = fadeMaterial.GetFloat("_LeftFadeChoke");
            rightFadeRange = fadeMaterial.GetFloat("_RightFadeRange");
            rightFadeChoke = fadeMaterial.GetFloat("_RightFadeChoke");
        }

        //CreateMesh();
        Refresh();
    }

    public void SetEditVertex(int index){
        editVertexIndex = index;
        if (editVertexIndex > (xDivisions + 1) * 2 - 1)
        {
            editVertexIndex = (xDivisions + 1) * 2 - 1;
        }
        else if (editVertexIndex < 0)
        {
            editVertexIndex = 0;
        }
    }

    public void UpdateCameraOffset()
    {
        projectionUI.referenceCameraOffsetXSlider.value = float.Parse(projectionUI.referenceCameraOffsetXInput.text);
        projectionUI.referenceCameraOffsetYSlider.value = float.Parse(projectionUI.referenceCameraOffsetYInput.text);
        referenceCameraOffset = new Vector2(projectionUI.referenceCameraOffsetXSlider.value, projectionUI.referenceCameraOffsetYSlider.value);
    }
    public void UpdateOffset()
    {
        projectionUI.offsetXSlider.value = float.Parse(projectionUI.offsetXInput.text);
        projectionUI.offsetYSlider.value = float.Parse(projectionUI.offsetYInput.text);

        CreateMesh();
        OffsetRefresh();
    }

    public void UpdateBlend(){
        Material fadeMaterial = meshRenderer.sharedMaterial;

        if (fadeMaterial)
        {
            topFadeRange = float.Parse(projectionUI.topFadeRangeInput.text);
            Debug.Log("UpdateTopRange" + topFadeRange);
            fadeMaterial.SetFloat("_TopFadeRange", topFadeRange);
            topFadeChoke = float.Parse(projectionUI.topFadeChokeInput.text);
            fadeMaterial.SetFloat("_TopFadeChoke", topFadeChoke);

            bottomFadeRange = float.Parse(projectionUI.bottomFadeRangeInput.text);
            fadeMaterial.SetFloat("_BottomFadeRange", bottomFadeRange);
            bottomFadeChoke = float.Parse(projectionUI.bottomFadeChokeInput.text);
            fadeMaterial.SetFloat("_BottomFadeChoke", bottomFadeChoke);

            leftFadeRange = float.Parse(projectionUI.leftFadeRangeInput.text);
            fadeMaterial.SetFloat("_LeftFadeRange", leftFadeRange);
            leftFadeChoke = float.Parse(projectionUI.leftFadeChokeInput.text);
            fadeMaterial.SetFloat("_LeftFadeChoke", leftFadeChoke);

            rightFadeRange = float.Parse(projectionUI.rightFadeRangeInput.text);
            fadeMaterial.SetFloat("_RightFadeRange", rightFadeRange);
            rightFadeChoke = float.Parse(projectionUI.rightFadeChokeInput.text);
            fadeMaterial.SetFloat("_RightFadeChoke", rightFadeChoke);
        }
    }

    public void BlendRefresh(){
        projectionUI.topFadeRangeInput.text = topFadeRange.ToString();
        projectionUI.topFadeChokeInput.text = topFadeChoke.ToString();
        projectionUI.bottomFadeRangeInput.text = bottomFadeRange.ToString();
        projectionUI.bottomFadeChokeInput.text = bottomFadeChoke.ToString();
        projectionUI.leftFadeRangeInput.text = leftFadeRange.ToString();
        projectionUI.leftFadeChokeInput.text = leftFadeChoke.ToString();
        projectionUI.rightFadeRangeInput.text = rightFadeRange.ToString();
        projectionUI.rightFadeChokeInput.text = rightFadeChoke.ToString();

        UpdateBlend();
    }
    public void OffsetRefresh()
    {

	    foreach(Transform child in controlPointsContainer)
        {
            child.gameObject.GetComponent<ControlPoint>().selected = false;
        }


        if (controlPointsContainer != null && controlPointsContainer.childCount > 0)
        {
            
            if (editVertexIndex < xDivisions + 1)
            {
                if (controlPointsContainer.Find("Top Offset " + editVertexIndex) != null)
                {
                    GameObject selectedPoint = controlPointsContainer.Find("Top Offset " + editVertexIndex).gameObject;
                    selectedPoint.gameObject.GetComponent<ControlPoint>().selected = true;
                }
            }
            else
            {
                int relativeIndex = editVertexIndex - xDivisions - 1;
                if(controlPointsContainer.Find("Bottom Offset " + relativeIndex) != null) {
                    GameObject selectedPoint = controlPointsContainer.Find("Bottom Offset " + relativeIndex).gameObject;
                    selectedPoint.gameObject.GetComponent<ControlPoint>().selected = true;
                }
                
            }
        }
    }
    public void ReferenceCameraOffsetSliderUpdate()
    {
        projectionUI.referenceCameraOffsetXInput.text = projectionUI.referenceCameraOffsetXSlider.value.ToString();
        projectionUI.referenceCameraOffsetYInput.text = projectionUI.referenceCameraOffsetYSlider.value.ToString();

        referenceCameraOffset = new Vector2(projectionUI.referenceCameraOffsetXSlider.value, projectionUI.referenceCameraOffsetYSlider.value);

    }

    public void OffsetXSliderUpdate()
    {
        editVertexIndex = (int)projectionUI.controlPointIndexSlider.value;

        if (controlPointsContainer.childCount > 0)
        {
            if (editVertexIndex < xDivisions + 1)
            {
                topOffset[editVertexIndex].x = projectionUI.offsetXSlider.value;
                projectionUI.offsetXInput.text = topOffset[editVertexIndex].x.ToString();

            }
            else
            {
                int relativeIndex = editVertexIndex - xDivisions - 1;
                bottomOffset[relativeIndex].x = projectionUI.offsetXSlider.value;
                projectionUI.offsetXInput.text = bottomOffset[relativeIndex].x.ToString();
            }
            CreateMesh();
        }
    }
    public void OffsetYSliderUpdate()
    {
        editVertexIndex = (int)projectionUI.controlPointIndexSlider.value;

        if (controlPointsContainer.childCount > 0)
        {
            if (editVertexIndex < xDivisions + 1)
            {
                topOffset[editVertexIndex].y = projectionUI.offsetYSlider.value;
                projectionUI.offsetYInput.text = topOffset[editVertexIndex].y.ToString();

            }
            else
            {
                int relativeIndex = editVertexIndex - xDivisions - 1;
                bottomOffset[relativeIndex].y = projectionUI.offsetYSlider.value;
                projectionUI.offsetYInput.text = bottomOffset[relativeIndex].y.ToString();
            }
            CreateMesh();
        }
    }
    public void ControlPointSliderUpdate()
    {
        
        editVertexIndex = (int)projectionUI.controlPointIndexSlider.value;
        projectionUI.controlPointIndexInput.text = projectionUI.controlPointIndexSlider.value.ToString();
        
        if (editVertexIndex < (xDivisions + 1))
        {
            projectionUI.offsetXSlider.value = topOffset[editVertexIndex].x;
            projectionUI.offsetYSlider.value = topOffset[editVertexIndex].y;
        }
        else
        {
            int relativeIndex = editVertexIndex - xDivisions - 1;
            projectionUI.offsetXSlider.value = bottomOffset[relativeIndex].x;
            projectionUI.offsetYSlider.value = bottomOffset[relativeIndex].y;
        }
        
        OffsetRefresh();
    }

    public void ClearControlPoints()
    {
        int childCount = controlPointsContainer.childCount;

        for (int i = 0; i < childCount; i++)
        {
            DestroyImmediate(controlPointsContainer.GetChild(0).gameObject);
        }

    }
   

    public void CreatecontrolPointsContainer()
    {
        ClearControlPoints();

        if (controlPointsContainer.childCount == 0)
        {
            GameObject controlPointPrefab = Resources.Load("Prefabs/Control Point") as GameObject;
            GameObject controlPoint;
            for (int i = 0; i < xDivisions + 1; i++)
            {
                controlPoint = (GameObject)Instantiate(controlPointPrefab, Vector3.zero, Quaternion.identity);
                controlPoint.transform.SetParent(controlPointsContainer);
                controlPoint.name = "Top Offset " + i;
            }

            for (int i = 0; i < xDivisions + 1; i++)
            {
                controlPoint = (GameObject)Instantiate(controlPointPrefab, Vector3.zero, Quaternion.identity);
                controlPoint.transform.SetParent(controlPointsContainer);
                controlPoint.name = "Bottom Offset " + i;
            }
        }

        Vector3 startPosition = new Vector3(-width / 2f, height / 2f, 0);
        float deltaX = width / (float)(xDivisions);
        for (int i = 0; i < xDivisions + 1; i++)
        {
            Vector3 offset = new Vector3(topOffset[i].x, topOffset[i].y, 0);
            Vector3 offsetX = new Vector3(i * deltaX, 0, 0);
            Transform controlPoint = controlPointsContainer.Find("Top Offset " + i);
            if (controlPointsContainer != null && controlPoint !=null) controlPoint.localPosition = startPosition + offsetX + offset;
        }
        startPosition = new Vector3(-width / 2f, -height / 2f, 0);
        for (int i = 0; i < xDivisions + 1; i++)
        {
            Vector3 offset = new Vector3(bottomOffset[i].x, bottomOffset[i].y, 0);
            Vector3 offsetX = new Vector3(i * deltaX, 0, 0);
            Transform controlPoint = controlPointsContainer.Find("Bottom Offset " + i);
            if (controlPointsContainer != null && controlPoint != null) controlPoint.localPosition = startPosition + offsetX + offset;
        }
    }
    public void CreateMesh()
    {
        int vertexCount = (xDivisions + 1) * (yDivisions + 1);
        vertices = new Vector3[vertexCount];
        originalVertices = new Vector3[vertexCount];

        if (topOffset == null || xDivisions != prevXDivision) { 
            topOffset = new Vector2[xDivisions + 1];
            bottomOffset = new Vector2[xDivisions + 1];
        }


        CreatecontrolPointsContainer();

        switch (mode)
        {
            case ProjectionMode.CUSTOM:
                
                for (int i = 0; i < yDivisions + 1; i++)
                {
                    for (int j = 0; j < xDivisions + 1; j++)
                    {
                        int index = i * (xDivisions + 1) + j;
                        
                        float trapezoidRad = (float)Mathf.PI * trapezoidAngle / 180.0f;
                        float skewRad = (float)Mathf.PI * skewAngle / 180.0f;
                        float xLength = width;
                        float yLength = height;

                        float sx, sy, dx, dy;
                        sx = sy = dx = dy = 0f;
                        float skewX = 0;
                        float skewY = 0;
                        float skewXStep = 0;
                        float skewYStep = 0;

                        switch (trapezoidAnchor)
                        {
                            case AnchorPosition.TOP:
                                xLength = width + (2f * (float)Mathf.Tan(trapezoidRad) * width * (((float)i / (float)yDivisions)));
                                break;
                            case AnchorPosition.BOTTOM:
                                xLength = width + (2f * (float)Mathf.Tan(trapezoidRad) * width * (1f - ((float)i / (float)yDivisions)));
                                break;
                            case AnchorPosition.LEFT:
                                yLength = height + (2f * (float)Mathf.Tan(trapezoidRad) * height * (((float)j / (float)xDivisions)));
                                break;
                            case AnchorPosition.RIGHT:
                                yLength = height + (2f * (float)Mathf.Tan(trapezoidRad) * height * (1f - ((float)j / (float)xDivisions)));
                                break;
                            default:
                                break;
                        }


                        switch (skewAnchor)
                        {
                            case AnchorPosition.TOP:
                                skewX = height * (float)Mathf.Tan(skewRad);
                                skewXStep = (skewX * (float)i/(float)yDivisions);
                                break;
                            case AnchorPosition.BOTTOM:
                                skewX = height * (float)Mathf.Tan(skewRad);
                                skewXStep = (skewX *(1f- ((float)i / (float)yDivisions)));
                                break;
                            case AnchorPosition.LEFT:
                                skewY = width * (float)Mathf.Tan(skewRad);
                                skewYStep = (skewY * (float)j / (float)xDivisions);
                                break;
                            case AnchorPosition.RIGHT:
                                skewY = width * (float)Mathf.Tan(skewRad);
                                skewYStep = (skewY * (1f - ((float)j / (float)xDivisions)));
                                break;
                            default:
                                break;
                        }

                        float xDeltaOffset = topOffset[j].x - bottomOffset[j].x;
                        //float yDeltaOffset = topOffset[j].y - bottomOffset[j].y;

                        sx = (-xLength / 2f) + topOffset[j].x + skewXStep;
                        sy = (yLength / 2f) + topOffset[j].y + skewYStep;
                        dx = (xLength) / (float)xDivisions;
                        dy = (yLength + (topOffset[j].y - bottomOffset[j].y)) / (float)yDivisions;

                        vertices[index] = new Vector3(
                            sx + ((float)j * dx) - (xDeltaOffset * (float)i / (float)yDivisions),
                            sy - ((float)i * dy), 
                            0);

                        originalVertices[index] = new Vector3(
                            (-xLength / 2f) + skewXStep + ((float)j * dx), 
                            (yLength / 2f) + skewYStep - ((float)i * yLength / (float)yDivisions), 
                            0);

                    }
                }
                break;
           
            
            default:
                break;
        }
       
        normals = new Vector3[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            normals[i] = new Vector3(0, 0, -1f);
        }


        
        uv = new Vector2[vertexCount];

        for (int i = 0; i < yDivisions + 1; i++)
        {
            for (int j = 0; j < xDivisions + 1; j++)
            {
                int index = i * (xDivisions + 1) + j;
                float su = 0f;
                float sv = 1f;
                float du = 1f / (float)xDivisions;
                float dv = 1f / (float)yDivisions;

                uv[index] = new Vector3(su + ((float)j * du), sv - ((float)i * dv));
            }
        }
        
        triangles = new int[xDivisions * yDivisions * 2 * 3];
        int triangleIndex = 0;
        for (int i = 0; i < yDivisions; i++)
        {
            for (int j = 0; j < xDivisions; j++)
            {
                int index = i * (xDivisions+1) + j;
                int right = index + 1;
                int bottom = index + (xDivisions+1);
                int opposite = bottom + 1;
                
                triangles[triangleIndex] = index;
                triangleIndex++;
                triangles[triangleIndex] = right;
                triangleIndex++;
                triangles[triangleIndex] = opposite;
                triangleIndex++;
                triangles[triangleIndex] = index;
                triangleIndex++;
                triangles[triangleIndex] = opposite;
                triangleIndex++;
                triangles[triangleIndex] = bottom;
                triangleIndex++;

            }
        }
        

        Mesh newMesh = new Mesh();
        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.uv = uv;
        newMesh.normals = normals;

        meshFilter.mesh = newMesh;

        //transform.position = new Vector3(0, 0, -PLANE_DISTANCE);

        prevXDivision = xDivisions;
        prevYDivision = yDivisions;

    }

    public void UpdateMesh(){
        
        if (meshFilter.sharedMesh != null)
        {
            meshFilter.sharedMesh.vertices = vertices;
            meshFilter.sharedMesh.triangles = triangles;
            meshFilter.sharedMesh.uv = uv;
            meshFilter.sharedMesh.normals = normals;
        }
        /*
        else
        {
            CreateMesh();
        }
        */
        
    }
	public void Refresh()
    {
        projectionUI.controlPointIndexSlider.maxValue = (xDivisions + 1) * 2 - 1;
        projectionUI.controlPointIndexSlider.value = editVertexIndex;
        projectionUI.controlPointIndexInput.text = editVertexIndex.ToString();
        BlendRefresh();
        OffsetRefresh();
    }
    
    public void UpdateUI(){

        projectionUI.referenceCameraOffsetXSlider.value = referenceCameraOffset.x;
        projectionUI.referenceCameraOffsetYSlider.value = referenceCameraOffset.y;
        projectionUI.referenceCameraOffsetXInput.text = referenceCameraOffset.x.ToString();
        projectionUI.referenceCameraOffsetYInput.text = referenceCameraOffset.y.ToString();

        int selectedIndex = editVertexIndex;
        if (selectedIndex < xDivisions + 1)
        {
            projectionUI.offsetXInput.text = topOffset[selectedIndex].x.ToString();
            projectionUI.offsetYInput.text = topOffset[selectedIndex].y.ToString();
            projectionUI.offsetXSlider.value = topOffset[selectedIndex].x;
            projectionUI.offsetYSlider.value = topOffset[selectedIndex].y;
        }
        else
        {
            selectedIndex = editVertexIndex - xDivisions - 1;

            projectionUI.offsetXInput.text = bottomOffset[selectedIndex].x.ToString();
            projectionUI.offsetYInput.text = bottomOffset[selectedIndex].y.ToString();
            projectionUI.offsetXSlider.value = bottomOffset[selectedIndex].x;
            projectionUI.offsetYSlider.value = bottomOffset[selectedIndex].y;
        }


    }
	public void Update()
    {
        
        if (controlPointsContainer.childCount != (xDivisions+1)*2 || yDivisions != prevYDivision)
        {
            ClearControlPoints();
            CreateMesh();
        }


        if (showControlPoints)
        {
            if(controlPointsContainer) controlPointsContainer.gameObject.SetActive(true);
        }
        else
        {
            if (controlPointsContainer) controlPointsContainer.gameObject.SetActive(false);
        }

        UpdateMesh();

        prevXDivision = xDivisions;
        prevYDivision = yDivisions;
	}

    

    void OnDrawGizmos()
    {
        
    }
}
