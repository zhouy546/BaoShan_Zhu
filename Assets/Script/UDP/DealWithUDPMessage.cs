
//*********************❤*********************
// 
// 文件名（File Name）：	DealWithUDPMessage.cs
// 
// 作者（Author）：			LoveNeon
// 
// 创建时间（CreateTime）：	Don't Care
// 
// 说明（Description）：	接受到消息之后会传给我，然后我进行处理
// 
//*********************❤*********************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using zhou.utility;
public struct HandPos
{
    public float x;
    public float y;
}
public class DealWithUDPMessage : MonoBehaviour {

   
    // public GameObject wellMesh;
    public string dataTest;
    public char separator = '#';




    public List<string> HandXy = new List<string>();

    /// <summary>
    /// 消息处理
    /// </summary>
    /// <param name="_data"></param>
    public void MessageManage(string _data)
    {


        if (_data != "")
        {


            dataTest = _data;


            if (dataTest == "null")
            {
                if( HandXy.Count > 0) { HandXy.Clear(); }
                
            }
            else {
                try
                {
                    HandXy = dataTest.Split(separator).ToList();

                }
                catch (Exception message)
                {

                    throw message;
                }
            }




            //foreach (var item in HandXy)
            //{
            //    Debug.Log(item);
            //}

        }


 
    }

    public List<HandPos> GetHandPos() {
        List<HandPos> posList = new List<HandPos>();
        foreach (string item in HandXy)
        {
            try
            {
                string[] str = item.Split('&');

                HandPos handpos;

                handpos.x = Utility.Maping(float.Parse(str[0]), 0f, 1f, 0, 1920, true);

                handpos.y = Utility.Maping(float.Parse(str[1]), 0f, 1f, 0, 1200, true);


                posList.Add(handpos);
            }
            catch (Exception m)
            {

                throw m;
            }


        }

        return posList;

    }



    private void Awake()
    {

    }


    public void Start()
    {

    }


    private void Update()
    {


        //Debug.Log("数据：" + dataTest);  
    }


}
