using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MRTEvent : MonoBehaviour
{
    public GameObject train;
    public GameObject startPoint, endPoint;
    public int cDTimeMin, cDTimeMax;
    public float rndTimeValue;
    public float curTimeCountDown;
    public float trainSpeed;
	
	// Update is called once per frame
	void Update ()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (train.transform.localPosition.x <= endPoint.transform.localPosition.x && curTimeCountDown <= 0)
            {
                RandomTimeCountDown();
            }
            else if (train.transform.localPosition.x > endPoint.transform.localPosition.x && curTimeCountDown <= 0)
            {
                train.transform.localPosition -= new Vector3(trainSpeed, 0, 0);
            }
            else if (train.transform.localPosition.x > endPoint.transform.localPosition.x && curTimeCountDown >= 0)
            {
                if (curTimeCountDown >= 0)
                {
                    curTimeCountDown -= Time.deltaTime;
                }
            }
        }
    }
    private void RandomTimeCountDown()
    {
        if (curTimeCountDown <= 0)
        {
            rndTimeValue = Random.Range(cDTimeMin, cDTimeMax);
            curTimeCountDown = rndTimeValue;
            train.transform.localPosition = startPoint.transform.localPosition;
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(curTimeCountDown);
            stream.SendNext(train.transform.localPosition);
        }
        else
        {
            train.transform.localPosition = (Vector3)stream.ReceiveNext();
            curTimeCountDown = (float)stream.ReceiveNext();
        }
    }
}
