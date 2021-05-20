using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    Vector3 selfRotation;
    Vector3 selfPos;
    PhotonView photonView;
    public BoxCollider2D collider;
    public GameObject respawn;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        respawn = GameObject.Find("SpawnPoint");
        collider = GetComponent<BoxCollider2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            collision.transform.localPosition = new Vector3(respawn.transform.localPosition.x, respawn.transform.localPosition.y, -180f);
        }
    }

    public void colliderEnabledTrue()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.enabled = true;
    }
    public void colliderEnabledFalse()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
    }
    public void DestroyItSelf()
    {
        if(photonView.isMine && PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(collider.enabled);
            stream.SendNext(selfRotation);
            stream.SendNext(selfPos);
        }
        else
        {
            collider.enabled = (bool)stream.ReceiveNext();
            selfRotation = (Vector3)stream.ReceiveNext();
            selfPos = (Vector3)stream.ReceiveNext();
        }
    }
}
