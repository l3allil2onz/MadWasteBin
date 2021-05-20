using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddScoreUIController : MonoBehaviour
{
    public PlMove player;
    Vector3 scale;
    Vector3 selfPos;
    [Range(0f,0.75f)]
    public float speedTranspos;
    public float lifeTime,curLifeTime;
    public bool isFacingRight = false;

    private void Awake()
    {
        scale = transform.localScale;
    }
    // Use this for initialization
    void Start()
    {

        curLifeTime = lifeTime;

    }

    // Update is called once per frame
    void Update()
    {
        if (curLifeTime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.localPosition += new Vector3(0, speedTranspos, 0);
            curLifeTime -= Time.deltaTime;
        }

    }

    public void DestroyItSelf()
    {
        Destroy(gameObject);
    }
}
