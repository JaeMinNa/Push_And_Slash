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

    private void Update()
    {
        _idText2.text = "뒤끝 연결 ? : " + GameManager.I.BackendManager.IsConnect();
        _idText3.text = "GameData.LoginID : " + GameManager.I.DataManager.GameData.LoginID;
    }

    public void GameStartButton()
    {
        //if (UnityEngine.Application.isEditor)
        //{
        //    GameManager.I.SoundManager.StartSFX("ButtonClick");
        //    GameManager.I.DataManager.GameData.LoginID = "0";
        //    GameManager.I.BackendManager.Login();
        //}
        //else
        //{
        //    GameManager.I.SoundManager.StartSFX("ButtonClick");
        //    GameManager.I.BackendManager.Login();
        //}

        GameManager.I.SoundManager.StartSFX("ButtonClick");
        GameManager.I.BackendManager.Login();
    }
}
