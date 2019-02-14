using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.IO;
using FTPNAMESPACE;
using System.Threading;

public class Upload : MonoBehaviour {
    string Root = "www/baoshan";

    public static Upload instance;
    private void Start()
    {
        if (instance == null) {
            instance = this;
        }
    }

    //public void upload() {
    //    ftp ftpClient = new ftp(@"ftp://39.104.81.205/", "ftpuser", "ftpuser");
    //    Debug.Log("runing");

    //    //ftpClient.createDirectory("www/baoshan/images/new");

    //    /* Upload a File */
    //   ftpClient.upload("www/baoshan/0/images/1.jpg", Application.streamingAssetsPath+"/1.jpg");
    //}

    public IEnumerator uploadImage(string table, string name, string surffix, string localFile) {
        Thread thread = new Thread(()=>upload(table,name,surffix,localFile));
        thread.Start();
        yield return new WaitForSeconds(2f);
        thread.Abort();

    }


    private void upload(string table, string name, string surffix,string localFile)
    {
        ftp ftpClient = new ftp(@"ftp://39.104.81.205/", "ftpuser", "ftpuser");

        string ImageDirectory = Root + "/" + table +"/"+ "Images/";
      
        //ftpClient.createDirectory(ImageDirectory);

        /* Upload a File */
        string remoteFile = ImageDirectory+ name + surffix; 
      
        Debug.Log(remoteFile);
        Debug.Log(localFile);
        ftpClient.upload(remoteFile, localFile);
    }


    public void delete(string str) {
        ftp ftpClient = new ftp(@"ftp://39.104.81.205/", "ftpuser", "ftpuser");
        ftpClient.delete(str);
    }

    void go() {

    }

    private void Update()
    {

    }
}
