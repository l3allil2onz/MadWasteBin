using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GameManager : MonoBehaviour, IPunObservable
{
    public static GameManager instance;
    GameObject spawnPoint;
    public PhotonView photonView;
    public float gameTime;
    public float curTime;
    //เพิ่มตัวจับเวลา
    private float timerAdd = 0.0f;
    public bool isStartGame = false;
    public GameObject startGameCountDownPanel;
    public TextMeshProUGUI timeCountDownTextMP_UGUI;
    float timeCountDown = 3;

    //ตัวจบเกม
    public GameObject extendTheTimeLimitPanel;
    float extendTime = 15;
    public bool isExtendTime = false;

    public GameObject timeIsUpObj;
    public TextMeshProUGUI timeTextMP_UGUI;

    // prefab pool
    private Queue<GameObject> pool;
    //ขยะแต่ละประเภท
    public GameObject[] generalTrash = new GameObject[2];
    public GameObject[] harzadousTrash = new GameObject[2];
    public GameObject[] organicTrash = new GameObject[2];
    public GameObject[] recycleTrash = new GameObject[2];
    //คะแนน
    public GameObject scorePlayerPrefab;
    public GameObject playerScoreList;
    public List<GameObject> scorePlayersObj = new List<GameObject>();
    public List<int> scorePlayerInt = new List<int>();
    //spawn item
    public int gTrashCount, gMaxTrashCount;
    public int hTrashCount, hMaxTrashCount;
    public int oTrashCount, oMaxTrashCount;
    public int rTrashCount, rMaxTrashCount;
    public float maxTimeReSpawn, minTimeReSpawn;
    public List<GameObject> gTrashListing = new List<GameObject>();
    public List<GameObject> hTrashListing = new List<GameObject>();
    public List<GameObject> oTrashListing = new List<GameObject>();
    public List<GameObject> rTrashListing = new List<GameObject>();

    photonHandler phoHandler = photonHandler.instance;

    private void Awake()
    {
        instance = this;
        spawnPoint = GameObject.Find("SpawnPoint");
        photonView = GetComponent<PhotonView>();
        timeIsUpObj.SetActive(false);
        startGameCountDownPanel.SetActive(true);

        pool = new Queue<GameObject>();
        //PhotonNetwork.PrefabPool = this;
    }
    private void Start()
    {
        curTime = gameTime;
        if (PhotonNetwork.isMasterClient)
        {
            InvokeRepeating("ReSpawningWithTime", 0, 0.5f);
        }
        TimeSpan timespanIngame = new TimeSpan(0, 0, (int)gameTime);
        timeTextMP_UGUI.text = (timespanIngame.Minutes).ToString(timespanIngame.Minutes < 10 ? "0" + "#" : "##") + ":" +
                               (timespanIngame.Seconds).ToString(timespanIngame.Seconds < 10 ? "0" + "#" : "##");
        //AddScorePlayerObjToInt();
    }
    private void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (curTime <= 0)
            {
                if (!timeIsUpObj.activeSelf)
                {
                    timeIsUpObj.SetActive(true);
                }
                if (extendTheTimeLimitPanel.activeSelf)
                {
                    extendTheTimeLimitPanel.SetActive(false);
                }
                curTime = 0;
            }
            else if(curTime > 0)
            {
                if (isStartGame)
                {
                    if (startGameCountDownPanel.activeSelf)
                    {
                        startGameCountDownPanel.SetActive(false);
                    }
                    //time
                    curTime -= Time.deltaTime;
                    TimeSpan timespanIngame = new TimeSpan(0, 0, (int)curTime);
                    //(ภายในมีตัวจับเวลาที่คูณกับ จำนวนของTickต่อวินาที) ทำให้วินาทีของเวลาเร็วขึ้น
                    //long เก็บค่าเป็นจำนวนเต็มที่มีจำนวนไบต์เป็น 2 เท่า ของจำนวนเดิม(2,147,483,649) 2พันล้านตำแหน่ง
                    //TicksPerSecond มี10ล้านตำแหน่ง
                    timespanIngame -= new TimeSpan(((long)timerAdd + TimeSpan.TicksPerSecond));
                    timeTextMP_UGUI.text = (timespanIngame.Minutes).ToString(timespanIngame.Minutes < 10 ? "0" + "#" : "##") + ":" +
                                           (timespanIngame.Seconds).ToString(timespanIngame.Seconds < 10 ? "0" + "#" : "##");
                    //SortRankOfPlayerScore();
                    if (curTime < 10 && !isExtendTime)
                    {
                        extendTheTimeLimitPanel.SetActive(true);
                    }
                }
                else
                {
                    StartGameCountDown();
                }
            }
        }
        else if(!PhotonNetwork.isMasterClient)
        {
            if (curTime < 0)
            {
                if (!timeIsUpObj.activeSelf)
                {
                    timeIsUpObj.SetActive(true);
                }
                if (extendTheTimeLimitPanel.activeSelf)
                {
                    extendTheTimeLimitPanel.SetActive(false);
                }
                curTime = 0;
            }
            else
            {
                if (isStartGame)
                {
                    if (startGameCountDownPanel.activeSelf)
                    {
                        startGameCountDownPanel.SetActive(false);
                    }
                    //time
                    curTime -= Time.deltaTime;
                    TimeSpan timespanIngame = new TimeSpan(0, 0, (int)curTime);
                    //(ภายในมีตัวจับเวลาที่คูณกับ จำนวนของTickต่อวินาที) ทำให้วินาทีของเวลาเร็วขึ้น
                    //long เก็บค่าเป็นจำนวนเต็มที่มีจำนวนไบต์เป็น 2 เท่า ของจำนวนเดิม(2,147,483,649) 2พันล้านตำแหน่ง
                    //TicksPerSecond มี10ล้านตำแหน่ง
                    timespanIngame -= new TimeSpan(((long)timerAdd + TimeSpan.TicksPerSecond));
                    timeTextMP_UGUI.text = (timespanIngame.Minutes).ToString(timespanIngame.Minutes < 10 ? "0" + "#" : "##") + ":" +
                                           (timespanIngame.Seconds).ToString(timespanIngame.Seconds < 10 ? "0" + "#" : "##");
                    //SortRankOfPlayerScore();
                    if (curTime < 10 && !isExtendTime)
                    {
                        extendTheTimeLimitPanel.SetActive(true);
                    }
                }
                else
                {
                    StartGameCountDown();
                }
            }
        }

    }

    private void AddScorePlayers()
    {
        print("playerScoreCount = " + playerScoreList.transform.childCount);
        if (playerScoreList.transform.childCount > 0)
        {
            print("AddScorePlayers : Active");
            GameObject[] playerScore = new GameObject[playerScoreList.transform.childCount];
            for (int i = 0; i < playerScoreList.transform.childCount; i++)
            {
                playerScore[i] = playerScoreList.transform.GetChild(i).gameObject;
                scorePlayersObj.Add(playerScore[i]);
            }
            //photonView.RPC("ScorePlayerSetting", PhotonTargets.AllBufferedViaServer);
        }
    }
    /*
    private string SetTagCharacterPlayer(int index)
    {
        if (index == 0)
            return "G";
        else if (index == 1)
            return "H";
        else if (index == 2)
            return "O";
        else if (index == 3)
            return "R";
        else
            return "?";
    }
    
    private void InstantiateScorePlayer()
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonPlayer[] players = PhotonNetwork.playerList;
            for (int i = 0; i < players.Length; i++)
            {
                GameObject scorePlayer = PhotonNetwork.Instantiate(scorePlayerPrefab.name,
                                                               scorePlayerPrefab.transform.position,
                                                               scorePlayerPrefab.transform.rotation, 0);
                PlayerScore ps = scorePlayer.GetComponent<PlayerScore>();
                phoHandler.SendCharacterIndex(ps);
            }
        }
    }
    [PunRPC]
    private void DestroyScorePlayer()
    {
        if (playerScoreList.transform.childCount > 0)
        {
            print("DestroyScorePlayer : Active");
            for (int j = 0; j < playerScoreList.transform.childCount; j++)
            {
                PhotonNetwork.Destroy(playerScoreList.transform.GetChild(j).gameObject);
            }
        }
    }
    [PunRPC]
    private void ScorePlayerSetting()
    {

        print("ScorePlayerSetting : Active");
        PhotonPlayer[] players = PhotonNetwork.playerList;
        for (int i = 0; i < players.Length; i++)
        {
            for (int j = 0; j < playerScoreList.transform.childCount; j++)
            {
                if (players[i].NickName == playerScoreList.transform.GetChild(j).GetComponent<PlayerScore>().photonPlayer.NickName)
                {
                    //PlayerScore playerScores = playerScoreList.transform.GetChild(j).GetComponent<PlayerScore>();
                    // จัด rank ตอนเริ่ม ครั้งเดียว
                    //playerScores.rank = playerScores.characterPlayerIndex;
                    break;
                }
            }
        }

    }
    */
    private void StartGameCountDown()
    {
        if (timeCountDown < 0)
        {
            if (gameTime == 3)
            {
                gameTime = 180;
                curTime = gameTime;
            }
            else if (gameTime == 6)
            {
                gameTime = 360;
                curTime = gameTime;
            }
            else if (gameTime == 10)
            {
                gameTime = 600;
                curTime = gameTime;
            }
            AddScorePlayers();
            isStartGame = true;
        }
        else
        {
            timeCountDown -= Time.deltaTime;
            timeCountDownTextMP_UGUI.text = timeCountDown.ToString("#");
        }
    }
    public void ExtendTime()
    {
        photonView.RPC("ExtendTimeRPC", PhotonTargets.AllBufferedViaServer);
    }
    [PunRPC]
    public void ExtendTimeRPC()
    {
        curTime += extendTime;
        isExtendTime = true;
        extendTheTimeLimitPanel.SetActive(false);
    }

    
    /*public new GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        int trash = UnityEngine.Random.Range(0, 2);
        float rndPos = UnityEngine.Random.Range(-700, 700);
        if (pool.Count > 0)
        {
            GameObject go = pool.Dequeue();
            go.transform.SetParent(spawnPoint.transform);
            go.transform.localPosition = position;
            go.transform.localScale = SendTrashPrefabLocalScale(go);
            go.SetActive(true);

            return go;
        }
  
        return Instantiate(prefabId, position, rotation);
    }

    public void Destroy(GameObject gameObject)
    {
        gameObject.SetActive(false);
        pool.Enqueue(gameObject);
    }*/

    private string SendGTrashPrefabName()
    {
        for (int i = 0; i < 2; i++)
        {
            if (!GameObject.Find(generalTrash[i].name + "(Clone)") && !GameObject.Find("GT"+i + "(Clone)") && !GameObject.Find("GeneralTrash" + i + "Img"))
            {
                return generalTrash[i].name;
            }
        }
        return null;
    }
    private string SendHTrashPrefabName()
    {
        for (int i = 0; i < 2; i++)
        {
            if (!GameObject.Find(harzadousTrash[i].name + "(Clone)") && !GameObject.Find("HT" + i + "(Clone)") && !GameObject.Find("HarzadousTrash" + i + "Img"))
            {
                return harzadousTrash[i].name;
            }
        }
        return null;
    }
    private string SendOTrashPrefabName()
    {
        for (int i = 0; i < 2; i++)
        {
            if (!GameObject.Find(organicTrash[i].name + "(Clone)") && !GameObject.Find("OT" + i + "(Clone)") && !GameObject.Find("OrganicTrash" + i + "Img"))
            {
                return organicTrash[i].name;
            }
        }
        return null;
    }
    private string SendRTrashPrefabName()
    {
        for (int i = 0; i < 2; i++)
        {
            if (!GameObject.Find(recycleTrash[i].name + "(Clone)") && !GameObject.Find("RT" + i + "(Clone)") && !GameObject.Find("RecycleTrash" + i + "Img"))
            {
                return recycleTrash[i].name;
            }
        }
        return null;
    }
    private Vector3 SendTrashPrefabLocalScale(GameObject go)
    {
        for(int i = 0; i < 2; i++)
        {
            if(go.name == generalTrash[i].name+"(Clone)")
            {
                return generalTrash[i].transform.localScale;
            }
            else if (go.name == harzadousTrash[i].name + "(Clone)")
            {
                return harzadousTrash[i].transform.localScale;
            }
            else if (go.name == organicTrash[i].name + "(Clone)")
            {
                return organicTrash[i].transform.localScale;
            }
            else if (go.name == recycleTrash[i].name + "(Clone)")
            {
                return recycleTrash[i].transform.localScale;
            }
        }
        Vector3 nullObjName = Vector3.one;
        return nullObjName;
    }
    
    private void ReSpawningWithTime()
    {
        float waitTime = UnityEngine.Random.Range(minTimeReSpawn, maxTimeReSpawn);
        StartCoroutine(Spawning(waitTime));
    }
    
    public IEnumerator Spawning(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        RandomTrash();
    }
    private void RandomTrash()
    {
        for (int i = 0; i < 4; i++)
        {
            int trash = UnityEngine.Random.Range(0, 2);
            float rndPos = UnityEngine.Random.Range(-700, 700);

            if (i == 0 && gTrashCount < gMaxTrashCount)
            {
                int newRndTrashIndex = trash;
                if (!GameObject.Find(generalTrash[trash].name + "(Clone)"))
                {
                    //photonView.RPC("GeneralTrashGenerator", PhotonTargets.AllViaServer, trash, rndPos);
                    GeneralTrashGenerator(trash, rndPos);
                }
                else
                {
                    while (newRndTrashIndex == trash)
                    {
                        newRndTrashIndex = UnityEngine.Random.Range(0, 2);
                    }
                    GeneralTrashGenerator(newRndTrashIndex, rndPos);
                }

            }
            else if (i == 1 && hTrashCount < hMaxTrashCount)
            {
                int newRndTrashIndex = trash;
                if (!GameObject.Find(harzadousTrash[trash].name + "(Clone)"))
                {
                    //photonView.RPC("HarzadousTrashGenerator", PhotonTargets.AllViaServer, trash, rndPos);
                    HarzadousTrashGenerator(trash, rndPos);
                }
                else
                {
                    while (newRndTrashIndex == trash)
                    {
                        newRndTrashIndex = UnityEngine.Random.Range(0, 2);
                    }
                    HarzadousTrashGenerator(newRndTrashIndex, rndPos);
                }
            }
            else if (i == 2 && oTrashCount < oMaxTrashCount)
            {
                int newRndTrashIndex = trash;
                if (!GameObject.Find(organicTrash[trash].name + "(Clone)"))
                {
                    //photonView.RPC("OrganicTrashGenerator", PhotonTargets.AllViaServer, trash, rndPos);
                    OrganicTrashGenerator(trash, rndPos);
                }
                else
                {
                    while (newRndTrashIndex == trash)
                    {
                        newRndTrashIndex = UnityEngine.Random.Range(0, 2);
                    }
                    OrganicTrashGenerator(newRndTrashIndex, rndPos);
                }
            }
            else if (i == 3 && rTrashCount < rMaxTrashCount)
            {
                int newRndTrashIndex = trash;
                if (!GameObject.Find(recycleTrash[trash].name + "(Clone)"))
                {
                    //photonView.RPC("RecycleTrashGenerator", PhotonTargets.AllViaServer, trash, rndPos);
                    RecycleTrashGenerator(trash, rndPos);
                }
                else
                {
                    while (newRndTrashIndex == trash)
                    {
                        newRndTrashIndex = UnityEngine.Random.Range(0, 2);
                    }
                    RecycleTrashGenerator(newRndTrashIndex, rndPos);
                }
            }
        }
    }
    [PunRPC]
    private void SettingGTrash(string trashName,int ranTrash,float rndPos)
    {
        GameObject thisTrash = GameObject.Find(trashName);
        thisTrash.transform.SetParent(spawnPoint.transform);
        thisTrash.transform.localScale = generalTrash[ranTrash].transform.localScale;
        thisTrash.transform.localPosition = new Vector3(generalTrash[ranTrash].transform.localPosition.x + rndPos,
                                                        generalTrash[ranTrash].transform.localPosition.y,
                                                        generalTrash[ranTrash].transform.localPosition.z);
    }
    [PunRPC]
    private void SettingHTrash(string trashName, int ranTrash, float rndPos)
    {
        GameObject thisTrash = GameObject.Find(trashName);
        thisTrash.transform.SetParent(spawnPoint.transform);
        thisTrash.transform.localScale = harzadousTrash[ranTrash].transform.localScale;
        thisTrash.transform.localPosition = new Vector3(harzadousTrash[ranTrash].transform.localPosition.x + rndPos,
                                                        harzadousTrash[ranTrash].transform.localPosition.y,
                                                        harzadousTrash[ranTrash].transform.localPosition.z);
    }
    [PunRPC]
    private void SettingOTrash(string trashName, int ranTrash, float rndPos)
    {
        GameObject thisTrash = GameObject.Find(trashName);
        thisTrash.transform.SetParent(spawnPoint.transform);
        thisTrash.transform.localScale = organicTrash[ranTrash].transform.localScale;
        thisTrash.transform.localPosition = new Vector3(organicTrash[ranTrash].transform.localPosition.x + rndPos,
                                                        organicTrash[ranTrash].transform.localPosition.y,
                                                        organicTrash[ranTrash].transform.localPosition.z);
    }
    [PunRPC]
    private void SettingRTrash(string trashName, int ranTrash, float rndPos)
    {
        GameObject thisTrash = GameObject.Find(trashName);
        thisTrash.transform.SetParent(spawnPoint.transform);
        thisTrash.transform.localScale = recycleTrash[ranTrash].transform.localScale;
        thisTrash.transform.localPosition = new Vector3(recycleTrash[ranTrash].transform.localPosition.x + rndPos,
                                                        recycleTrash[ranTrash].transform.localPosition.y,
                                                        recycleTrash[ranTrash].transform.localPosition.z);
    }
    public void GeneralTrashGenerator(int ranTrash, float rndPos)
    {
        if (gTrashCount < gMaxTrashCount)
        {

            GameObject thisTrash = PhotonNetwork.Instantiate(generalTrash[ranTrash].name, spawnPoint.transform.localPosition, spawnPoint.transform.localRotation, 0);
            photonView.RPC("SettingGTrash", PhotonTargets.AllBufferedViaServer, thisTrash.name, ranTrash, rndPos);
            AddGTrash(thisTrash);
        }

    }
    public void HarzadousTrashGenerator(int ranTrash, float rndPos)
    {
        if (hTrashCount < hMaxTrashCount)
        {
            GameObject thisTrash = PhotonNetwork.Instantiate(harzadousTrash[ranTrash].name, spawnPoint.transform.localPosition, spawnPoint.transform.localRotation, 0);
            photonView.RPC("SettingHTrash", PhotonTargets.AllBufferedViaServer, thisTrash.name, ranTrash, rndPos);
            AddHTrash(thisTrash);
        }

    }
    public void OrganicTrashGenerator(int ranTrash, float rndPos)
    {
        if (oTrashCount < oMaxTrashCount)
        {
            GameObject thisTrash = PhotonNetwork.Instantiate(organicTrash[ranTrash].name,spawnPoint.transform.localPosition, spawnPoint.transform.localRotation, 0);
            photonView.RPC("SettingOTrash", PhotonTargets.AllBufferedViaServer, thisTrash.name, ranTrash, rndPos);
            AddOTrash(thisTrash);
        }

    }
    public void RecycleTrashGenerator(int ranTrash, float rndPos)
    {
        if (rTrashCount < rMaxTrashCount)
        {
            GameObject thisTrash = PhotonNetwork.Instantiate(recycleTrash[ranTrash].name, spawnPoint.transform.localPosition, spawnPoint.transform.localRotation, 0);
            photonView.RPC("SettingRTrash", PhotonTargets.AllBufferedViaServer, thisTrash.name, ranTrash, rndPos);
            AddRTrash(thisTrash);
        }

    }
    public void AddGTrash(GameObject thisTrash)
    {
        gTrashListing.Add(thisTrash);
        gTrashCount = gTrashListing.Count;
    }
    public void AddHTrash(GameObject thisTrash)
    {
        hTrashListing.Add(thisTrash);
        hTrashCount = hTrashListing.Count;
    }
    public void AddOTrash(GameObject thisTrash)
    {
        oTrashListing.Add(thisTrash);
        oTrashCount = oTrashListing.Count;
    }
    public void AddRTrash(GameObject thisTrash)
    {
        rTrashListing.Add(thisTrash);
        rTrashCount = rTrashListing.Count;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(curTime);
            stream.SendNext(timeCountDown);
        }
        else
        {
            curTime = (float)stream.ReceiveNext();
            timeCountDown = (float)stream.ReceiveNext();
        }
    }
}
