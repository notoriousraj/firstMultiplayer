using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;

    [Header("info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject HatObject;

    [HideInInspector]
    public float Curhattime;

    [Header("Component")]
    public Rigidbody rigbody;
    public Player PhotonPlayer;

    public void initialize(Player player)
    {
        PhotonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;

        //give the first player the hat
        if (id == 1)
            GameManager.instance.GiveHat(id, true);

        //if this isn't our local player, diable physcisc as that 
        //contolled by the user and synced to all other client 
        if (!photonView.IsMine)
            rigbody.isKinematic = true;
    }
    void Update()
    {
        // the host will chck if the player has won
        if (PhotonNetwork.IsMasterClient)
        {
            if(Curhattime >= GameManager.instance.timetowin && !GameManager.instance.Gameended)
            {
                GameManager.instance.Gameended = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }

        if (photonView.IsMine)
        {
            Move();
            if (Input.GetKeyDown(KeyCode.Space))
                Tryjump();

            //track the amount of time we are where the hat 
            if (HatObject.activeInHierarchy)
                Curhattime += Time.deltaTime;
        }
    }

    // move the player in x and z axis
    void Move()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        rigbody.velocity = new Vector3(x, rigbody.velocity.y, z);
    }

    //check if we're grounded - if so jump
    void Tryjump()
    {
        //Creating a ray which shoots below us
        Ray ray = new Ray(transform.position, Vector3.down);

        // if we hit something then we're grounded - so jump
        if (Physics.Raycast(ray, 0.7f))
        {
            rigbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // set the players hat active or not 
    public void SetHat(bool HasHat)
    {
        HatObject.SetActive(HasHat);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
            return;

        //did we hit another player
        if(collision.gameObject.CompareTag("Player"))
        {
            //do they have the hat
            if(GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerwithhat)
            {
                //can we get the hat 
                if (GameManager.instance.CanGetHat())
                {
                    //give us the hat
                    GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
        
    }
    public void OnPhotonSerializeView(PhotonStream stream , PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext("Curhattime");
        }
        else if (stream.IsReading)
        {
            Curhattime = (float)stream.ReceiveNext();
        }
    }
}
