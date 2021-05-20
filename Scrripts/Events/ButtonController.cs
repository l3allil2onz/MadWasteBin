using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public bool pressed = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        print("OnPointerDown : " + pressed);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        print("OnPointerUp : " + pressed);
    }
}
