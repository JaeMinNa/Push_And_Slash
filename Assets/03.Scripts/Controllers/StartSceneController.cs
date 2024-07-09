using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private GameObject _startButton;

    [SerializeField] private TMP_Text _idText2;
    [SerializeField] private TMP_Text _idText3;

    private void Start()
    {
        GameManager.I.SoundManager.StartBGM("StartScene");
    }

    public void GameStartButton()
    {
        if (UnityEngine.Application.isEditor)
        {
            GameManager.I.SoundManager.StartSFX("ButtonClick");
            GameManager.I.DataManager.GameData.LoginID = "0";
            GameManager.I.BackendManager.Login();
        }
        else
        {
            GameManager.I.SoundManager.StartSFX("ButtonClick");
            GameManager.I.BackendManager.Login();
        }
    }
}
