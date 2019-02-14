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

    public Action Select;

    public YZPos yZPos;
    public float Xoffset;

    public int ID;

    public bool isMoveToHole;
    
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

    public bool b_like;

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
        Select += MoveToHole;

        Select += FTPupload;
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

     //   Debug.Log(this.sprite.sprite.name);
    }

    private void setSprite(Sprite _sprite) {
        sprite.sprite = _sprite;
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

    private Vector3 getBlackHolePos() {
        Vector3 pos = new Vector3(subCamera.blackHoleTrans.position.x, subCamera.blackHoleTrans.position.y, this.transform.position.z);
        return pos;
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


    public override void Selected()
    {
        if (Select != null)
        {
            Select.Invoke();
        }
    }

    public void MoveDown()
    {
      //  Debug.Log(transform.position.x);

        LeanTween.move(this.gameObject, new Vector3(transform.position.x, -2.5f, transform.position.z), 10f).setOnUpdate(UpdatePos).setOnComplete(MoveOnTable);

    }


    public void tableMove() {
        this.transform.position = CicleTrans.position;
        this.transform.rotation = Quaternion.Euler(getTableRotation());
      //  Debug.Log("runn");
        LeanTween.move(this.gameObject, GetTableMoveTargetPos(),10f).setOnUpdate(UpdatePos).setOnComplete(setBack);
    }

    public void GoBack() {


        //Debug.Log(Max + "min:" + Min);
        this.transform.position = GenerateStartPos();
        this.transform.rotation = Quaternion.Euler(Vector3.zero);
        this.transform.localScale = Vector3.one*0.5F;
        b_OnEnter = false;
        //Debug.Log(GetTableMoveTargetPos());
        FallDown();

        int val = UnityEngine.Random.Range(0, ValueSheet.Imagesprites.Count);

        setSprite(ValueSheet.Imagesprites[val]);

    }

    public void MoveToHole()
    {
        LeanTween.cancel(this.gameObject);
        LeanTween.move(this.gameObject, getBlackHolePos(), 1f).setOnComplete(GoBack);
        LeanTween.scale(this.gameObject, Vector3.zero, 1f);
        subCamera.UploadImages.Add(this);//扫码后清除
    }


    public void FTPupload() {
        string localFile = Application.streamingAssetsPath + "/"+"Image"+"/" + this.sprite.sprite.name;
       StartCoroutine( Upload.instance.uploadImage(ID.ToString(), subCamera.UploadImages.Count.ToString(), ".jpg", localFile));

    }

    private bool b_OnEnter;
    public void OnPointerEnter() {
         Selected();

        //Moveto black hole 重置b_OnEnter;

        //添加list，FTP上传，命名从0开始， 

    }

    public void OnPointerUpdate() {

    }



    public void AddLikeUpdate() {
        if (b_OnEnter == false)
        {
            b_OnEnter = true;
            OnPointerEnter();
        }

        OnPointerUpdate();

    }

    public void Update()
    {
        //if (isMoveToHole) {
        //    isMoveToHole = false;
        //    MoveToHole();
        //   // FTPupload();
        //}

  
    }

    public struct PositionXY {
       public float x;
       public float y;
    }

    public PositionXY positionXY;

    private void UpdatePos(float val) {
        positionXY.x = this.transform.position.x;
        positionXY.y = this.transform.position.y;
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
