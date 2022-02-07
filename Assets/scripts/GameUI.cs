using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    public PLayerUIContainer[] playerContainer;
    public TextMeshProUGUI WinText;


    public static GameUI instance;

    private void Awake()
    {
        //set the instance to this script
        instance = this;
    }
    private void Start()
    {
        InitializePlayerUI();
    }

    private void Update()
    {
        UpdatePlayerUI();
    }
    void InitializePlayerUI()
    {
        //loop through all containers
        for (int i = 0; i <playerContainer.Length; ++i)
        {
            PLayerUIContainer container = playerContainer[i];

            //only enable and modify the UI container which we need
            if (i < PhotonNetwork.PlayerList.Length)
            {
                container.Obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[i].NickName;
                container.HattimeSlider.maxValue = GameManager.instance.timetowin;
            }
            else
                container.Obj.SetActive(false);
        }
    }
   
    void UpdatePlayerUI()
    {
        //loop through all player
        for (int i = 0; i < GameManager.instance.players.Length; ++i)
        {
            if (GameManager.instance.players[i] != null)
                playerContainer[i].HattimeSlider.value = GameManager.instance.players[i].Curhattime;
        }
    }

    public void SetWinText(string winnername)
    {
        WinText.gameObject.SetActive(true);
        WinText.text = winnername + "wins";
    }
}

[System.Serializable]
public class PLayerUIContainer
{
    public GameObject Obj;
    public TextMeshProUGUI nameText;
    public Slider HattimeSlider;
}
