using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProjectionUI : MonoBehaviour {
    public ProjectionMesh referenceCamera;

    public Text displayIDLabel;

    [Header("Reference Camera UI Controllers")]
    public InputField referenceCameraOffsetXInput;
    public Slider referenceCameraOffsetXSlider;
    public InputField referenceCameraOffsetYInput;
    public Slider referenceCameraOffsetYSlider;

    [Header("Control Point UI Controllers")]
    public InputField controlPointIndexInput;
    public Slider controlPointIndexSlider;
    

    [Header("Offset UI Controllers")]
    public InputField offsetXInput;
    public Slider offsetXSlider;
    public InputField offsetYInput;
    public Slider offsetYSlider;

    [Header("Fade UI Controllers")]
    public InputField topFadeRangeInput;
    public InputField topFadeChokeInput;
    public InputField bottomFadeRangeInput;
    public InputField bottomFadeChokeInput;
    public InputField leftFadeRangeInput;
    public InputField leftFadeChokeInput;
    public InputField rightFadeRangeInput;
    public InputField rightFadeChokeInput;

    public void LinkUI(){
        if (referenceCamera == null)
        {
            Debug.Log("MISSING REFERENCE CAMERA ON PROJECTION UI");
            return;
        }

        #region Control Point Input Callback

        controlPointIndexSlider.onValueChanged.RemoveAllListeners();
        controlPointIndexSlider.onValueChanged.AddListener(    
            delegate {
            referenceCamera.ControlPointSliderUpdate();
        }
        );

        if (controlPointIndexInput != null)
        {
            controlPointIndexInput.onEndEdit.RemoveAllListeners();
            controlPointIndexInput.onEndEdit.AddListener(val =>
                {
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                    {
                        referenceCamera.SetEditVertex(int.Parse(controlPointIndexInput.text));
                        controlPointIndexSlider.value = referenceCamera.editVertexIndex;
                        referenceCamera.OffsetRefresh();
                    }
                });
        }
        #endregion

        #region Offset Input Callbacks
        offsetXSlider.onValueChanged.RemoveAllListeners();
        offsetXSlider.onValueChanged.AddListener(val => 
            {
                referenceCamera.OffsetXSliderUpdate();
            });
        offsetYSlider.onValueChanged.RemoveAllListeners();
        offsetYSlider.onValueChanged.AddListener(val => 
            {
                referenceCamera.OffsetYSliderUpdate();
            });

        offsetXInput.onEndEdit.RemoveAllListeners();
        offsetXInput.onEndEdit.AddListener(val =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateOffset();
                }
            });
        offsetYInput.onEndEdit.RemoveAllListeners();
        offsetYInput.onEndEdit.AddListener(val =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateOffset();
                }
            });
        #endregion

        #region Reference Camera Callbacks

        referenceCameraOffsetXSlider.onValueChanged.RemoveAllListeners();
        referenceCameraOffsetXSlider.onValueChanged.AddListener(val=>
            {
                referenceCamera.ReferenceCameraOffsetSliderUpdate();
            });
        referenceCameraOffsetYSlider.onValueChanged.RemoveAllListeners();
        referenceCameraOffsetYSlider.onValueChanged.AddListener(val=>
            {
                referenceCamera.ReferenceCameraOffsetSliderUpdate();
            });
        referenceCameraOffsetXInput.onEndEdit.RemoveAllListeners();
        referenceCameraOffsetXInput.onEndEdit.AddListener(val =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateCameraOffset();
                }
            });
        referenceCameraOffsetYInput.onEndEdit.RemoveAllListeners();
        referenceCameraOffsetYInput.onEndEdit.AddListener(val =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateCameraOffset();
                }
            });

        #endregion

        #region Fade Input Callbacks
        topFadeRangeInput.onEndEdit.RemoveAllListeners();
        topFadeRangeInput.onEndEdit.AddListener(val =>
            {  
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateBlend();
                }
            });
        topFadeChokeInput.onEndEdit.RemoveAllListeners();
        topFadeChokeInput.onEndEdit.AddListener(val => 
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateBlend();
                }
            });
        bottomFadeRangeInput.onEndEdit.RemoveAllListeners();
        bottomFadeRangeInput.onEndEdit.AddListener(val =>  
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateBlend();
                }
            });
        bottomFadeChokeInput.onEndEdit.RemoveAllListeners();
        bottomFadeChokeInput.onEndEdit.AddListener(val => 
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateBlend();
                }
            });
        leftFadeRangeInput.onEndEdit.RemoveAllListeners();
        leftFadeRangeInput.onEndEdit.AddListener(val =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateBlend();
                }
            });
        leftFadeChokeInput.onEndEdit.RemoveAllListeners();
        leftFadeChokeInput.onEndEdit.AddListener(val =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateBlend();
                }
            });
        rightFadeRangeInput.onEndEdit.RemoveAllListeners();
        rightFadeRangeInput.onEndEdit.AddListener(val =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateBlend();
                }
            });
        rightFadeChokeInput.onEndEdit.RemoveAllListeners();
        rightFadeChokeInput.onEndEdit.AddListener(val =>    
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    referenceCamera.UpdateBlend();
                }
            });

        #endregion

    }

    void Start () {
        LinkUI();
	}
	
	void Update () {
	
	}
}
