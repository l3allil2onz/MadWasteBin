using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PhotonUsernameRegister : MonoBehaviour
{
    PhotonConnect phoCon;
    photonHandler phoHan;
    private bool alreadyRegistered = false;
    public InputField usernameInput;
    public GameObject objectParent;
    public GameObject createButton;
    Animator animUsernameRegisterPanel;

    private void Awake()
    {
        // Test CheckRegister();
        //CheckRegister();
        phoCon = GameObject.Find("photonScript").GetComponent<PhotonConnect>();
        phoHan = GetComponent<photonHandler>();
    }

    private void CheckMainMenuSceneForGetCompAnim()
    {
        if (phoHan.mainMenuScene)
        {
            animUsernameRegisterPanel = objectParent.GetComponent<Animator>();
        }
    }
    //  Work after player clicked the start button
    public void CheckRegister()
    {
        if(!alreadyRegistered)
        {
            CheckMainMenuSceneForGetCompAnim();
            objectParent.SetActive(true);
        }
    }

    public void CloseMainMenu()
    {
        phoCon.mainMenu.SetActive(false);
    }

    public void UsernameInputChange()
    {
        if(usernameInput.text.Length >= 2)
        {
            createButton.SetActive(true);
        }
        else
        {
            createButton.SetActive(false);
        }
    }

    public void CreateUsername()
    {
        createButton.SetActive(false);
        PhotonNetwork.playerName = usernameInput.text;
        animUsernameRegisterPanel.SetBool("Close", true);
    }

    public void CloseUserRegisterPanel()
    {
        objectParent.SetActive(false);

        print("This machine name is : " + PhotonNetwork.playerName);
        phoCon.MultiplayerMode();   
    }
}
