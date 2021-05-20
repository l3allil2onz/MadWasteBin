using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    public ContactFilter2D contactFilter;
    public Collider2D[] col;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //GetComponent<BoxCollider2D>().isTrigger = true;
        print("collision");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("trigger");
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        GetComponent<BoxCollider2D>().OverlapCollider(contactFilter, col);
        print("collisionStay");
    }
}
