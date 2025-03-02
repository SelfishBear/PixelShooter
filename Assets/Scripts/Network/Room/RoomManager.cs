using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Player;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _player;
    [Space]
    [SerializeField] private Transform _spawnPoint;
    
    
    private void Start()
    {
        Debug.Log("Connecting....");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to server");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        PhotonNetwork.JoinOrCreateRoom("Test", null, null);
        
        Debug.Log("Joined lobby");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined room");
        
        GameObject _player = PhotonNetwork.Instantiate("Player", _spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
    }
}