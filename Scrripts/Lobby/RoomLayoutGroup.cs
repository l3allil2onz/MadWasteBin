using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomLayoutGroup : MonoBehaviour
{
    [SerializeField]
    private GameObject _roomListingPrefab;
    private GameObject RoomListingPrefab
    {
        get { return _roomListingPrefab; }
    }

    //  Listเก็บจำนวนของห้อง ข้อดีง่ายต่อการตรวจสอบ บังคับมีindex ใช้indexระบุลำดับห้องสำหรับการแสดงผลและตรวจสอบ
    [SerializeField]
    private List<RoomListing> _roomListingButtons = new List<RoomListing>();
    private List<RoomListing> RoomListingButtons
    {
        get { return _roomListingButtons; }
    }

    //  แสดงผลจำนวนห้องทำงานอัตโนมัติ เมื่อเข้าสู่ล๊อบบี้ หรือPhotonNetwork มีการอัพเดต
    private void OnReceivedRoomListUpdate()
    {
        //  นำจำนวนห้อง รายละเอียดห้อง ที่ถูกสร้างขึ้นไปเก็บไว้ที่ RoomInfo เพื่อใช้ในการแสดงผลรายละเอียดห้อง หรือจำนวนผู้เล่นของห้องนั้นๆ ในภายหลัง
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        print("Room Length : "+rooms.Length);
        //  การแสดงผลจำนวนห้องที่ถูกสร้างในปัจจุบัน โดยส่งจำนวนห้องที่ถูกสร้าง (RoomInfo) ทั้งหมดด้วย (foreach loop)ไปให้แก่ RoomReceived ทางparameter
        foreach (RoomInfo room in rooms)
        {
            print("Room : "+room.Name);
            RoomReceived(room);
        }
        
        RemoveOldRooms();
    }

    public bool CheckJoiningRoomWithButton(RoomInfo room,string roomId)
    {
        int index = RoomListingButtons.FindIndex(x => x.RoomName == roomId);
        if (room.PlayerCount < room.MaxPlayers)
        {
            if (index == -1)
                return false;
            else
                return true;
        }
        else
            return false;
    }
    public GameObject CheckRoomNameTextObjWithButton(RoomInfo room, string roomId)
    {
        int index = RoomListingButtons.FindIndex(x => x.RoomName == roomId);
        GameObject roomObj = RoomListingButtons[index].gameObject;

        if (room.PlayerCount < room.MaxPlayers)
        {
            if (index == -1)
                return null;
            else
                return roomObj;
        }
        else
            return null;
    }

    private void RoomReceived(RoomInfo room)
    {
        //  ตรวจสอบindex ของชื่อห้องที่สร้าง กับ ชื่อของห้องในListตัวปัจจุบันที่รับค่ามาจากPhoton ที่ตรงกัน แล้วนำมาเก็บไว้ยังตัวแปร index ที่สร้างขึ้น(ค้นหาห้องในlist)
        int index = RoomListingButtons.FindIndex(x => x.RoomName == room.Name);
        //  หากindex ที่ได้มีค่าเป็น -1 หมายความว่า ห้องนั้นอาจสร้างขึ้นมาแล้ว แต่ยังไม่มีตัวแสดงผลห้องบนหน้าListingRoomLayout  จึงต้องทำการดึงprefabที่เราได้สร้างไว้มาเป็นตัวแทนหรือ
        //  ส่วนแสดงผลที่ทำให้ตามนุษย์มองเห็นและ เข้าใจตรงกันว่ามีห้องนั้นถูกสร้างขึ้น 
        //  (ส่วนAdd)
        if (index == -1)
        {
            //  หากห้องสามารถมองเห็นได้ และ ผู้เล่นปัจจุบันในห้องนั้นๆ ไม่เกินจำนวนผู้เล่นสูงสุดต่อห้อง ให้เข้าไปทำงานคำสั่งด้านล่าง
            if(room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                //  ทำการสร้าง GameObj ที่เป็นRoomListingPrefab เผื่อไว้ใช้สำหรับการSetParent หรือปรับค่าอื่นๆเท่าที่GameObjectสามารถทำได้
                GameObject roomListingObj = Instantiate(RoomListingPrefab);
                roomListingObj.transform.SetParent(transform, false);
                //  ทำการสร้าง RoomListing แล้วเข้าถึงComponent RoomListing เพื่อใช้ในการเพิ่ม เข้าList สำหรับนำListตัวนั้นๆมาดำเนินการต่อไป
                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                RoomListingButtons.Add(roomListing);
                //  index คือตัวบ่งชี้ .count คือตัวบ่งบอกจำนวนที่มี ดังนั้นจำนวนที่บ่งบอกว่ามีอยู่จึงถูกกำหนดไว้ที่ 1 ตัวบ่งชี้จึงเริ่มที่ค่า 1 โดยมีindex 0 บ่งบอกถึงตัวเริ่มต้น
                index = (RoomListingButtons.Count - 1);
                print("RoomReceived[" + index + "] : " + "Add");
            }
        }
        //  หากindex ที่ได้มีค่าที่ไม่ใช่ -1 หมายความว่าเป็นได้ทั้ง มีค่ามากกว่า หรือน้อยกว่า แต่ในกรณีที่ไม่มีค่าที่ตรงกันในunityจะถูกมองเป็น -1 เสมอ 
        //  ดังนั้น index ในกรณีนี้จึงต้องเป็นค่าที่มากกว่า -1 ซึ่งมีความหมายว่า อาจมีห้องนั้นอยู่แล้ว และยังคงอยู่จนกระทั่งเงื่อนไขนี้ตรวจสอบมันเจอ ก็ให้มันเข้าไปทำงานคำสั่งด้านล่างต่อไป 
        //  (ส่วนUpdated)
        if(index != -1)
        {
            //  ทำการสร้าง RoomListing โดยให้ตัวมันมีค่าเท่ากับ index 
            //  ดังนั้นจึงกำหนดให้ชื่อของห้องที่สร้างขึ้นแล้ว ตรงกับชื่อที่photonตรวจสอบ เนื่องจากในอนาคตอาจมีการเปลี่ยนแปลงชื่อทำให้ชื่อห้องที่สร้างไม่ตรงกับชื่อห้องที่photonตรวจพบ 
            //  จึงให้ระบบอัพเดต การเปลี่ยนชื่อ หรือ จำนวนผู้เล่นตลอดเวลา หากผู้เล่นภายในห้อง หรือชื่อของห้องมีการเปลี่ยนแปลงในภายหลัง 
            RoomListing roomListing = RoomListingButtons[index];
            roomListing.SetRoomNameText(room.Name);
            GameObject roomListingObj = roomListing.gameObject;
            roomListingObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Players in Room : " + room.PlayerCount + " / " + room.MaxPlayers;
            
            for(int i = 0; i < room.PlayerCount; i++)
            {
                roomListingObj.transform.GetChild(4).transform.GetChild(i).GetComponent<Image>().color = Color.white;
                if(room.PlayerCount < room.MaxPlayers)
                {
                    for (int j = room.PlayerCount; j < room.MaxPlayers; j++)
                    {
                        roomListingObj.transform.GetChild(4).transform.GetChild(j).GetComponent<Image>().color = Color.black;
                    }
                }
            }
            roomListing.Updated = true;
            print("RoomReceived[" + index + "] : " + "Updated");
        }
    }

    //  ลบห้องเก่าที่ถูกสร้างไว้
    private void RemoveOldRooms()
    {
        //  สร้างListเก็บห้องที่ต้องลบ
        List<RoomListing> removeRooms = new List<RoomListing>();
        //  ใช้foreach loop กับ List เพื่อนำห้องที่ต้องการลบทุกๆห้อง มาเก็บไว้ที่ List
        foreach(RoomListing roomlisting in RoomListingButtons)
        //  RoomListing ทุกคัวที่อยู่ใน RoomListingButtons
        {
            //  หากห้องนั้นไม่มีการอัพเดต ซึ่งหมายความว่าห้องนั้นอาจไม่มีอยู่แล้ว แต่Prefabของมันยังคงแสดงว่ามีอยู่ในหน้าHierachy ให้ทำการเพิ่มเข้าไปใน List ที่ต้องการลบ
            //  แต่หากไม่ ห้องนั้นมีการอัพเดตอยู่ ซึ่งหมายความว่าห้องยังคงอยู่ ก็ปรับเป็น false แทนและมันจะเป็น true อีกครั้งเสมอก่อนเข้ามาตรวจสอบส่วนนี้ เป็นการวนลูปไปเรื่อยๆ เมื่อตรวจสอบครั้งต่อไปแล้วห้องยังคงอยู่
            if (!roomlisting.Updated)
                removeRooms.Add(roomlisting);
            else
                roomlisting.Updated = false;
        }
        //  นำห้องที่ต้องลบทั้งหมด มาทำการลบทิ้ง
        foreach(RoomListing roomlisting in removeRooms)
        //  RoomListing ทุกคัวที่อยู่ใน removeRooms
        {
            GameObject roomlistingObj = roomlisting.gameObject;
            RoomListingButtons.Remove(roomlisting);
            Destroy(roomlistingObj);
        }
    }
}
