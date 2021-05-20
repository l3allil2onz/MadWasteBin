using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : MonoBehaviour
{
    public SFXcreator sfxCreator;
    public AudioClip addPoint,minusPoint,hit;

    public GameObject[] addingScoreText = new GameObject[3];
    public GameObject hitEffectPrefabs;
    private Vector3 selfPos;
    public float timeDestroy;
    public float curTimeToDestroy;

    public int plusScore;
    public int minusScore;
    public bool waitForDestroy = false;

    photonHandler phohand;
    public PhotonPlayer photonPlayer;
    public PhotonView photonView;

    GameManager gm;
    private void Awake()
    {
        phohand = photonHandler.instance;
    }
    // Use this for initialization
    void Start ()
    {
        sfxCreator = GameObject.Find("SFXcreator").GetComponent<SFXcreator>();
        curTimeToDestroy = timeDestroy;
        gm = GameManager.instance;
        photonView = GetComponent<PhotonView>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (curTimeToDestroy < 0 && photonPlayer != null)
        {
            waitForDestroy = true;
        }
        else if(curTimeToDestroy > 0 && photonPlayer != null)
        {
            curTimeToDestroy -= Time.deltaTime;
        }

        if (waitForDestroy && photonPlayer != null)
        {
            print("waitForDestroy");
            DestroyGameObject();
            waitForDestroy = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
 
        print(photonPlayer);
        if (collision.transform.tag == "Player")
        {
            if (photonPlayer != null && collision.transform.GetComponent<PhotonView>() != null)
            {
                if (collision.GetComponent<PhotonView>().owner.NickName != photonPlayer.NickName)
                {
                    print("AddScoreAndWaitForDestroy");
                    AddScoreAndWaitForDestroy(collision);
                }
            }
        }

    }
    private void HitTarget(Collider2D collision)
    {
        sfxCreator.Create(hit, 0.8f, 1f);
        GameObject thisEffect = Instantiate(hitEffectPrefabs, collision.transform.localPosition, hitEffectPrefabs.transform.rotation);
        GameObject player = GameObject.Find(collision.transform.name);
        thisEffect.transform.parent = player.transform;
        thisEffect.transform.localPosition = new Vector3(0, 0, -0.1f);
        thisEffect.transform.localScale = new Vector3(7f, 7f, 7f);
    }
    private void CheckMatchingPhotonPlayers(int index)
    {
        if (photonView.isMine && phohand.photonPlayer.IsLocal)
        {
            AddScoreUIController addScoreUI;
            GameObject playerObj;
            PlMove playerPl;
            PhotonPlayer[] players = PhotonNetwork.playerList;
            if (players.Length > 1)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (GameObject.Find("Character01" + "(Clone)") != null)
                    {
                        if (players[i] == phohand.photonPlayer &&
                            GameObject.Find("Character01" + "(Clone)").GetComponent<PhotonView>().owner.NickName == phohand.photonPlayer.NickName)
                        {
                            playerObj = GameObject.Find("Character01" + "(Clone)");
                            playerPl = playerObj.GetComponent<PlMove>();
                            GameObject thisScoreText = Instantiate(addingScoreText[index],
                                                                                 playerPl.addingScorePanel.transform.localPosition,
                                                                                 addingScoreText[index].transform.rotation);
                            addScoreUI = thisScoreText.GetComponent<AddScoreUIController>();
                            addScoreUI.player = playerPl;
                            thisScoreText.transform.SetParent(playerPl.addingScorePanel.transform);
                            thisScoreText.transform.localPosition = Vector3.zero;
                            thisScoreText.transform.localScale = addingScoreText[index].transform.localScale;
                            break;
                        }
                    }

                    if (GameObject.Find("Character02" + "(Clone)") != null)
                    {
                        if (players[i] == phohand.photonPlayer &&
                            GameObject.Find("Character02" + "(Clone)").GetComponent<PhotonView>().owner.NickName == phohand.photonPlayer.NickName)
                        {
                            playerObj = GameObject.Find("Character02" + "(Clone)");
                            playerPl = playerObj.GetComponent<PlMove>();
                            GameObject thisScoreText = Instantiate(addingScoreText[index],
                                                                                 playerPl.addingScorePanel.transform.localPosition,
                                                                                 addingScoreText[index].transform.rotation);
                            thisScoreText.transform.SetParent(playerPl.addingScorePanel.transform);
                            thisScoreText.transform.localPosition = Vector3.zero;
                            thisScoreText.transform.localScale = addingScoreText[index].transform.localScale;
                            break;
                        }
                    }

                    if (GameObject.Find("Character03" + "(Clone)") != null)
                    {
                        if (players[i] == phohand.photonPlayer &&
                            GameObject.Find("Character03" + "(Clone)").GetComponent<PhotonView>().owner.NickName == phohand.photonPlayer.NickName)
                        {
                            playerObj = GameObject.Find("Character03" + "(Clone)");
                            playerPl = playerObj.GetComponent<PlMove>();
                            GameObject thisScoreText = Instantiate(addingScoreText[index],
                                                                                 playerPl.addingScorePanel.transform.localPosition,
                                                                                 addingScoreText[index].transform.rotation);
                            thisScoreText.transform.SetParent(playerPl.addingScorePanel.transform);
                            thisScoreText.transform.localPosition = Vector3.zero;
                            thisScoreText.transform.localScale = addingScoreText[index].transform.localScale;
                            break;
                        }
                    }

                    if (GameObject.Find("Character04" + "(Clone)") != null)
                    {
                        if (players[i] == phohand.photonPlayer &&
                            GameObject.Find("Character04" + "(Clone)").GetComponent<PhotonView>().owner.NickName == phohand.photonPlayer.NickName)
                        {
                            playerObj = GameObject.Find("Character04" + "(Clone)");
                            playerPl = playerObj.GetComponent<PlMove>();
                            GameObject thisScoreText = Instantiate(addingScoreText[index],
                                                                                 playerPl.addingScorePanel.transform.localPosition,
                                                                                 addingScoreText[index].transform.rotation);
                            thisScoreText.transform.SetParent(playerPl.addingScorePanel.transform);
                            thisScoreText.transform.localPosition = Vector3.zero;
                            thisScoreText.transform.localScale = addingScoreText[index].transform.localScale;
                            break;
                        }
                    }

                }
            }
        }
    }
    private void AddScoreAndWaitForDestroy(Collider2D collision)
    {
        print(collision.GetComponent<PhotonView>() != null);
        int curScore = new int();
        curScore = photonPlayer.GetScore();
        HitTarget(collision);

        if (collision.transform.gameObject.name == "Character01(Clone)")
        {
            if (transform.name == "GT0(Clone)" || transform.name == "GT1(Clone)")
            {
                if (collision.GetComponent<PhotonView>().owner.NickName != photonPlayer.NickName)
                {
                    print("plusScore += " + plusScore);
                    if (photonView.isMine)
                    {
                        photonPlayer.AddScore(plusScore);
                        sfxCreator.Create(addPoint, 0.658f, 1f);
                    }
                    CheckMatchingPhotonPlayers(1);
                    waitForDestroy = true;
                }
            }
            else
            {
                if (collision.GetComponent<PhotonView>().owner.NickName != photonPlayer.NickName)
                {
                    if (curScore > 0)
                    {
                        print("minusScore -= " + minusScore);
                        if (photonView.isMine)
                        {
                            photonPlayer.AddScore(minusScore);
                            if (curScore < 0)
                            {
                                print("(Score < 0) Score += " + curScore);
                                photonPlayer.AddScore(-(curScore));

                            }
                        }
                        sfxCreator.Create(minusPoint, 0.658f, 1f);
                        CheckMatchingPhotonPlayers(2);
                        waitForDestroy = true;
                    }
                    else
                    {
                        sfxCreator.Create(minusPoint, 0.658f, 1f);
                        CheckMatchingPhotonPlayers(2);
                        waitForDestroy = true;
                    }
                }
            }
        }
        else if (collision.transform.gameObject.name == "Character02(Clone)")
        {
            if (transform.name == "HT0(Clone)" || transform.name == "HT1(Clone)")
            {
                if (collision.GetComponent<PhotonView>().owner.NickName != photonPlayer.NickName)
                {
                    print("plusScore += " + plusScore);
                    if (photonView.isMine)
                    {
                        photonPlayer.AddScore(plusScore);
                        sfxCreator.Create(addPoint, 0.658f, 1f);
                    }
                    CheckMatchingPhotonPlayers(1);
                    waitForDestroy = true;
                }
            }
            else
            {
                if (collision.GetComponent<PhotonView>().owner.NickName != photonPlayer.NickName)
                {
                    if (curScore > 0)
                    {
                        print("minusScore -= " + minusScore);
                        if (photonView.isMine)
                        {
                            photonPlayer.AddScore(minusScore);
                            if (curScore < 0)
                            {
                                print("(Score < 0) Score += " + curScore);
                                photonPlayer.AddScore(-(curScore));
                            }
                        }
                        sfxCreator.Create(minusPoint, 0.658f, 1f);
                        CheckMatchingPhotonPlayers(2);
                        waitForDestroy = true;
                    }
                    else
                    {
                        sfxCreator.Create(minusPoint, 0.658f, 1f);
                        CheckMatchingPhotonPlayers(2);
                        waitForDestroy = true;
                    }
                }
            }

        }
        else if (collision.transform.gameObject.name == "Character03(Clone)")
        {
            if (transform.name == "OT0(Clone)" || transform.name == "OT1(Clone)")
            {
                if (collision.GetComponent<PhotonView>().owner.NickName != photonPlayer.NickName)
                {
                    print("plusScore += " + plusScore);
                    if (photonView.isMine)
                    {
                        photonPlayer.AddScore(plusScore);
                        sfxCreator.Create(addPoint, 0.658f, 1f);
                    }
                    CheckMatchingPhotonPlayers(1);
                    waitForDestroy = true;
                }

            }
            else
            {
                if (collision.GetComponent<PhotonView>().owner.NickName != photonPlayer.NickName)
                {
                    if (curScore > 0)
                    {
                        print("minusScore -= " + minusScore);
                        if (photonView.isMine)
                        {
                            photonPlayer.AddScore(minusScore);
                            if (curScore < 0)
                            {
                                print("(Score < 0) Score += " + curScore);
                                photonPlayer.AddScore(-(curScore));
                            }
                        }
                        sfxCreator.Create(minusPoint, 0.658f, 1f);
                        CheckMatchingPhotonPlayers(2);
                        waitForDestroy = true;
                    }
                    else
                    {
                        sfxCreator.Create(minusPoint, 0.658f, 1f);
                        CheckMatchingPhotonPlayers(2);
                        waitForDestroy = true;
                    }
                }
            }
        }
        else if (collision.transform.gameObject.name == "Character04(Clone)")
        {
            if (transform.name == "RT0(Clone)" || transform.name == "RT1(Clone)")
            {
                if (collision.GetComponent<PhotonView>().owner.NickName != photonPlayer.NickName)
                {
                    print("plusScore += " + plusScore);
                    if (photonView.isMine)
                    {
                        photonPlayer.AddScore(plusScore);
                        sfxCreator.Create(addPoint, 0.658f, 1f);
                    }
                    CheckMatchingPhotonPlayers(1);
                    waitForDestroy = true;
                }
            }
            else
            {
                if (collision.GetComponent<PhotonView>().owner.NickName != photonPlayer.NickName)
                {
                    if (curScore > 0)
                    {
                        print("minusScore -= " + minusScore);
                        if (photonView.isMine)
                        {
                            photonPlayer.AddScore(minusScore);
                            if (curScore < 0)
                            {
                                print("(Score < 0) Score += " + curScore);
                                photonPlayer.AddScore(-(curScore));
                            }
                        }
                        sfxCreator.Create(minusPoint, 0.658f, 1f);
                        CheckMatchingPhotonPlayers(2);
                        waitForDestroy = true;
                    }
                    else
                    {
                        sfxCreator.Create(minusPoint, 0.658f, 1f);
                        CheckMatchingPhotonPlayers(2);
                        waitForDestroy = true;
                    }
                }
            }
        }
        else
        {
            print("Character name have change.");
        }
    }
    private PhotonPlayer CheckMasterClientPhotonPlayers()
    {
        PhotonPlayer[] players = PhotonNetwork.playerList;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].IsMasterClient)
            {
                return players[i];
            }
        }
        print("CheckMasterClientPhotonPlayers Error.");
        return photonPlayer;
    }

    private void DestroyGameObject()
    {
        if (photonView.isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(curTimeToDestroy);
        }
        else
        {
            selfPos = (Vector3)stream.ReceiveNext();
            curTimeToDestroy = (float)stream.ReceiveNext();
        }
    }
}
