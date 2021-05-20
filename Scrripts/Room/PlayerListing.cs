using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{
    public PhotonPlayer photonPlayer;

    [SerializeField]
    private TextMeshProUGUI _playerName;
    private TextMeshProUGUI playerName
    {
        get { return _playerName; }
    }

    CurrentRoomCanvas curRC = CurrentRoomCanvas.instance;
    public PhotonView photonView;
    private GameObject masterClientLogo;
    private Vector3 selfPos;
    public GameObject readyLogo;
    public Button startBtn;
    public Button readyBtn;
    public Button leaveBtn;
    public bool playerReady = false;
    public bool isMember = false;
    public bool isLockedCharacter;
    public int myCharacter;
    public Sprite[] selectCharFlag;
    public Color32[] color;

    int countOfPlayersInThisRooms = 0;

    private void Start()
    {
        photonHandler phoHand = photonHandler.instance;
        phoHand.time = 3;
    }
    private void Update()
    {
        countOfPlayersInThisRooms = CheckCountOfPlayersInThisRooms();
        if (PhotonNetwork.isMasterClient && (countOfPlayersInThisRooms > 1 ))
        {
            startBtn.interactable = true;
        }
        else if(PhotonNetwork.isMasterClient && (countOfPlayersInThisRooms <= 1))
        {
            startBtn.interactable = false;
        }

        if(!PhotonNetwork.isMasterClient && !isLockedCharacter)
        {
            readyBtn.interactable = false;
        }
        else if(!PhotonNetwork.isMasterClient && isLockedCharacter)
        {
            readyBtn.interactable = true;
        }
    }
    private int CheckCountOfPlayersInThisRooms()
    {
        PhotonPlayer[] players = PhotonNetwork.playerList;
        return players.Length;
    }
    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer)
    {
        photonView = GetComponent<PhotonView>();
        startBtn = CurrentRoomCanvas.instance.transform.GetChild(3).GetChild(1).GetComponent<Button>();
        readyBtn = CurrentRoomCanvas.instance.transform.GetChild(3).GetChild(0).GetComponent<Button>();
        leaveBtn = CurrentRoomCanvas.instance.transform.GetChild(3).GetChild(3).GetComponent<Button>();
        this.photonPlayer = photonPlayer;
        playerName.text = photonPlayer.NickName;
        masterClientLogo = transform.GetChild(1).GetChild(2).gameObject;
        readyLogo = transform.GetChild(1).GetChild(3).gameObject;
        readyLogo.SetActive(true);
        if(photonPlayer.IsLocal && photonPlayer.IsMasterClient)
            photonView.RPC("OnPlayerReadyTrue", PhotonTargets.AllBuffered);
        else if (photonPlayer.IsLocal && !photonPlayer.IsMasterClient)
            photonView.RPC("OnPlayerReadyFalse", PhotonTargets.AllBuffered);
    }

    public void UpdateViewID(int index)
    {
        int newIndex = index;
        newIndex--;
        photonView.viewID = newIndex;
    }
    public void UpdateButtonAndIndexForInRoomPlayer(int index)
    {
        //int newIndex = index;
        //newIndex--;
        readyBtn.onClick.RemoveAllListeners();
        leaveBtn.onClick.RemoveAllListeners();
        readyLogo.SetActive(false);
        //AddReadyButton(newIndex);
        //AddLeaveRoomBotton(newIndex);
    }
    public void UpdateButtonAndIndexForLeaveRoomPlayer(int index)
    {
        readyBtn.onClick.RemoveAllListeners();
        leaveBtn.onClick.RemoveAllListeners();
        readyLogo.SetActive(false);
    }

    public void ActiveMasterClientLogo(PhotonPlayer photonPlayer)
    {
        if (photonPlayer.IsMasterClient)
            ActivateMasterClientLogo();
        else
            DeactivateMasterClientLogo();
    }

    public void AddSelectStageButton()
    {
        print("AddSelectStageButton(MasterClient)");
        CurrentRoomCanvas.instance.selectStageBtn[0].onClick.AddListener(() => SelectStageLeftBtn());
        CurrentRoomCanvas.instance.selectStageBtn[1].onClick.AddListener(() => SelectStageRightBtn());
    }
    public void AddSelectTimeButton()
    {
        print("AddSelectTimeButton(MasterClient)");
        CurrentRoomCanvas.instance.selectTimeBtn[0].onClick.AddListener(() => SelectTimeLeftBtn());
        CurrentRoomCanvas.instance.selectTimeBtn[1].onClick.AddListener(() => SelectTimeRightBtn());
    }
    public void AddSelectCharacterButton()
    {
        print("AddSelectCharacterButton(All)");
        CurrentRoomCanvas.instance.selectCharacterBtn[0].onClick.AddListener(() => SelectCharacterLeftBtn());
        CurrentRoomCanvas.instance.selectCharacterBtn[1].onClick.AddListener(() => SelectCharacterRightBtn());
    }
    public void AddLockCharacterButton()
    {
        print("AddLockCharacterButton(All)");
        CurrentRoomCanvas.instance.lockBtn.onClick.AddListener(() => LockCharacterBtn());
    }
    public void AddStartButton()
    {
        print("AddStartButton(masterClient)");
        startBtn.onClick.AddListener(() => photonHandler.instance.SendingStageAndTimeIndex(curRC.stageIndex,curRC.timeIndex));
    }
    public void AddLeaveRoomBotton(int index)
    {
        print("AddLeaveRoomBotton(" + index + ")");
        leaveBtn.onClick.AddListener(() => PlayerLayoutGroup.instance.OnClickLeaveRoom(index));
    }
    public void AddReadyButton(int index)
    {
        print("photonPlayer : " + photonPlayer.NickName + " , index : " + index);
        readyBtn.onClick.AddListener(() => OnClickReadyButton(index));
        print(readyBtn);
    }
    private void OnClickReadyButton(int index)
    {
        print(this.photonPlayer.NickName + " On Click Ready BTN " + index);

        if (!playerReady)
        {
            readyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "cancle";
            readyLogo.GetComponent<Image>().color = color[0];
            playerReady = true;
            photonView.RPC("OnPlayerReadyTrue", PhotonTargets.AllBufferedViaServer, index);
        }
        else
        {
            readyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "ready";
            readyLogo.GetComponent<Image>().color = color[1];
            playerReady = false;
            photonView.RPC("OnPlayerReadyFalse", PhotonTargets.AllBufferedViaServer, index);
        }
    }
    private void SelectStageLeftBtn()
    {
        curRC.stageIndex--;

        if (curRC.stageIndex < 0)
        {
            curRC.stageIndex = curRC.stagePreImg.Length - 1;
            curRC.descriptionTexts[0].text = curRC.stagesDescription[curRC.stagesDescription.Length-1];
            SwitchStage(curRC.stageIndex, curRC.stagePreImg.Length);
        }
        else
        {
            curRC.descriptionTexts[0].text = curRC.stagesDescription[curRC.stageIndex];
            SwitchStage(curRC.stageIndex, curRC.stagePreImg.Length);
        }
        photonView.RPC("SwitchStage", PhotonTargets.AllBufferedViaServer, curRC.stageIndex, curRC.stagePreImg.Length);
    }
    private void SelectStageRightBtn()
    {
        curRC.stageIndex++;

        if (curRC.stageIndex > curRC.stagePreImg.Length - 1)
        {
            curRC.stageIndex = 0;
            curRC.descriptionTexts[0].text = curRC.stagesDescription[0];
            SwitchStage(curRC.stageIndex, curRC.stagePreImg.Length);
        }
        else
        {
            curRC.descriptionTexts[0].text = curRC.stagesDescription[curRC.stageIndex];
            SwitchStage(curRC.stageIndex, curRC.stagePreImg.Length);
        }
        photonView.RPC("SwitchStage", PhotonTargets.AllBufferedViaServer, curRC.stageIndex, curRC.stagePreImg.Length);
    }
    private void SelectTimeLeftBtn()
    {
        curRC.timeIndex--;

        if (curRC.timeIndex < 0)
        {
            curRC.timeIndex = curRC.timeTag.Length - 1;
            curRC.descriptionTexts[1].text = curRC.timesDescription[curRC.timesDescription.Length - 1];
            SwitchTime(curRC.timeIndex);
        }
        else
        {
            curRC.descriptionTexts[1].text = curRC.timesDescription[curRC.timeIndex];
            SwitchTime(curRC.timeIndex);
        }
        photonView.RPC("SwitchTime", PhotonTargets.AllBufferedViaServer, curRC.timeIndex);
    }
    private void SelectTimeRightBtn()
    {
        curRC.timeIndex++;

        if (curRC.timeIndex > curRC.timeTag.Length - 1)
        {
            curRC.timeIndex = 0;
            curRC.descriptionTexts[1].text = curRC.timesDescription[0];
            SwitchTime(curRC.timeIndex);
        }
        else
        {
            curRC.descriptionTexts[1].text = curRC.timesDescription[curRC.timeIndex];
            SwitchTime(curRC.timeIndex);
        }
        photonView.RPC("SwitchTime", PhotonTargets.AllBufferedViaServer, curRC.timeIndex);
    }
    private void SelectCharacterLeftBtn()
    {
        curRC.characterIndex--;

        if (curRC.characterIndex < 0)
        {
            curRC.characterIndex = curRC.characterPreImg.Length - 1;
            curRC.descriptionTexts[2].text = curRC.charactersDescription[curRC.charactersDescription.Length-1];
            SwitchCharacter(curRC.characterIndex, curRC.characterPreImg.Length);
        }
        else
        {
            curRC.descriptionTexts[2].text = curRC.charactersDescription[curRC.characterIndex];
            SwitchCharacter(curRC.characterIndex, curRC.characterPreImg.Length);
        }
        myCharacter = curRC.characterIndex;
    }
    private void SelectCharacterRightBtn()
    {
        curRC.characterIndex++;

        if (curRC.characterIndex > curRC.characterPreImg.Length - 1)
        {
            curRC.characterIndex = 0;
            curRC.descriptionTexts[2].text = curRC.charactersDescription[0];
            SwitchCharacter(curRC.characterIndex, curRC.characterPreImg.Length);
        }
        else
        {
            curRC.descriptionTexts[2].text = curRC.charactersDescription[curRC.characterIndex];
            SwitchCharacter(curRC.characterIndex, curRC.characterPreImg.Length);
        }
        myCharacter = curRC.characterIndex;
    }
    private void SwitchCharacter(int index, int preImgLength)
    {
        for (int i = 0; i < preImgLength; i++)
        {
            if (i == index)
            {
                curRC.characterPreImg[index].SetActive(true);
                curRC.textCharacter.text = curRC.characterTag[index];
            }
            else
            {
                curRC.characterPreImg[i].SetActive(false);
                curRC.textCharacter.text = curRC.characterTag[index];
            }
        }
    }
    private void LockCharacterBtn()
    {
        if (!curRC.isLock[curRC.characterIndex] && !isLockedCharacter)
        {
            photonView.RPC("LockCharacter", PhotonTargets.AllBufferedViaServer, curRC.characterIndex, curRC.characterPreImg.Length);
            curRC.selectCharacterBtn[0].gameObject.SetActive(false);
            curRC.selectCharacterBtn[1].gameObject.SetActive(false);
            curRC.lockBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = curRC.masterKeyTag[1];
            curRC.lockBtn.transform.GetChild(1).gameObject.SetActive(false);
            curRC.lockBtn.transform.GetChild(2).gameObject.SetActive(true);
            isLockedCharacter = true;
            myCharacter = curRC.characterIndex;
            photonHandler.instance.characterPlayerIndex = this.myCharacter;
            photonHandler.instance.photonPlayer = photonPlayer;
        }
        else if (curRC.isLock[curRC.characterIndex] && isLockedCharacter)
        {
            //UnLockCharacter(curRC.characterIndex, curRC.characterPreImg.Length);
            photonView.RPC("UnLockCharacter", PhotonTargets.AllBufferedViaServer, curRC.characterIndex, curRC.characterPreImg.Length);
            curRC.selectCharacterBtn[0].gameObject.SetActive(true);
            curRC.selectCharacterBtn[1].gameObject.SetActive(true);
            curRC.lockBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = curRC.masterKeyTag[0];
            curRC.lockBtn.transform.GetChild(1).gameObject.SetActive(true);
            curRC.lockBtn.transform.GetChild(2).gameObject.SetActive(false);
            isLockedCharacter = false;
        }
        else if (curRC.isLock[curRC.characterIndex] && !isLockedCharacter)
        {
            //animation 
        }

    }

    #region DefaultCharacterGeneratorSystem
    /*
    [PunRPC]
    private void DefaultCharacterGenerator()
    {

        /*if (!isLockedCharacter)
        {
            if (curRC.characterListings.Count > 0)
            {
                int characterIndexInList;
                int rndCharacterIndex;

                do
                {
                    for (int i = 0; i < curRC.characterListings.Count; i++)
                    {
                        characterIndexInList = curRC.characterListings.IndexOf(curRC.characterListings[i]);
                    }
                    characterIndex = Random.Range(0, curRC.characterListings.Count - 1);
                }
                while (rndCharacterIndex == characterIndexInList);
                
                    index = curRC.characterListings.IndexOf(curRC.characterListings[i]);
                    characterIndex = Random.Range(0, curRC.characterListings.Count - 1);
                    myCharacter = curRC.characterListings[characterIndex];
                    RemoveUnlockCharacterToList(characterIndex);
                    photonView.RPC("RemoveUnlockCharacterToList", PhotonTargets.AllBufferedViaServer, characterIndex);
                    isLockedCharacter = true;
                
            }
            else if (curRC.characterListings.Count == 1)
            {
                myCharacter = curRC.characterListings[0];
                RemoveUnlockCharacterToList(0);
                photonView.RPC("RemoveUnlockCharacterToList", PhotonTargets.AllBufferedViaServer, 0);
                isLockedCharacter = true;
            }
            else if (curRC.characterListings.Count == 0)
            {
                for(int i = 0; i < curRC.isLock.Length; i++ )
                {
                    if(curRC.isLock[i] == false)
                    {
                        myCharacter = i;
                    }
                }
            }

        }
    }
    [PunRPC]
    private void SortCharacterInList()
    {
        /*if(curRC.characterListings.Count == 0)
        {
            if (curRC.isLock[i] == false)
            {
                int index = curRC.characterListings.IndexOf(curRC.characterListings[i]);
            }
        }
        for (int i = 0; i < curRC.characterPreImg.Length; i++)
        {
            if (curRC.characterListings.Count > 0)
            {
                if (isLockedCharacter && myCharacter == curRC.characterListings[i])
                {
                    int index = curRC.characterListings.IndexOf(curRC.characterListings[i]);
                    photonView.RPC("RemoveUnlockCharacterToList", PhotonTargets.AllBufferedViaServer, index);
                }
            }
            else if(curRC.characterListings.Count == 0)
            {
                if (curRC.isLock[i] == false)
                {
                    photonView.RPC("AddUnlockCharacterToList", PhotonTargets.AllBufferedViaServer, i);
                }
            }
        }
    }
    */
    #endregion
    [PunRPC]
    private void OnPlayerReadyTrue()
    {
        readyLogo.GetComponent<Image>().color = color[0];
    }
    [PunRPC]
    private void OnPlayerReadyFalse()
    {
        readyLogo.GetComponent<Image>().color = color[1];
        playerReady = false;
    }
    [PunRPC]
    private void OnPlayerReadyTrue(int index)
    {
        print("[T] RPC index parameters : " + index);
        readyLogo.GetComponent<Image>().color = color[0];
        playerReady = true;

    }
    [PunRPC]
    private void OnPlayerReadyFalse(int index)
    {
        print("[F] RPC index parameters : " + index);
        readyLogo.GetComponent<Image>().color = color[1];
        playerReady = false;
    }

    [PunRPC]
    private void SwitchStage(int index,int preImgLength)
    {
        for(int i = 0; i < preImgLength ; i++)
        {
            if(i == index)
            {
                curRC.stagePreImg[index].SetActive(true);
                curRC.textStage.text = curRC.stageTag[index];
                curRC.stageIndex = index;
                curRC.descriptionTexts[0].text = curRC.stagesDescription[index];
            }
            else
            {
                curRC.stagePreImg[i].SetActive(false);
                curRC.textStage.text = curRC.stageTag[index];
            }
        }
    }
    [PunRPC]
    private void SwitchTime(int index)
    {
        photonHandler phoHand = photonHandler.instance;
        curRC.textTime.text = curRC.timeTag[index] + " " + "MINUTES";
        curRC.timePre.text = curRC.timeTag[index];
        phoHand.time = float.Parse(curRC.timeTag[index]);
        curRC.descriptionTexts[1].text = curRC.timesDescription[index];
    }
    [PunRPC]
    private void LockCharacter(int index,int preImgLength)
    {
        isLockedCharacter = true;
        curRC.isLock[index] = true;
        curRC.characterPreImg[index].GetComponent<Image>().color = Color.black;
        curRC.characterPreImg[index].transform.GetChild(0).gameObject.SetActive(true);
        GameObject characterLock = transform.GetChild(1).GetChild(1).gameObject;
        Image imgCharLock = characterLock.GetComponent<Image>();
        characterLock.SetActive(true);
        imgCharLock.sprite = selectCharFlag[index];
        myCharacter = index;
    }
    [PunRPC]
    private void UnLockCharacter(int index, int preImgLength)
    {
        isLockedCharacter = false;
        curRC.isLock[index] = false;
        curRC.characterPreImg[index].GetComponent<Image>().color = Color.white;
        curRC.characterPreImg[index].transform.GetChild(0).gameObject.SetActive(false);
        GameObject characterLock = transform.GetChild(1).GetChild(1).gameObject;
        Image imgCharLock = characterLock.GetComponent<Image>();
        characterLock.SetActive(false);
        imgCharLock.sprite = null;
    }

    public void ActivateMasterClientLogo()
    {
        masterClientLogo.SetActive(true);
        isMember = false;
    }
    public void DeactivateMasterClientLogo()
    {
        masterClientLogo.SetActive(false);
        isMember = true;
    }
    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            selfPos = (Vector3)stream.ReceiveNext();
        }
    }
}
