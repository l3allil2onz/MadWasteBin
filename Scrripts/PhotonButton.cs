using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonButton : MonoBehaviour
{
    public photonHandler phandler;
    public InputField joinRoomInput, createRoomInput;

    public void OnClickCreateRoom()
    {
        //if(PhotonNetwork.get)

        phandler.CreateNewRoom();
    }

    public void OnClickJoinRoom()
    {
        phandler.JoinRoom();
    }

}
