using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeEvent : MonoBehaviour
{
    PhotonView photonView;
    public GameObject treePrefabs;
    public GameObject startPoint, endPoint;
    public int cDTimeMin, cDTimeMax;

    public float rndTimeValue;
    public float rndPosX;
    public float curTimeCountDown;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (curTimeCountDown > 0)
            {
                curTimeCountDown -= Time.deltaTime;
            }
            else if(curTimeCountDown <= 0)
            {
                RandomTimeCountDown();
            }
        }
    }
    private void RandomTimeCountDown()
    {
        if (curTimeCountDown <= 0)
        {
            rndTimeValue = Random.Range(cDTimeMin, cDTimeMax);
            rndPosX = Random.Range(startPoint.transform.localPosition.x, endPoint.transform.localPosition.x);
            GameObject thisTrees = PhotonNetwork.Instantiate(treePrefabs.name, transform.localPosition, treePrefabs.transform.rotation, 0);
            thisTrees.transform.parent = transform;
            thisTrees.transform.localPosition = new Vector3(rndPosX, -3.65f, 10f);
            curTimeCountDown = rndTimeValue;
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(curTimeCountDown);
        }
        else
        {
            curTimeCountDown = (float)stream.ReceiveNext();
        }
    }
}
