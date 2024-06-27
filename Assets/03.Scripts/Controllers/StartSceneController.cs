using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private GameObject _loginGoogleButton;
    [SerializeField] private GameObject _startButton;

    [SerializeField] private TMP_Text _idText;
    [SerializeField] private TMP_Text _idText1;
    [SerializeField] private TMP_Text _idText2;
    [SerializeField] private TMP_Text _idText3;

    private void Start()
    {
        GameManager.I.SoundManager.StartBGM("StartScene");
    }

    private void Update()
    {
        //_idText.text = "GPGS UserID : " + GameManager.I.GPGSManager.GetGPGSUserID();
        //_idText1.text = "GPGS DisplayName : " + GameManager.I.GPGSManager.GetGPGSUserDisplayName();
        _idText2.text = "뒤끝 연결 ? : " + GameManager.I.BackendManager.IsConnect();
        _idText3.text = "GameData.LoginID : " + GameManager.I.DataManager.GameData.LoginID;
    }

    public void GameStartButton()
    {
        if(UnityEngine.Application.isEditor)
        {
            GameManager.I.SoundManager.StartSFX("ButtonClick");
            //GameManager.I.DataManager.GameData.LoginID = GameManager.I.GPGSManager.GetGPGSUserID();
            GameManager.I.BackendManager.Login();
        }
        else
        {
            //if (GameManager.I.GPGSManager.GetGPGSUserID() == "0")
            //{
            //    GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
            //}
            //else
            //{
            //    GameManager.I.SoundManager.StartSFX("ButtonClick");
            //    GameManager.I.DataManager.GameData.LoginID = GameManager.I.GPGSManager.GetGPGSUserID();
            //    GameManager.I.BackendManager.Login();
            //}
        }
    }
}
