using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimUsernamePanelController : MonoBehaviour
{
    PhotonUsernameRegister phoUser;

    private void Awake()
    {
        phoUser = GameObject.Find("photonDontDestroy").GetComponent<PhotonUsernameRegister>();
    }
    public void CallCloseUserRegisPanelFunction()
    {
        phoUser.CloseUserRegisterPanel();
    }
}
