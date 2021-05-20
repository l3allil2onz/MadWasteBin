using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    public static LobbyUIManager Instance;
    public GameObject joinRoomBtn, createRoomBtn;
    public GameObject joinRoomPanel, createRoomPanel, joinRoomFailedPanel,joinFullRoomPanel;
    public InputField joinRoomInputField, createRoomInputField;

    private void Awake()
    {
        Instance = this;
    }
    public void OpenJoiningRoomPanelInLobby()
    {
        joinRoomPanel.SetActive(true);
    }

    public void OpenCreatingRoomPanelInLobby()
    {
        createRoomPanel.SetActive(true);
    }

    public void OpenJoiningRoomFailedPanelInLobby()
    {
        joinRoomFailedPanel.SetActive(true);
    }

    public void OpenJoiningFullRoomPanelInLobby()
    {
        joinFullRoomPanel.SetActive(true);
    }

    public void CloseJoiningFullRoomPanelInLobby()
    {
        joinFullRoomPanel.SetActive(false);
        joinRoomInputField.text = null;
    }

    public void CloseJoiningRoomPanelInLobby()
    {
        joinRoomPanel.SetActive(false);
        joinRoomInputField.text = null;
    }

    public void CloseCreatingRoomPanelInLobby()
    {
        createRoomPanel.SetActive(false);
        createRoomInputField.text = null;
    }
    
    public void CloseJoiningRoomFailedPanelInLobby()
    {
        joinRoomFailedPanel.SetActive(false);
        joinRoomInputField.text = null;
    }

    public void JoiningRoomInputChange()
    {
        if(joinRoomInputField.text.Length > 0)
            joinRoomBtn.SetActive(true);
        else
            joinRoomBtn.SetActive(false);
    }

    public void CreatingRoomInputChange()
    {
        if (createRoomInputField.text.Length > 0)
            createRoomBtn.SetActive(true);
        else
            createRoomBtn.SetActive(false);
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }

}
