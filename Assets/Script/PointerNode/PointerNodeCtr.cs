using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerNodeCtr : MonoBehaviour {
    public List<Node> nodes = new List<Node>();
    public DealWithUDPMessage dealWithUDPMessage;

    public GameObject NodeObj;
    // Use this for initialization
    private float xoffset = 1920/2;
    private float yoffset =1200/2;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        List<HandPos> handPosList = dealWithUDPMessage.GetHandPos();


        while (nodes.Count < handPosList.Count) {
            CreateNode();
        }

        while (nodes.Count > handPosList.Count) {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i >= handPosList.Count) {
                    Destroy(nodes[i].gameObject,0.5f);

                    nodes.RemoveAt(i);
                }
            }
        }
        

        foreach (var HandPos in handPosList)
        {
            if (handPosList.Count != 0)
            {
                Vector3 pos = new Vector3(HandPos.x - xoffset, -HandPos.y + yoffset);

                nodes[handPosList.IndexOf(HandPos)].transform.localPosition = pos;

            }
        }
    }

    private void CreateNode()
    {
       GameObject g = Instantiate(NodeObj);
        g.transform.SetParent(this.transform);
        g.transform.localScale = Vector3.one;
        nodes.Add(g.GetComponent<Node>());
    }

}
