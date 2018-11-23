using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCamera : MonoBehaviour {
    public Transform[] Boundtransforms;

    public Transform particleTran;

    public GameObject ImageNodeprefabs;

    public int numImage;

    public int id;

    public Transform parent;

	// Use this for initialization
	void Start () {
    //    StartCoroutine(initialization());
	}

    public IEnumerator initialization() {
        for (int i = 0; i < numImage; i++)
        {
            GameObject g = Instantiate(ImageNodeprefabs);
            g.transform.SetParent(parent);
            ImageNode node = g.GetComponent<ImageNode>();
            ValueSheet.ImageNodes.Add(node);
            node.initialization(id,this);
            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
