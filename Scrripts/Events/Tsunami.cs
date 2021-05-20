using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tsunami : MonoBehaviour
{
    public GameObject respawn;
    public BoxCollider2D collider;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            collision.transform.localPosition = new Vector3(respawn.transform.localPosition.x, respawn.transform.localPosition.y,-180f);
        }
    }
    public void DeactiveTsunamiEvent()
    {
        gameObject.SetActive(false);
        TsunamiEvent.Instance.isTsunamiActive = false;
    }
    public void ColliderEnabledTrue()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.enabled = true;
    }
    public void ColliderEnabledFalse()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
    }
}
