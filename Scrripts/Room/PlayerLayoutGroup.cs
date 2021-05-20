using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLayoutGroup : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerListingPrefab;
    private GameObject PlayerListingPrefab
    {
        get { return _playerListingPrefab; }
    }

    public List<PlayerListing> _playerListings = new List<PlayerListing>();
    public List<PlayerListing> PlayerListings
    {
        get { return _playerListings; }
    }

    public static PlayerLayoutGroup instance;
    //public bool isUpdateViewID = false;
    //public int curViewID = 1;

    private void Awake()
    {
        instance = this;
    }

    private void Authorzation()
    {
        //  ใช้ PhotonNetwork.isMasterClient เพื่อตรวจสอบClientทุกๆตัวว่าตัวไหนคือ MasterClient เพื่อสามารถให้ทำงานในรูปแบบเฉพาะClient นั้นๆได้
        if (PhotonNetwork.isMasterClient)
            CurrentRoomCanvas.instance.BecomeAHost();
        else
            CurrentRoomCanvas.instance.BecomeAMember();
    }

    private void UpdateListeners(PhotonPlayer photonPlayer)
    {
        PhotonPlayer[] player = PhotonNetwork.playerList;
        for(int i = 0; i < player.Length; i++)
        {
            //if(PlayerListings[i].GetComponent<PlayerListing>().photon)
        }
    }

    private void UpdateMasterClientLogo(PhotonPlayer photonPlayer)
    {
        int playerIndex = PlayerListings.FindIndex(x => x.photonPlayer == photonPlayer);
        if (photonPlayer.IsMasterClient)
        {
            PlayerListings[playerIndex].ActivateMasterClientLogo();
        }
        else
        {
            PlayerListings[playerIndex].DeactivateMasterClientLogo();
        }
    }
    //  ถูกเรียกอัตโนมัติโดย photon เมื่อใดก็ตามที่ master client ได้ถูกสับเปลี่ยน
    private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (PhotonNetwork.masterClient.NickName != newMasterClient.NickName)
        {
            PhotonNetwork.SetMasterClient(newMasterClient);
            Debug.Log(newMasterClient.NickName);
        }
        //  index 0 == masterClient
        Authorzation();
        UpdateMasterClientLogo(newMasterClient);
    }

    //  ถูกเรียกอัตโนมัติจาก photon ทุกครั้งที่มีผู้เล่นเข้าร่วมห้อง
    private void OnJoinedRoom()
    {
        foreach(Transform child in transform)
        {
            //print("Transform child : "+child.name);
            Destroy(child.gameObject);
        }

        MainCanvasManager.Instance.CurrentRoomCanvas.transform.SetAsLastSibling();
        //  เข้าถึงจำนวนผู้เล่นที่อยู่ในห้องปัจจุบันทางClass PhotonNetwork แล้วนำมาเก็บไว้ยังPhotonPlayer
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for(int i =0; i < photonPlayers.Length; i++)
        {
            PlayerJoinedRoom(photonPlayers[i]);
            print("Player in this room : " + photonPlayers[i]);
        }
        if (photonPlayers.Length > 0)
        {
            AllowcateBtn(photonPlayers.Length-1);
        }
        UpdateMasterClientLogo(photonPlayers[0]);
    }

    private void AllowcateBtn(int index)
    {
        if (index == 0)
        {
            print("This player is local index(Master) : " + index);
            PlayerListings[index].AddSelectStageButton();
            PlayerListings[index].AddSelectTimeButton();
            PlayerListings[index].AddSelectCharacterButton();
            PlayerListings[index].AddLockCharacterButton();
            PlayerListings[index].AddStartButton();
            //PlayerListings[index].AddLeaveRoomBotton(index);
        }
        else
        {
            print("This player is local index : " + index);
            PlayerListings[index].AddReadyButton(index);
            PlayerListings[index].AddSelectCharacterButton();
            PlayerListings[index].AddLockCharacterButton();
            //PlayerListings[index].AddLeaveRoomBotton(index);
        }
    }
    //  ถูกเรียกอัตโนมัติจาก photon เมื่อมีผู้เล่นเชื่อมต่อ
    private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
    {
        PlayerJoinedRoom(photonPlayer);
    }
    //  ถูกเรียกอัตโนมัติจาก photon เมื่อมีผู้เล่นออกจากห้อง
    private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer)
    {
        PlayerLeftRoom_Disconnect(photonPlayer);
    }
    //  สร้างPrefab แสดงผลชื่อ ดึงผู้เล่นมาเก็บไว้ที่List รอการดำเนินการต่อไป
    private void PlayerJoinedRoom(PhotonPlayer photonPlayer)
    {
        if (photonPlayer == null)
            return;

        //PlayerLeftRoom(photonPlayer);
        //  เรียกPrefabมาวางที่ Player(s) List Panel 
        GameObject playerListingObj = Instantiate(PlayerListingPrefab);
        playerListingObj.transform.SetParent(transform, false);
        //  กำหนดชื่อที่แสดงตาม ชื่อที่ผู้เล่นคนนั้นได้ป้อนเข้าระบบ Photon 
        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
        playerListing.photonView.viewID = PlayerListings.Count + 2;
        playerListing.ApplyPhotonPlayer(photonPlayer);

        //  เพิ่มเข้า List เพื่อใช้ดำเนินการต่อไป
        PlayerListings.Add(playerListing);
        int playerIndex = PlayerListings.IndexOf(playerListing);
        print("playerJoinedRoomIndex : " + playerIndex);
        Authorzation();
        

    }
    //  ลบPrefabผู้เล่น ออกจาก Player(s) List Panel
    private void PlayerLeftRoom(PhotonPlayer photonPlayer)
    {
        PhotonPlayer[] player = PhotonNetwork.playerList;
        //  ตรวจสอบด้วยFindIndex ว่าผู้เล่นที่อยู่ในระบบทุกคน มีใครที่มีค่าตรงกับ parameter ที่ส่งมา ถ้ามีจริง ให้มีค่า index
        int index = PlayerListings.FindIndex(x => x.photonPlayer == photonPlayer);
        //  เป็น -1 เมื่อFindIndex ไม่พบค่าที่ตรงกัน หมายความว่าผู้เล่นปัจจุบันได้ออกจากห้องไปแล้ว
        if (index != -1)
        {
            //  ทำลายPrefabของผู้เล่นคนนั้นทิ้ง แล้วลบออกจาก List
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }

    private void PlayerLeftRoom_Disconnect(PhotonPlayer photonPlayer)
    {
        PhotonPlayer[] player = PhotonNetwork.playerList;
        int index = PlayerListings.FindIndex(x => x.photonPlayer == photonPlayer);
        if (index != -1)
        {
            UpdateButtonAndIndex(index);
            ResetViewID(index);
            UpdateViewID(index);
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }
    public void OnClickHiddenRoom()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        PhotonNetwork.room.IsOpen = !PhotonNetwork.room.IsOpen;
        PhotonNetwork.room.IsVisible = PhotonNetwork.room.IsOpen;
    }

    public void OnClickLeaveRoom(int index)
    {
        print("======================= OnClickLeaveRoom(" + index + ") =======================");
        print("index  : " + index);
        //  Update ViewID & Button
        UpdateButtonAndIndex(index);
        ResetViewID(index);
        UpdateViewID(index);
        //  UI Control
        GameObject joinRoom = GameObject.Find("sectionView2 - connected menu");
        Animator joinRoomAnim = joinRoom.transform.GetComponent<Animator>();
        joinRoomAnim.SetBool("JoinRoom", false);
        GameObject curRoom = GameObject.Find("CurrentRoom");
        RectTransform rectCurRoom = curRoom.transform.GetComponent<RectTransform>();
        rectCurRoom.anchoredPosition = new Vector2(513, -513);
        //  LeaveRoom
        PhotonNetwork.LeaveRoom();
        print("Player Count  = " + PlayerListings.Count);
        print("index LeaveRoom : " + index);
        PlayerLeftRoom(PlayerListings[index].photonPlayer);
        print("===============================================================================");
    }

    public void ResetViewID(int index)
    {
        if (PhotonView.Find(index + 2))
        {
            PhotonView.Find(index + 2).OnDestroy();
            PhotonNetwork.UnAllocateViewID(index+2);
            //PlayerListings[index].photonView.view
            print("ViewID : "+PhotonView.Find(index + 2));
        }
    }

    public void UpdateViewID(int index)
    {
        PhotonPlayer[] player = PhotonNetwork.playerList;

        for (int i = 0; i < player.Length; i++)
        {
            if (i > index)
            {
                PlayerListings[i].UpdateViewID(index);
            }
        }

    }

    public void UpdateButtonAndIndex(int index)
    {
        print("======================= UpdateButtonAndIndex(" + index + ") =======================");
        PhotonPlayer[] player = PhotonNetwork.playerList;
        print("playerCount : "+player.Length);
        print("playerIndex : " + index);
        for (int i = 0; i < player.Length; i++)
        {
            //  ลบ Listener ของคนที่ออก เพื่อไม่ให้คนที่เพิ่มปุ่ม มีค่าใน parameter ตรงกับค่า index เดิมของคนที่ออก 
            if (i == index)
            {
                print("i = index : " + i + " = " + index);
                PlayerListings[index].UpdateButtonAndIndexForLeaveRoomPlayer(index);
            }

            //  ส่วน Update Listener
            //  ถ้าลูปทำงานมาถึงส่วนที่ i(ผู้เล่นที่ยังอยู่ในห้อง) มีค่า มากกว่า index(ผู้เล่นที่ออก) ให้ทำการลบ Listener ของผู้เล่นในห้องที่มีค่า i มากกว่า index แล้วเพิ่มปุ่มเข้าไปใหม่
            if (i > index)
            {
                print("i > index : "+i + " > " + index);
                    // [แก้แล้ว**]
                    // **ต้องแก้***ยังไม่เป็น net code เพราะการทำงานตอนนี้คือ เวลาSet false จะ false เฉพาะในผู้เล่นที่ i ที่เป็นหน้าจอของผู้เล่นคนที่ออกห้อง
                    // ส่วนหน้าจอของผู้เล่นคนนั้นจะยังคงเห็นตัวเองเป็น true อยู่ในกรณีกด Ready ไว้ แม้เป็นหัวห้องก็ตาม
                PlayerListings[i].UpdateButtonAndIndexForInRoomPlayer(index);
            }
        }
        print("===================================================================================");
    }
}
