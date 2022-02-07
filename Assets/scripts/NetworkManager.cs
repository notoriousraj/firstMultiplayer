using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Instance
    public static NetworkManager instance;

    void Awake()
    {
        // if an instance already exist and it's not this one - destroy us
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            // set the instance
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }
    public void Start()
    {
        // connect to the master server
        PhotonNetwork.ConnectUsingSettings();
    }

    // Attempt to creat an room
    public void Creatroom(string roomname)
    {
        PhotonNetwork.CreateRoom(roomname);
    }

    //attempt to join the room
    public void Joinroom(string roomname)
    {
        PhotonNetwork.JoinRoom(roomname);
    }

    // Change the scene using photon's system
    [PunRPC]
    public void Changescene(string scenename)
    {
        PhotonNetwork.LoadLevel(scenename);
    }
}

