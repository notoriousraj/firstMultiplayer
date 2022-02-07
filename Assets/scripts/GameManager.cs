using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool Gameended = false;          //has the game ended?
    public float timetowin;                 //time the player need to hold to win
    public float invincibleduration;        //how long the player gets the hat, are the
    private float hatpickuptime;            //the time the hat is picked by the current player

    [Header("Players")]
    public string PLayerPrefablocaton;
    public Transform[] spawnpoint;
    public PlayerController[] players;
    public int playerwithhat;
    private int playeringame;

    public static GameManager instance;

    private void Awake()
    {
        // instance
        instance = this;
    }

    private void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImIngame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImIngame()
    {
        playeringame++;

        //when all the player are in the game - spwan the players
        if (playeringame == PhotonNetwork.PlayerList.Length)
            SpawnPLayer();
    }

    //spwan a player and initialize it
    void SpawnPLayer()
    {
        // instantiate player across the network
        GameObject playerOBJ = PhotonNetwork.Instantiate(PLayerPrefablocaton, spawnpoint[Random.Range(0, spawnpoint.Length)].position, Quaternion.identity);

        //get the player script
        PlayerController playerscript = playerOBJ.GetComponent<PlayerController>();

        //initialize the player 
        playerscript.photonView.RPC("initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    //Return the player who has the requested ID
    public PlayerController GetPlayer1(int playerID)
    {
        return players.First(x => x.id == playerID);
    }

    //return the playerof the requested gameobject
    public PlayerController GetPlayer(GameObject playerOBJ)
    {
        return players.First(x => x.gameObject == playerOBJ);
    }

    //called when the player hits the hattedd player - giving them the hat

    [PunRPC]
    public void GiveHat(int playerID, bool initialgive)
    {
        //Rmove the hat fromt the current Hatted Player
        if (!initialgive)
            GetPlayer1(playerwithhat).SetHat(false);
        //give the hat to the new player
        playerwithhat = playerID;
        GetPlayer1(playerID).SetHat(true);
        hatpickuptime = Time.time;
}
    public bool CanGetHat()
    {
        if (Time.time > hatpickuptime + invincibleduration)
            return true;
        else
            return false;

    }

    //called when the player holds the hat for the winning amount of time 
    [PunRPC]
    void WinGame(int playerID)
    {
        Gameended = true;
        PlayerController player = GetPlayer1(playerID);
        //set the ui to show who won 
        GameUI.instance.SetWinText(player.PhotonPlayer.NickName);

        Invoke("GobacktoMenu", 0.3f);
    }

    void GobacktoMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.Changescene("Menu");
    }
}
