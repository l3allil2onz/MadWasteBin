using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMovement : MonoBehaviour
{
    public GameObject[] cloud;
    public GameObject spawnCloud;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    private IEnumerator WaitAndSpawnCloud(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //GameObject
    }
}
