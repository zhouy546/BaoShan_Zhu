using UnityEngine;
using System.Collections;

public class MultiDisplayActivator : MonoBehaviour
{
    public static MultiDisplayActivator Control;

    void Awake()
    {
        if (Control == null)
        {
            DontDestroyOnLoad(gameObject);
            Control = this;
            Load();
        }
        else if (Control != this)
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        //Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.

        if (Display.displays.Length > 1)
        {
            for (int i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
        }
    }

    void Load()
    {

    }

    void Save()
    {

    }
    void Update()
    {

    }
}

