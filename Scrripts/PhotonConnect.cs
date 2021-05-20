using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonConnect : MonoBehaviour
{
    public string versionName = "0.1";
    public GameObject sectionView1, sectionView2, sectionView3,mainMenu;

    private void Awake()
    {
        /*sectionView1.SetActive(true);
        PhotonNetwork.ConnectUsingSettings(versionName);*/
    }

    // Work after player clicked the start button
    public void MultiplayerMode()
    {
        if (!PhotonNetwork.connected)
        {
            sectionView1.SetActive(true);
            mainMenu.SetActive(false);
            PhotonNetwork.ConnectUsingSettings(versionName);
            print("Connecting to photon.. .");
        }
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        print("We are connected to master");
    }

    private void OnJoinedLobby()
    {
        sectionView1.SetActive(false);
        sectionView2.SetActive(true);
        print("On Joined Lobby");
        if(!PhotonNetwork.inRoom)
            MainCanvasManager.Instance.LobbyCanvas.transform.SetAsLastSibling();
    }

    private void OnDisconnectedFromPhoton()
    {
        if (sectionView1.active)
            sectionView1.SetActive(false);

        if (sectionView2.active)
            sectionView2.SetActive(false);

        sectionView3.SetActive(true);

        print("Disconnected from photon services");
    }

    private void OnFailedToConnectToPhoton()
    {
        print("Failed to connect to photon");
    }
}
