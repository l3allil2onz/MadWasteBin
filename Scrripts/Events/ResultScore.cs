using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ResultScore : MonoBehaviour
{
    public Image characterWinnerImg;
    public int characterIndex = 0;
    public Sprite[] characterSprite;
    public TextMeshProUGUI nameCharacterWinner;
    public static ResultScore Instance;
    PhotonView photonView;
    GameManager gm = GameManager.instance;

    private void Awake()
    {
        Instance = this;
    }

	void Update ()
    {
        if (gm != null)
        {
            if (gm.curTime > 0)
            {
                //characterWinnerImg.sprite = characterSprite[characterIndex];
            }
        }
    }

    public void OnClick_LeaveMatch()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(characterWinnerImg.sprite);
            stream.SendNext(nameCharacterWinner.text);
            //stream.SendNext(characterIndex);
        }
        else
        {
            characterWinnerImg.sprite = (Sprite)stream.ReceiveNext();
            nameCharacterWinner.text = (string)stream.ReceiveNext();
            //characterIndex = (int)stream.ReceiveNext();
        }
    }
}
