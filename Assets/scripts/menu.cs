using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class menu : MonoBehaviourPunCallbacks
{
    [Header("Screen")]
    public GameObject MainScreen;
    public GameObject LobbyScreen;

    [Header("Main Screen")]
    public Button CreatRoomButton;
    public Button JoinRoomButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button StartGameButton;

    void Start()
    {
        //Disable the buton at the start as we are not connected to the server
        CreatRoomButton.interactable = false;
        JoinRoomButton.interactable = false;
    }

    //called when connected to the master server
    //enables "Creat" and "join" buttons
    public override void OnConnectedToMaster()
    {
        CreatRoomButton.interactable = true;
        JoinRoomButton.interactable = true;
    }

    public void setscreen(GameObject screen)
    {
        // Deactivae all screen
        MainScreen.SetActive(false);
        LobbyScreen.SetActive(false);

        //enable the requested screen
        screen.SetActive(true);
    }

    // Called when the "creat room" button is pressed
    public void OnCreatroombutton(TMP_InputField RoomNameInput)
    {
        NetworkManager.instance.Creatroom(RoomNameInput.text);
    }

    // Called when "join room" button is pressed
    public void OnJoinRoomButton(TMP_InputField RoomNameInput)
    {
        NetworkManager.instance.Joinroom(RoomNameInput.text);
    }

    // Called when the player name field is updated
    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    //called when we join a room
    public override void OnJoinedLobby()
    {
        setscreen(LobbyScreen);
        //Since the is a new player in the lobby tell everyone about it
        photonView.RPC("UpdatelobbyUI", RpcTarget.All);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatelobbyUI();
    }

    // update the lobby UI to show player Name and host button
    [PunRPC]
    public void UpdatelobbyUI()
    {
        playerListText.text = "";

        //Display all player in the lobby
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        if (PhotonNetwork.IsMasterClient)
            CreatRoomButton.interactable = true;
        else
            CreatRoomButton.interactable = false;
    }

    //called when the leave lobby button is pressed
    public void OnleaveButton()
    {
        PhotonNetwork.LeaveLobby();
        setscreen(MainScreen);
    }

    //called when start button is pressed 
    //only the host can click this button
    public void OnstartGameButton()
    {
        NetworkManager.instance.photonView.RPC("Changescene", RpcTarget.All, "Game");
    }
}

