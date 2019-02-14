using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FTPNAMESPACE;
public class SubCamera : MonoBehaviour {
    public Transform[] Boundtransforms;

    public Transform particleTran;

    public GameObject ImageNodeprefabs;

    public int numImage;

    public int id;

    public Transform parent;

    public Transform blackHoleTrans;

    public List<ImageNode> UploadImages = new List<ImageNode>();

	// Use this for initialization
	void Start () {
    //    StartCoroutine(initialization());
	}

    public IEnumerator initialization() {
        List<ImageNode> ImageNodes = new List<ImageNode>();
        ValueSheet.id_ImageNodes.Add(id, ImageNodes);
        for (int i = 0; i < numImage; i++)
        {
            GameObject g = Instantiate(ImageNodeprefabs);
            g.transform.SetParent(parent);
            ImageNode node = g.GetComponent<ImageNode>();
            ImageNodes.Add(node);
         
            yield return new WaitForSeconds(Random.Range(3f, 6f));
            ValueSheet.id_ImageNodes[id]= ImageNodes;

            node.initialization(id, this);
        }


    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ClearUploadImages() {
        UploadImages.Clear();
    }

    public void DeleteImages() {
        for (int i = 1; i <= UploadImages.Count; i++)
        {
            string str = i.ToString() + ".jpg";
            Upload.instance.delete(str);
        }
    }
}
