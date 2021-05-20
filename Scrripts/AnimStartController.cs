using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimStartController : MonoBehaviour
{
    PhotonUsernameRegister phoUser;
    Animator anim;

    private void Awake()
    {
        phoUser = GameObject.Find("photonDontDestroy").GetComponent<PhotonUsernameRegister>();
        anim = GameObject.Find("MainMenuPanel").GetComponent<Animator>();
    }
    public void CallCheckRegisterFunction()
    {
        phoUser.CheckRegister();
    }
    public void CallCloseMainMenuFunction()
    {
        phoUser.CloseMainMenu();
    }
    public void EnableCloseMainMenuCondition()
    {
        anim.SetBool("Close", true);
    }
    public void DisableCloseMainMenuCondition()
    {
        anim.SetBool("Close", false);
    }
}
