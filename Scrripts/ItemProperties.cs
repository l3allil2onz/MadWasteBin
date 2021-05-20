using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemProperties : MonoBehaviour 
{

    private Vector3 selfPos;
    public float timeDestroy;
    public float curTimeToDestroy;

    public PhotonView photonView;

    GameManager gm;

    void Start ()
    {
        curTimeToDestroy = timeDestroy;
        gm = GameManager.instance;
        photonView = GetComponent<PhotonView>();
        //playerScore = 
    }

    void Update()
    {
        if (PhotonNetwork.isMasterClient && photonView.isMine)
        {
            if (curTimeToDestroy <= 0)
            {
                CheckTrashForRemove();
            }
            else if (curTimeToDestroy > 0)
            {
                curTimeToDestroy -= Time.deltaTime;
            }
        }
	}
    [PunRPC]
    public void CheckTrashForRemove()
    {
        for (int i = 0; i < 2; i++)
        {
            if (transform.name == gm.generalTrash[i].name + "(Clone)")
            {
                if (!PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RemoveGTrash", PhotonTargets.MasterClient);
                    photonView.RPC("DestroyGameObject", PhotonTargets.MasterClient);
                }
                else if (PhotonNetwork.isMasterClient)
                {
                    RemoveGTrash();
                    DestroyGameObject();
                }
            }
            else if (transform.name == gm.harzadousTrash[i].name + "(Clone)")
            {
                if (!PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RemoveHTrash", PhotonTargets.MasterClient);
                    photonView.RPC("DestroyGameObject", PhotonTargets.MasterClient);
                }
                else if (PhotonNetwork.isMasterClient)
                {
                    RemoveHTrash();
                    DestroyGameObject();
                }
            }
            else if (transform.name == gm.organicTrash[i].name + "(Clone)")
            {
                if (!PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RemoveOTrash", PhotonTargets.MasterClient);
                    photonView.RPC("DestroyGameObject", PhotonTargets.MasterClient);
                }
                else if (PhotonNetwork.isMasterClient)
                {
                    RemoveOTrash();
                    DestroyGameObject();
                }
            }
            else if (transform.name == gm.recycleTrash[i].name + "(Clone)")
            {
                if (!PhotonNetwork.isMasterClient)
                {
                    photonView.RPC("RemoveRTrash", PhotonTargets.MasterClient);
                    photonView.RPC("DestroyGameObject", PhotonTargets.MasterClient);
                }
                else if (PhotonNetwork.isMasterClient)
                {
                    RemoveRTrash();
                    DestroyGameObject();
                }
            }
        }
    }
    [PunRPC]
    private void RemoveGTrash()
    {
        if (photonView.isMine)
        {
            gm.gTrashCount--;
            gm.gTrashListing.Remove(this.gameObject);
        }
    }
    [PunRPC]
    private void RemoveHTrash()
    {
        if (photonView.isMine)
        {
            gm.hTrashCount--;
            gm.hTrashListing.Remove(this.gameObject);
        }
    }
    [PunRPC]
    private void RemoveOTrash()
    {
        if (photonView.isMine)
        {
            gm.oTrashCount--;
            gm.oTrashListing.Remove(this.gameObject);
        }
    }
    [PunRPC]
    private void RemoveRTrash()
    {
        if (photonView.isMine)
        {
            gm.rTrashCount--;
            gm.rTrashListing.Remove(this.gameObject);
        }

    }
    [PunRPC]
    private void DestroyGameObject()
    {
        if (photonView.isMine && PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(curTimeToDestroy);
        }
        else
        {
            selfPos = (Vector3)stream.ReceiveNext();
            curTimeToDestroy = (float)stream.ReceiveNext();
        }
    }
}
