using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class photonHandler : MonoBehaviour
{
    public GameObject mainMenuGame;
    public PhotonButton photonButt;
    public GameObject[] mainPlayer;
    public GameObject scorePlayerPrefab;

    public PhotonPlayer photonPlayer;
    private PhotonView photonView;
    public bool mainMenuScene, gameScene;
    public static photonHandler instance;
    public string stage;
    public float time;
    bool isReady;
    bool isLocked;
    public int characterPlayerIndex;
    int playerInGame;

    private void Awake()
    {
        CheckMainMenuScene();
        instance = this;
        DontDestroyOnLoad(this.transform);
        photonView = GetComponent<PhotonView>();

        PhotonNetwork.sendRate = 30;
        PhotonNetwork.sendRateOnSerialize = 20;
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
        print(SceneManager.GetActiveScene().name);
    }
    private void CheckMainMenuScene()
    {
        if (SceneManager.GetActiveScene().name == "mainMenu")
        {
            if (PhotonNetwork.connected)
            {
                mainMenuGame.SetActive(false);
            }
            else if (!PhotonNetwork.connected)
            {
                mainMenuGame.SetActive(true);
            }
            mainMenuScene = true;
            gameScene = false;
        }
        else if (SceneManager.GetActiveScene().name == "City")
        {
            mainMenuScene = false;
            gameScene = true;
        }
        else if (SceneManager.GetActiveScene().name == "Beach")
        {
            mainMenuScene = false;
            gameScene = true;
        }
        else if (SceneManager.GetActiveScene().name == "Forest")
        {
            mainMenuScene = false;
            gameScene = true;
        }
    }

    public void CreateNewRoom()
    {
        print("CreateNewRoom !");
        RoomOptions roomOption = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };
        if (photonButt.createRoomInput.text.Length > 0)
            PhotonNetwork.CreateRoom(photonButt.createRoomInput.text, roomOption, TypedLobby.Default);

        SwitchFromLobbyToRoomPanel();
    }
    private void ChangeRoomName(string roomName)
    {
        GameObject curRoomName = GameObject.Find("CurRoomNameText");
        TextMeshProUGUI roomNameText = curRoomName.GetComponent<TextMeshProUGUI>();
        roomNameText.text = roomName;
    }
    private void SwitchFromLobbyToRoomPanel()
    {
        string roomName = "";
        GameObject joinRoom = GameObject.Find("sectionView2 - connected menu");
        Animator joinRoomAnim = joinRoom.transform.GetComponent<Animator>();
        roomName = SetRoomName();
        ChangeRoomName(roomName);
        photonButt.createRoomInput.text = null;
        LobbyUIManager.Instance.CloseCreatingRoomPanelInLobby();
        joinRoomAnim.SetBool("JoinRoom", true);
    }
    private string SetRoomName()
    {
        string name;
        name = LobbyUIManager.Instance.createRoomInputField.text;
        return name;
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("create room failed : " + codeAndMessage[1]);
    }

    public void JoinRoom()
    {
        RoomLayoutGroup _roomLayout;
        string roomName = photonButt.joinRoomInput.text;
        bool isAvailable = false;

        //  ตรวจสอบว่า idของห้องที่ป้อนไป หลังจากกดปุ่ม Join ไปแล้วปัจจุบันมี idตรงตามที่ป้อนไปหรือไม่
        //  ทำการส่งไปให้ RoomLayoutGroup ตรวจสอบ เนื่องจากจำเป็นต้องตรวจสอบจาก Listทั้งหมดของ RoomListingButtons ใน class RoomLayoutGroup
        _roomLayout = GameObject.Find("RoomLayout").GetComponent<RoomLayoutGroup>();
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach (RoomInfo room in rooms)
        {
            isAvailable = _roomLayout.CheckJoiningRoomWithButton(room, roomName);
            if (isAvailable)
                break;
        }

        if (isAvailable)
        {
            PhotonNetwork.JoinRoom(photonButt.joinRoomInput.text);
            GameObject joinRoom = GameObject.Find("sectionView2 - connected menu");
            Animator joinRoomAnim = joinRoom.transform.GetComponent<Animator>();
            ChangeRoomName(roomName);
            joinRoomAnim.SetBool("JoinRoom", true);
            photonButt.joinRoomInput.text = null;
            LobbyUIManager lobbyUI = GameObject.Find("lobbyUIManager").GetComponent<LobbyUIManager>();
            lobbyUI.CloseJoiningRoomPanelInLobby();
            print("Join (" + roomName + ") room");
        }
        else
        {
            LobbyUIManager lobbyUI = GameObject.Find("lobbyUIManager").GetComponent<LobbyUIManager>();
            lobbyUI.OpenJoiningRoomFailedPanelInLobby();
            print("Room id is dose not exist");
        }
    }

    public void SendingStageAndTimeIndex(int stageIndex, int timeIndex)
    {
        CurrentRoomCanvas curRc = CurrentRoomCanvas.instance;
        stage = curRc.stageTag[stageIndex];
        time = float.Parse(curRc.timeTag[timeIndex]);
        moveScene();
    }

    public void moveScene()
    {
        CurrentRoomCanvas curRc = CurrentRoomCanvas.instance;
        PhotonPlayer[] players = PhotonNetwork.playerList;
        isReady = curRc.CheckReadyPlayer(); ;
        isLocked = curRc.CheckPlayersAreLockedCharacters();
        if (isLocked && isReady)
        {
            print("moveScene() : MoveScene !");
            print("Scene : " + stage);
            PhotonNetwork.LoadLevel(stage);
        }
        else
        {
            print("moveScene() : Some players are not ready");
        }
    }

    /*private void OnJoinedRoom()
    {
        moveScene();
        print("We are connected to the room!");
    }*/
    

    private void OnSceneFinishedLoading(Scene scene,LoadSceneMode mode)
    {
        if(scene.name == "City")
        {
            if(PhotonNetwork.isMasterClient)
            {
                MasterLoadedGame();
            }
            else
            {
                NonMasterLoadedGame();
            }
            //SpawnPlayer();
            //photonView.RPC("SpawnPlayer", PhotonTargets.All);
        }
        else if (scene.name == "Beach")
        {
            if (PhotonNetwork.isMasterClient)
            {
                MasterLoadedGame();
            }
            else
            {
                NonMasterLoadedGame();
            }
            //SpawnPlayer();
            //photonView.RPC("SpawnPlayer", PhotonTargets.All);
        }
        else if (scene.name == "Forest")
        {
            if (PhotonNetwork.isMasterClient)
            {
                MasterLoadedGame();
            }
            else
            {
                NonMasterLoadedGame();
            }
            //SpawnPlayer();
            //photonView.RPC("SpawnPlayer", PhotonTargets.All);
        }
    }
    private void MasterLoadedGame()
    {
        photonView.RPC("RPC_LoadGameScene", PhotonTargets.MasterClient);
        photonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others,stage);
    }
    private void NonMasterLoadedGame()
    {
        photonView.RPC("RPC_LoadGameScene", PhotonTargets.MasterClient);
    }

    [PunRPC]
    private void RPC_LoadGameOthers(string stage)
    {
        PhotonNetwork.LoadLevel(stage);
    }
    [PunRPC]
    private void RPC_LoadGameScene()
    {
        playerInGame++;
        if(playerInGame == PhotonNetwork.playerList.Length)
        {
            print("All players are in the game scene.");
            photonView.RPC("RPC_SpawnPlayer", PhotonTargets.AllBufferedViaServer);
        }
    }

    public void SendCharacterIndex(PlayerScore ps)
    {
        ps.characterPlayerIndex = characterPlayerIndex;
    }
    [PunRPC]
    private void RPC_SpawnPlayer()
    {
        CurrentRoomCanvas curRc = CurrentRoomCanvas.instance;
        GameManager gm = GameManager.instance;

        int characterIndex = curRc.CheckPlayersCharacters();
        //characterIndex.Add(curRc.CheckPlayersCharacters());
        //GameObject spawnPoint = GameObject.Find("SpawnPoint");
        //float rndSpawn = Random.Range(-7f, 7f);
        GameObject player = PhotonNetwork.Instantiate(mainPlayer[characterIndex].name,
                                              mainPlayer[characterIndex].transform.position,
                                              mainPlayer[characterIndex].transform.rotation, 0);

        GameObject scorePlayer = PhotonNetwork.Instantiate(scorePlayerPrefab.name,
                                                           scorePlayerPrefab.transform.position,
                                                           scorePlayerPrefab.transform.rotation, 0);
        //photonView.RPC("SendPlayerListing", PhotonTargets.AllViaServer, scorePlayer, player);

        PlayerScore ps = scorePlayer.GetComponent<PlayerScore>();
        SendCharacterIndex(ps);

        gm.gameTime = time;
        print("Set game time : " + gm.gameTime);
    }
}
