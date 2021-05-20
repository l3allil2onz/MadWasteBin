using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CurrentRoomCanvas : MonoBehaviour
{

    //  btn0 = ready
    //  btn1 = start
    //  btn2 = start(delayed)
    //  btn3 = leave

    //  btn4 = Left Select Stage 
    //  btn5 = Right Select Stage
    //  btn6 = Left Select time
    //  btn7 = Right Select time

    //  SelectStage
    public GameObject[] stagePreImg;
    public int stageIndex = 0;
    public TextMeshProUGUI textStage;
    public string[] stageTag = { "City", "Beach", "Forest" };
    public Button[] selectStageBtn = new Button[2];
    //  SelectTime
    public TextMeshProUGUI timePre;
    public TextMeshProUGUI textTime;
    public int timeIndex = 0;
    public string[] timeTag = { "3", "6", "10" };
    public Button[] selectTimeBtn = new Button[2];
    //  SelectCharacter
    public GameObject[] characterPreImg;
    public int characterIndex = 0;
    public TextMeshProUGUI textCharacter;
    public string[] characterTag = { "general Waste Bin", "hazardous Waste Bin", "organic Waste Bin", "recycle Waste Bin" };
    public string[] masterKeyTag = { "Lock", "Unlock"};
    public Button[] selectCharacterBtn = new Button[2];
    public Button lockBtn;
    public bool[] isLock;

    public GameObject blockRayCast_NotLockedChar;
    public GameObject blockRayCast_NotReady;
    public static GameObject[] btn = new GameObject[4];
    public static GameObject[] selectBtnGroup = new GameObject[2];
    public static GameObject[] selectBtn = new GameObject[4];
    public static GameObject btnGroup;
    public static CurrentRoomCanvas instance;
    //text descript
    public TextMeshProUGUI[] descriptionTexts;
    public string[] stagesDescription;
    public string[] timesDescription;
    public string[] charactersDescription;

    public void Awake()
    {
        stagesDescription[0] = "The city is a large human settlement cities generally have extensive systems for housing, therefore cannot avoid having garbage.";
        timesDescription[0] = "Choose three minutes if you want to finish the game faster.";
        charactersDescription[0] = "The mad general waste bin is an evolution between rat and general waste bin, rats have been used in many scientific studie and diseases and they are awaiting for day of come to revenge on humans.";
        stagesDescription[1] = "The Beach is a landform alongside a body of water which consists of sand and rocks, this is a place that people like to enjoy in the summer and often abounding with garbage.";
        timesDescription[1] = "six minutes suitable for playing for fun.";
        charactersDescription[1] = "The mad harzadous waste bin is an evolution between centipedes and harzadous waste bin, centipedes are predominantly carnivorous, they like to live in areas with moisture such as trash.";
        stagesDescription[2] = "The forest is a large area dominated by trees, the people find it to seek tranquility, the garbage can be found when people live in near the forest.";
        timesDescription[2] = "ten minutes if you want to play continuously.";
        charactersDescription[2] = "The mad organic waste bin is an evolution between flies and organic waste bin, flies are carriers of the disease due to impurities that cause pathogens to follow the legs";
        charactersDescription[3] = "The mad recycle waste bin is an evolution between vulture and recycle waste bin, vultures are scavengers, meaning they eat dead animals. When a carcass has too thick a hide for its beak to open, it waits for a larger scavenger to eat first";

        instance = this;
        btnGroup = GameObject.Find("BtnGroupCurrentRoom");

        for (int i = 0; i < btn.Length; i++)
        {
            btn[i] = btnGroup.transform.GetChild(i).gameObject;
        }
        GetSelectBtn();
    }

    private void GetSelectBtn()
    {
        selectBtnGroup[0] = GameObject.Find("SelectStageGroup");
        selectBtnGroup[1] = GameObject.Find("SelectTimeGroup");
        selectBtn[0] = selectBtnGroup[0].transform.GetChild(1).gameObject;
        selectBtn[1] = selectBtnGroup[0].transform.GetChild(2).gameObject;
        selectBtn[2] = selectBtnGroup[1].transform.GetChild(1).gameObject;
        selectBtn[3] = selectBtnGroup[1].transform.GetChild(2).gameObject;

    }

    public void BecomeAHost()
    {
        btn[0].SetActive(false);
        btn[1].SetActive(true);
        btn[2].SetActive(false);
        //btn[3].SetActive(true);
        //  demo
        btn[3].SetActive(false);
        selectBtn[0].SetActive(true);
        selectBtn[1].SetActive(true);
        selectBtn[2].SetActive(true);
        selectBtn[3].SetActive(true);
    }
    public void BecomeAMember()
    {
        btn[0].SetActive(true);
        btn[1].SetActive(false);
        btn[2].SetActive(false);
        //btn[3].SetActive(true);
        //  demo
        btn[3].SetActive(false);
        selectBtn[0].SetActive(false);
        selectBtn[1].SetActive(false);
        selectBtn[2].SetActive(false);
        selectBtn[3].SetActive(false);
    }
    public bool CheckReadyPlayer()
    {
        PhotonPlayer[] player = PhotonNetwork.playerList;
        PlayerLayoutGroup plg = PlayerLayoutGroup.instance;
        int readyCount = 0;

        for(int i = 0; i < player.Length;i++)
        {
            if(i > 0)
            {
                if(plg.PlayerListings[i].playerReady == true)
                {
                    readyCount++;
                }
            }
        }
        //  << Example >>
        // 1master + 2member = 3 players (2 players have ready btn) => 0
        // 1master + 1ready member + 1not ready member = 3 players (2 players have ready btn) (1 ready) => 1
        // 1master + 2ready member = 3 players (2 players have ready btn) (2 ready) => 2
        //  2 == (3-1) 
        if (readyCount == player.Length - 1)
        {
            return true;
        }
        else
        {
            if (!CheckPlayersAreLockedCharacters())
            {
                blockRayCast_NotLockedChar.SetActive(true);
                                return false;
            }
            else
            {
                blockRayCast_NotReady.SetActive(true);
                return false;
            }
        }
    }
    public bool CheckPlayersAreLockedCharacters()
    {
        PhotonPlayer[] player = PhotonNetwork.playerList;
        PlayerLayoutGroup plg = PlayerLayoutGroup.instance;
        int lockCount = 0;

        for (int i = 0; i < player.Length; i++)
        {
            if (plg.PlayerListings[i].isLockedCharacter == true)
            {
                lockCount++;
            }
        }
        //  << Example >>
        // 1master + 2member = 3 players (2 players have ready btn) => 0
        // 1master + 1ready member + 1not ready member = 3 players (2 players have ready btn) (1 ready) => 1
        // 1master + 2ready member = 3 players (2 players have ready btn) (2 ready) => 2
        //  2 == (3-1) 
        if (lockCount == player.Length)
        {
            return true;
        }
        else
        {
            blockRayCast_NotLockedChar.SetActive(true);
            return false;
        }
    }
    public int CheckPlayersCharacters()
    {
        PhotonPlayer[] player = PhotonNetwork.playerList;
        PlayerLayoutGroup plg = PlayerLayoutGroup.instance;

        for (int i = 0; i < player.Length; i++)
        {
            print(plg.PlayerListings[i] + " myCharacter : " + plg.PlayerListings[i].myCharacter);
            if(plg.PlayerListings[i].myCharacter == characterIndex)
            {
                return characterIndex;
            }
            /*if (plg.PlayerListings[i].myCharacter == 0)
            {
                return 0;
            }
            else if (plg.PlayerListings[i].myCharacter == 1)
            {
                return 1;
            }
            else if (plg.PlayerListings[i].myCharacter == 2)
            {
                return 2;
            }
            else if (plg.PlayerListings[i].myCharacter == 3)
            {
                return 3;
            }*/
        }
        
        return 0;
    }
    public void CloseNoticePlayerNotReady()
    {
        blockRayCast_NotReady.SetActive(false);
    }
    public void CloseNoticePlayerNotLockedCharacter()
    {
        blockRayCast_NotLockedChar.SetActive(false);
    }
    public void OnClickStartSync()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
