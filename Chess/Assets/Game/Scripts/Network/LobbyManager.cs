using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class LobbyManager : MonoBehaviourPun
{
    public GameObject _startButton;

    [SerializeField] private TMP_Text p1;
    [SerializeField] private TMP_Text p2;

    SpawnPlayers sp;

    public void Start()
    {
        sp = GameObject.FindObjectOfType<SpawnPlayers>();
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(UpdateP1Text), RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer.NickName);
            _startButton.SetActive(true);
            p2.text = "None";
        } else{
            photonView.RPC(nameof(UpdateP2Text), RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer.NickName);
            _startButton.SetActive(false);
        }
        
        
        
    }

    [PunRPC]
    public void UpdateP1Text(string nick){
        p1.text = nick;
    }

    [PunRPC]
    public void UpdateP2Text(string nick){
        p2.text = nick;
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void ReturnToMenu(){
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(sp.p1);
        else
            PhotonNetwork.Destroy(sp.p2);
        
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("MainMenu");
    }
}
