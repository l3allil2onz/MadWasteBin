using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    public float speed;
    public float timeToDestroy = 10f;
    public float curTimeToDestroy;

    private void Start()
    {
        curTimeToDestroy = timeToDestroy;
    }
    void Update ()
    {
        transform.localPosition -= new Vector3(speed, 0, 0);
        DestroyCloud();

    }
    private void DestroyCloud()
    {
        if (curTimeToDestroy <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            curTimeToDestroy -= Time.deltaTime;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
