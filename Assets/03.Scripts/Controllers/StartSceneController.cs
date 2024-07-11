using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private GameObject _startButton;

    private void Start()
    {
        GameManager.I.SoundManager.StartBGM("StartScene");
    }

    public void GameStartButton()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        GameManager.I.BackendManager.Login();
    }
}
