using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{

    public AudioClip acSFXPlayer;
    AudioSource asSFXPlayer;

	void Start ()
    {
        //สร้างComponent AudioSource
        asSFXPlayer = GetComponent<AudioSource>();
        //สร้างตัวเก็บคลิปเสียงเพื่อรับจากGameObject มาเก็บไว้ที่AudioSourceตัวเอง
        asSFXPlayer.clip = acSFXPlayer;
        //เล่นเสียงจากคลิป
        asSFXPlayer.Play();
        //ทำลายคลิปเสียง
        Destroy(gameObject, acSFXPlayer.length);
	}
}
