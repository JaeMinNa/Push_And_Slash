using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public GameObject Player;
    
    //public GameObject EnemyPlayer;
    //private bool _isFull;

    //private void Update()
    //{
    //    if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
    //    {
    //        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && !_isFull)
    //        {
    //            _isFull = true;

    //            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
    //            {
    //                if (PhotonNetwork.PlayerList[0].NickName == PhotonNetwork.NickName)
    //                    EnemyPlayer = FindPlayerObjectByNickName(PhotonNetwork.PlayerList[1].NickName);
    //                else EnemyPlayer = FindPlayerObjectByNickName(PhotonNetwork.PlayerList[0].NickName);
    //            }
    //        }
    //    }
    //}

    public void Init()
    {
        //_isFull = false;
    }

    public void Release()
    {

    }

    //private GameObject FindPlayerObjectByNickName(string nickName)
    //{
    //    foreach (Player player in PhotonNetwork.PlayerList)
    //    {
    //        if (player.NickName == nickName)
    //        {
    //            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
    //            {
    //                PhotonView photonView = obj.GetComponent<PhotonView>();
    //                if (photonView != null && photonView.Owner == player)
    //                {
    //                    return obj;
    //                }
    //            }
    //        }
    //    }
    //    return null;
    //}
}
