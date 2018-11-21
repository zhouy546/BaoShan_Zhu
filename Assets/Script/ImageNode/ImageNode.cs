using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using zhou.utility;
public class ImageNode : INode {


    [System.Serializable]
    public struct YZPos {
        public float y;
        public float z;
    };


    public Action FallingDown;
    public Action MovingOnTable;
    public Action SettingBack;

    public YZPos yZPos;
    public float Xoffset;

    public int ID;


    public float xmin, xmax;


    private float Max
    {
        get { return xmin + ((xmax - xmin) / 4 * (ID + 1)) - Xoffset; }

    }


    private float Min
    {
        get { return xmin + ((xmax - xmin) / 4 * ID) + Xoffset; }

    }

   // public float ScreenWidth;
    public SubCamera subCamera;

    private Transform CicleTrans;
    private float tableX;

    public SpriteRenderer sprite;

    // Use this for initialization
    public override void Start () {
        base.Start();

        //initialization(ID, subCamera);
    }


    private void OnEnable()
    {
        FallingDown += MoveDown;
        MovingOnTable += tableMove;
        SettingBack += GoBack;

    }

    Vector3 GenerateStartPos() {
        float xPos = UnityEngine.Random.Range(Min, Max);

        setTableX(xPos);

        Vector3 temp = new Vector3(xPos, yZPos.y, yZPos.z);

        return temp;
    }

    private void  setTableX(float val) {

       // Debug.Log("val"+val);
     //   Debug.Log("max:"+Max + "min:" + Min);

        tableX = Utility.Maping(val, Min, Max, subCamera.Boundtransforms[0].position.x+Xoffset, subCamera.Boundtransforms[1].position.x - Xoffset, true);

       // Debug.Log("tablex:"+tableX);
    }



    public void initialization(int id, SubCamera _Subcamera) {



        subCamera = _Subcamera;

        CicleTrans = _Subcamera.particleTran;

        ID = id;

        setBack();
    }


    private Vector3 getTableRotation() {
        Vector2 circleXY = new Vector2(CicleTrans.position.x, CicleTrans.position.y);
        Vector2 EdgePos = new Vector2(tableX, subCamera.Boundtransforms[0].position.y);

        Vector2 dir = EdgePos - circleXY;

        Vector2 temp = new Vector2(dir.x, dir.y).normalized;

        float z = -Mathf.Atan(temp.x / temp.y)* Mathf.Rad2Deg; 
        //Debug.Log(z);
        return new Vector3(0, 0, z);

    }

    private Vector3 GetTableMoveTargetPos()
    {
        Vector2 circleXY = new Vector2(CicleTrans.position.x, CicleTrans.position.y);
        Vector2 EdgePos = new Vector2(tableX, subCamera.Boundtransforms[0].position.y);

        Vector2 dir = EdgePos - circleXY;

        //Debug.Log(dir);

        Vector3 temp = new Vector3(dir.x, dir.y,0f).normalized;
        //Debug.Log(temp);
        Vector3 targetPos = new Vector3((circleXY.x + temp.x*12), (circleXY.y + temp.y*12), 40f);

        return targetPos;

    }

    public override void FallDown()
    {
        if (FallingDown != null)
        {
            FallingDown.Invoke();
        }
    }

    public override void MoveOnTable()
    {
        if (MovingOnTable != null)
        {
            MovingOnTable.Invoke();
        }
    }

    public override void setBack()
    {
        if (SettingBack != null)
        {
            SettingBack.Invoke();
        }
    }


    public void MoveDown()
    {
      //  Debug.Log(transform.position.x);

        LeanTween.move(this.gameObject, new Vector3(transform.position.x, -2.5f, transform.position.z), 10f).setOnComplete(MoveOnTable);

    }


    public void tableMove() {
        this.transform.position = CicleTrans.position;
        this.transform.rotation = Quaternion.Euler(getTableRotation());
      //  Debug.Log("runn");
        LeanTween.move(this.gameObject, GetTableMoveTargetPos(),10f).setOnComplete(setBack);
    }

    public void GoBack() {


        //Debug.Log(Max + "min:" + Min);
        this.transform.position = GenerateStartPos();
        this.transform.rotation = Quaternion.Euler(Vector3.zero);
        //Debug.Log(GetTableMoveTargetPos());
        FallDown();
    }

    //private void fallingDown() {
    //    if (FallingDown != null) {
    //        FallingDown.Invoke();
    //    }
    //}

    //private void movingOnTable() {
    //    if (MovingOnTable != null)
    //    {
    //        MovingOnTable.Invoke();
    //    }
    //}

    //private void settingBack()
    //{
    //    if (SettingBack != null)
    //    {
    //        SettingBack.Invoke();
    //    }
    //}
}
