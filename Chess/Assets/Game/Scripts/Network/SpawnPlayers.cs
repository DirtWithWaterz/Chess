using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using mudz;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    private LobbyManager lobbyManager;

    public GameObject p1;
    public GameObject p2;

    void Start(){
        Spawn();
        lobbyManager = GameObject.Find("GUI").GetComponent<LobbyManager>();
    }
    public void Spawn()
    {
        if(PhotonNetwork.IsMasterClient)
            p1 = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        else
            p2 = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
    }
}
