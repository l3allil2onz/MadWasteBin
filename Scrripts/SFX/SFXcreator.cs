using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXcreator : MonoBehaviour
{
    //รับPrefabเสียงเอฟเฟคเข้ามาไว้ในตัวแปร
    public GameObject SFXPrefab;

    //ฟังก์ชั่นการสร้างเสียง รับคลิปเสียง และ ระดับเสียง
    public void Create(AudioClip ac,float volume,float pitch)
    {
        //สร้างตัวแปรกำหนดให้เรียกSFXPrefabมาใช้
        GameObject thisSFX = Instantiate(SFXPrefab);
        //เมื่อมีการรับเสียงเข้ามายังGameObjectนั้นๆพร้อมกับการสร้างComponentมารับSFXPlayer จะถูกนำมาเก็บไว้เพื่อใช้เล่นเสียงที่รับเข้ามาทางGameObjectนั้นๆ  
        // GameComponent<SFXPlayer>().Create(GameObject ac,GameObject volume);
        thisSFX.GetComponent<SFXPlayer>().acSFXPlayer = ac;
        //GameObjectที่ใช้การเข้าถึง<SFXPlayer>().Createจะเพิ่มระดับความดังเสียงได้ "ปรับระดับเสียง"
        thisSFX.GetComponent<AudioSource>().volume = volume;
        //GameObjectที่ใช้การเข้าถึง<SFXPlayer>().Createจะเพิ่มระดับย่านเสียงได้ "ปรับย่านเสียง"
        thisSFX.GetComponent<AudioSource>().pitch = pitch;
    }
}
