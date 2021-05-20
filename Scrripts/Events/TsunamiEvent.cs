using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiEvent : MonoBehaviour
{
    PhotonView photonView;
    public GameObject tsunami;
    public int cDTimeMin, cDTimeMax;
    float rndTimeValue;
    public float curTimeCountDown;
    public bool isTsunamiActive = false;
    public static TsunamiEvent Instance;

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if(!PhotonNetwork.isMasterClient)
        {
            if(isTsunamiActive)
            {
                tsunami.SetActive(true);
            }
            else
            {
                tsunami.SetActive(false);
            }
        }
        if (PhotonNetwork.isMasterClient)
        {
            if (curTimeCountDown > 0)
            {
                curTimeCountDown -= Time.deltaTime;
            }
            else if (curTimeCountDown <= 0)
            {
                RandomTimeCountDown();
            }
        }
    }
    private void RandomTimeCountDown()
    {
        if (rndTimeValue != 0)
        {
            tsunami.SetActive(true);
            isTsunamiActive = true;
        }
        rndTimeValue = Random.Range(cDTimeMin, cDTimeMax);
        curTimeCountDown = rndTimeValue;
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(curTimeCountDown);
            stream.SendNext(isTsunamiActive);
        }
        else
        {
            curTimeCountDown = (float)stream.ReceiveNext();
            isTsunamiActive = (bool)stream.ReceiveNext();
        }
    }
}
