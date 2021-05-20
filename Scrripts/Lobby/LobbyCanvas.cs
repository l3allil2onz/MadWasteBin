using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyCanvas : MonoBehaviour
{
    [SerializeField]
    private RoomLayoutGroup _roomLayoutGroup;
    private RoomLayoutGroup RoomLayoutGroup
    {
        get { return _roomLayoutGroup; }
    }

	public void OnClickJoinRoom(string roomName)
    {
        RoomLayoutGroup _roomLayout;
        bool isAvailable = false;

        _roomLayout = GameObject.Find("RoomLayout").GetComponent<RoomLayoutGroup>();
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        GameObject roomNameTextObj;

        if (PhotonNetwork.JoinRoom(roomName))
        {
            foreach (RoomInfo room in rooms)
            {
                isAvailable = _roomLayout.CheckJoiningRoomWithButton(room, roomName);
                roomNameTextObj = _roomLayout.CheckRoomNameTextObjWithButton(room, roomName);
                ChangeRoomName(roomName, roomNameTextObj);
                if (isAvailable)
                    break;
            }
        }
        else
        {
            print("Join room failed.");
        }

        if (isAvailable)
        {
            PhotonNetwork.JoinRoom(roomName);
            GameObject joinRoom = GameObject.Find("sectionView2 - connected menu");
            Animator joinRoomAnim = joinRoom.transform.GetComponent<Animator>();
            joinRoomAnim.SetBool("JoinRoom", true);
            ChangeRoomName(roomName);
            print("Join (" + roomName + ") room");
        }
        else
        {
            LobbyUIManager lobbyUI = GameObject.Find("lobbyUIManager").GetComponent<LobbyUIManager>();
            lobbyUI.OpenJoiningRoomFailedPanelInLobby();
            print("Room id is dose not exist");
        }
    }
    private void ChangeRoomName(string roomName)
    {
        GameObject curRoomName = GameObject.Find("CurRoomNameText");
        TextMeshProUGUI roomNameText = curRoomName.GetComponent<TextMeshProUGUI>();
        roomNameText.text = roomName;
    }
    private void ChangeRoomName(string roomName,GameObject roomNameTextObj)
    {
        TextMeshProUGUI roomNameText = roomNameTextObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        roomNameText.text = roomName;
    }
    public void ShowCurrentRoom()
    {
        GameObject curRoom = GameObject.Find("CurrentRoom");
        curRoom.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }
}
