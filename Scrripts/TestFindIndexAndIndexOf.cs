using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestFindIndexAndIndexOf : MonoBehaviour
{
    public List<CurrentRoomCanvas> characterListings = new List<CurrentRoomCanvas>();
    public Text[] text;
    int characterIndex = 1;

    void Start ()
    {
        for (int i = 0; i < 4; i++)
        {
            characterListings.Add(CurrentRoomCanvas.instance);
        }
        text[0].text = characterListings.Count.ToString();
        text[1].text = characterListings.FindIndex(x => x.characterIndex == characterIndex).ToString();
        text[2].text = characterListings.IndexOf(characterListings[1]).ToString();
    }
	
	void Update ()
    {
		
	}
}
