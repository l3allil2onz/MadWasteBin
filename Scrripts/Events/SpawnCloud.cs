using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCloud : MonoBehaviour
{
    public GameObject startPoint, endPoint;
    public GameObject[] cloud;
    public float time;
    public float repeatRate;

	// Use this for initialization
	void Start ()
    {
        InvokeRepeating("SpawnClouds", time, repeatRate);
	}

    private void SpawnClouds()
    {
        int rndCloud = Random.Range(0, 2);
        float rndPosY = Random.Range(startPoint.transform.localPosition.y, endPoint.transform.localPosition.y);
        GameObject thisCloud = Instantiate(cloud[rndCloud], new Vector3(this.transform.localPosition.x, rndPosY, 0), cloud[rndCloud].transform.rotation);
        thisCloud.transform.parent = this.transform;
        thisCloud.transform.localPosition = new Vector3(this.transform.localPosition.x, rndPosY, 0);
        if (thisCloud.name == cloud[0].name)
        {
            thisCloud.transform.localScale = cloud[0].transform.localScale;
        }
        else if(thisCloud.name == cloud[1].name)
        {
            thisCloud.transform.localScale = cloud[1].transform.localScale;
        }
    }
}
