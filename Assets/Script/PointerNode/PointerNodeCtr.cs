using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using zhou.utility;

public class PointerNodeCtr : MonoBehaviour {
    public List<Node> nodes = new List<Node>();
    public DealWithUDPMessage dealWithUDPMessage;
    public SubCamera subCamera;
    public GameObject NodeObj;
    // Use this for initialization
    private float xoffset = 1920/2;
    private float yoffset =1200/2;

    Thread m_thread;
    bool loop = true;

    bool threadOnOff = true;

    public Transform ShowQRBtn;

    public Camera m_camera;

    List<ImageNode> imageNodes {
        get { return ValueSheet.id_ImageNodes[subCamera.id]; }
    }


    void Start () {

        StartCoroutine(upadteDistance(nodes));


        //Debug.Log(m_thread.ThreadState);


    }

    // Update is called once per frame
    List<HandPos> handPosList = new List<HandPos>();


    public List<HandPos> GetMousePos()
    {
        List<HandPos> posList = new List<HandPos>();

        HandPos handpos;

        handpos.x = Input.mousePosition.x;//Utility.Maping(Input.mousePosition.x, 0f, 1f, 0, 1920, true);

        handpos.y = Utility.Maping(Input.mousePosition.y, 1200f, 0f, 0, 1200, true);


        posList.Add(handpos);

      

        return posList;
    }


    void Update () {

    handPosList = dealWithUDPMessage.GetHandPos();



        if (nodes.Count < handPosList.Count) {
            CreateNode();
        }

        if (nodes.Count > handPosList.Count)
        {
            //if (threadOnOff)
            //{
            //    Debug.Log("off");

            //        loop = false;
            //        m_thread.Abort();
                   
            //    threadOnOff = false;
            //}

   

            for (int i = 0; i < nodes.Count; i++)
            {
                if (i >= handPosList.Count)
                {

                    Destroy(nodes[i].gameObject, .2f);      

                    nodes.RemoveAt(i);


                }
            }
            //if (!threadOnOff)
            //{// 线程开关
            //    Debug.Log("on");
            //    m_thread = new Thread(() => UpdateThread(nodes));
            //    loop = true;
            //    m_thread.Start();

            //    threadOnOff = true;                                              

            //}
        }


        foreach (HandPos HandPos in handPosList)
        {
            if (handPosList.Count != 0)
            {
                Vector3 pos = new Vector3(HandPos.x - xoffset, -HandPos.y + yoffset);

                try
                {
                    if (handPosList.IndexOf(HandPos) <= nodes.Count - 1) {
                        nodes[handPosList.IndexOf(HandPos)].transform.localPosition = pos;

                    }
                }
                catch (System.Exception message)
                {

                    throw message;
                }

            }
        }

        //UpdateThread();
    }

    //public void UpdateThread(List<Node> _nodes) {


    //    while(loop){

    //        Thread.Sleep(TimeSpan.FromSeconds(0.2f));
    //        upadteDistance(_nodes);


    //    }
    //}

    private IEnumerator upadteDistance(List<Node> _nodes) {
        Debug.Log(_nodes.Count);
        foreach (Node node in _nodes)
        {
            foreach (ImageNode imageNode in imageNodes)
            {
                float distance = (new Vector2(imageNode.positionXY.x, imageNode.positionXY.y) - node.WorldPosXY).magnitude;

               // Debug.Log(distance);
                if (distance < 1)
                {
                    Debug.Log("Like");
                    imageNode.AddLikeUpdate();//添加到喜欢 UPDATE
                }

            }

            // float btndis = (new Vector2(ShowQRBtn.position.x, ShowQRBtn.position.y) - node.WorldPosXY).magnitude;

            //if (btndis < 1)
            //{
            //    //triggerQRBTN
            //    Debug.Log("triggerQRBTN");
            //}
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(upadteDistance(nodes));
    }

    //private void OnApplicationQuit()
    //{
    //    m_thread.Abort();

    //    loop = false;
    //}



    private void CreateNode()
    {
       GameObject g = Instantiate(NodeObj);
        g.transform.SetParent(this.transform);
        g.transform.localScale = Vector3.one;
        nodes.Add(g.GetComponent<Node>());
    }

}
