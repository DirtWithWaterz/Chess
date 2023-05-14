using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MainMenu : MonoBehaviourPun
{
    [SerializeField] GameObject settingsPage;

    bool settingsToggle;

    void Start(){
        PhotonNetwork.ConnectUsingSettings();
    }


    public void StartGame(){
        SceneManager.LoadScene("LoadingLobby");
    }
    public void QuitGame(){
        Application.Quit();
    }
    public void ToggleSettings(){
        settingsToggle = !settingsToggle;
        settingsPage.SetActive(settingsToggle);
    }
}
