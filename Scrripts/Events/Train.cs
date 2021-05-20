using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public GameObject respawn;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            collision.transform.localPosition = new Vector3(respawn.transform.localPosition.x, respawn.transform.localPosition.y,-180f);
        }
    }
}
