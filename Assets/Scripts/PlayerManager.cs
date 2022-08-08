using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    private PhotonView _photonView;
    private float[,] coordinates = {
        {-1.4f, -35.24f},
        {8.2f, -5.1f},
        {37.8f, -5.1f},
        {29.4f, 10.09f},
        {-8.4f, -28.7f}
    };

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        if(_photonView.IsMine)
            CreatePlayer();
    }

    private void CreatePlayer()
    {
        int randomIndex = UnityEngine.Random.Range(0, coordinates.Length - 1);

        Vector3 playerPosition = new Vector3(coordinates[randomIndex, 0], 0, coordinates[randomIndex, 1]);
        PhotonNetwork.Instantiate("Player", playerPosition, Quaternion.identity);
    }
}
