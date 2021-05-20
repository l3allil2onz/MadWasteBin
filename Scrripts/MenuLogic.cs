using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLogic : MonoBehaviour
{

    public void disableMenuUI()
    {
        PhotonNetwork.LoadLevel("gameScene");
    }
}
