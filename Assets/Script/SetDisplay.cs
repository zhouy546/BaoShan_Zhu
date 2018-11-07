using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDisplay : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] 是主显示器, 默认显示并始终在主显示器上显示.        
        // 检查其他显示器是否可用并激活.        
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();


        Display.displays[1].SetRenderingResolution(3360, 1050);
        Display.displays[1].SetRenderingResolution(2048, 1152);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
