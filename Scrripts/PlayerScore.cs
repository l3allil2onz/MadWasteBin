using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    public int rank;
    public PhotonView photonView;
    public int characterPlayerIndex;
    public PhotonPlayer photonPlayer;
    public List<PhotonPlayer> players = new List<PhotonPlayer>();
    //public List<GameObject> scoreBoard = new List<GameObject>();
    public string playerName;
    public string tagCharacterPlayer;
    public int scorePlayer;
    public TextMeshProUGUI textMP_UGUI;
    photonHandler phoHand = photonHandler.instance;
    GameManager gm;

    private void Awake()
    {
        gm = GameManager.instance;
        textMP_UGUI = GetComponent<TextMeshProUGUI>();
        photonView = GetComponent<PhotonView>();

    }

    void Start ()
    {
        AddPhotonPlayerIntoPlayersList();
        SetPhotonPlayer();
        transform.SetParent(gm.playerScoreList.transform);
        transform.localScale = Vector3.one;
        if (photonView.isMine)
        {
            playerName = PhotonNetwork.playerName;
        }

        if (!photonView.isMine)
        {
            playerName = photonView.owner.name;
        }
        tagCharacterPlayer = SetTagCharacterPlayer(characterPlayerIndex);
    }

    void Update()
    {
        if (photonView.isMine)
        {
            scorePlayer = photonPlayer.GetScore();
            rank = Ranking();
            textMP_UGUI.text = "___________________" + "\n" +
                               "[" + rank + "] " + playerName + "(" + tagCharacterPlayer + ")" + " :" + scorePlayer;
            if (rank == 1)
            {
                if (playerName != null && ResultScore.Instance != null)
                {
                    ResultScore.Instance.nameCharacterWinner.text = playerName;
                    ResultScore.Instance.characterWinnerImg.sprite = ResultScore.Instance.characterSprite[characterPlayerIndex];
                }

            }
        }

    }
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

    private int CheckCountOfIsScoreLowest(int count)
    {
        switch (count)
        {
            case 0:
                break;
            case 1:
                return 2;
            case 2:
                return 3;
            case 3:
                return 4;
            default: break;
        }
        return -1;
    }
    private int CheckCountOfIsScoreHighest(int count)
    {
        PhotonPlayer[] players = PhotonNetwork.playerList;
        switch (count)
        {
            case 0:
                break;
            case 1:
                if (players.Length == 2)
                {
                    return 1;
                }
                else if (players.Length == 3)
                {
                    return 2;
                }
                else if (players.Length == 4)
                {
                    return 3;
                }
                break;
            case 2:
                if(players.Length == 3)
                {
                    return 1;
                }
                else if (players.Length == 4)
                {
                    return 2;
                }
                break;
            case 3:
                if (players.Length == 4)
                {
                    return 1;
                }
                break;
            default: break;
        }
        return -1;
    }
    private int Ranking()
    {
        int rank = 0;
        int isScoreHighest = 0;
        int isScoreLowest = 0;
        int index = this.players.FindIndex(x => x.NickName == phoHand.photonPlayer.NickName);
        for (int i = 0; i < players.Count; i++)
        {
            if(players.Count > 1)
            {
                if (players[i] != players[index])
                {
                    if (index == 0)
                    {
                        if (photonPlayer.GetScore() > players[i].GetScore())
                        {
                            isScoreHighest++;
                        }
                        else if (photonPlayer.GetScore() < players[i].GetScore())
                        {
                            isScoreLowest++;
                        }
                    }
                    else if (index == 1)
                    {
                        if (photonPlayer.GetScore() > players[i].GetScore())
                        {
                            isScoreHighest++;
                        }
                        else if (photonPlayer.GetScore() < players[i].GetScore())
                        {
                            isScoreLowest++;
                        }
                    }
                    else if (index == 2)
                    {
                        if (photonPlayer.GetScore() > players[i].GetScore())
                        {
                            isScoreHighest++;
                        }
                        else if (photonPlayer.GetScore() < players[i].GetScore())
                        {
                            isScoreLowest++;
                        }
                    }
                    else if (index == 3)
                    {
                        if (photonPlayer.GetScore() > players[i].GetScore())
                        {
                            isScoreHighest++;
                        }
                        else if (photonPlayer.GetScore() < players[i].GetScore())
                        {
                            isScoreLowest++;
                        }
                    }
                }
                if(i < players.Count)
                {
                    if(isScoreHighest > 0)
                    {
                        rank = CheckCountOfIsScoreHighest(isScoreHighest);
                        return rank;
                    }
                    else if(isScoreLowest > 0)
                    {
                        rank = CheckCountOfIsScoreLowest(isScoreLowest);
                        return rank;
                    }
                }
            }
            else if (players.Count <= 1)
            {
                return 1;
            }
        }
        return players.Count;
    }
    private void AddPhotonPlayerIntoPlayersList()
    {
        PhotonPlayer[] players = PhotonNetwork.playerList;
        for (int i = 0; i < players.Length; i++)
        {
            this.players.Add(players[i]);
        }
    }
    private void SetPhotonPlayer()
    {
        int index = this.players.FindIndex(x => x.NickName == phoHand.photonPlayer.NickName);
        photonPlayer = players[index];
    }
    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(photonPlayer);
            stream.SendNext(textMP_UGUI.text);
        }
        else
        {
            photonPlayer = (PhotonPlayer)stream.ReceiveNext();
            textMP_UGUI.text = (string)stream.ReceiveNext();
        }
    }


}
