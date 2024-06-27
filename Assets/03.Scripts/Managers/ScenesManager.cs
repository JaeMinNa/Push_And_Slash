using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class ScenesManager : MonoBehaviour
{
    public string CurrentSceneName;

    public void Init()
    {
        CurrentSceneName = SceneManager.GetActiveScene().name;

        if (CurrentSceneName == "BattleScene1")
        {
            GameObject playerPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.I.DataManager.PlayerData.Tag), Vector3.zero, Quaternion.identity);
            GameManager.I.PlayerManager.Player = playerPrefab;
        }
    }

    public void Release()
    {

    }

    public void LoadLoadingScene(string name)
    {
        LoadingScene.LoadScene(name);
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    private void OnApplicationQuit()
    {
        if(CurrentSceneName == "MultiBattleScene1")
        {
            if(GameManager.I.DataManager.GameData.RankPoint >= 1) GameManager.I.DataManager.GameData.RankPoint--;
            if (GameManager.I.DataManager.GameData.Lose >= 1) GameManager.I.DataManager.GameData.Lose++;

            GameManager.I.DataManager.DataSave();
        }
    }
}
