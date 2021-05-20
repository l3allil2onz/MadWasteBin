
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomListing : MonoBehaviour,IPointerClickHandler
{

    [SerializeField]
    private TextMeshProUGUI _roomNameText;
    private TextMeshProUGUI RoomNameText
    {
        get { return _roomNameText; }
    }

    LobbyCanvas lobbyCanvas;
    GameObject blockRaycastRoom;
    public string RoomName { get; private set; }
    public bool Updated { get; set; }

    private void Start()
    {
        blockRaycastRoom = GameObject.Find("BlockRaycastDoubleClickRoom");
        blockRaycastRoom.SetActive(false);
        GameObject lobbyCanvasObj = MainCanvasManager.Instance.LobbyCanvas.gameObject;
        if (lobbyCanvasObj == null)
            return;
        lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }

    public void SetRoomNameText(string Text)
    {
        RoomName = Text;
        RoomNameText.text = RoomName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;

        if(clickCount == 2)
        {
            OnDoubleClick();
        }
    }

    void OnDoubleClick()
    {
        lobbyCanvas.OnClickJoinRoom(RoomNameText.text);
        blockRaycastRoom.SetActive(true);
    }
}
