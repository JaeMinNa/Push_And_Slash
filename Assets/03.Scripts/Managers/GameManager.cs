using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameManager은 Manager을 관리하는 하나의 역할만 함

    public PlayerManager PlayerManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public DataManager DataManager { get; private set; }
    public SoundManager SoundManager { get; private set; }
    public ScenesManager ScenesManager { get; private set; }
    public BackendManager BackendManager { get; private set; }
    public AdsManager AdsManager { get; private set; }

    public static GameManager I;

    private void Awake()
    {
        if (I == null)
        {
            I = this;
        }

        DataManager = GetComponentInChildren<DataManager>();
        PlayerManager = GetComponentInChildren<PlayerManager>();
        ScenesManager = GetComponentInChildren<ScenesManager>();
        SoundManager = GetComponentInChildren<SoundManager>();
        UIManager = GetComponentInChildren<UIManager>();
        BackendManager = GetComponentInChildren<BackendManager>();
        AdsManager = GetComponentInChildren<AdsManager>();

        Init();
    }

    private void Init()
    {
        DataManager.Init();
        PlayerManager.Init();
        SoundManager.Init();
        ScenesManager.Init();
        UIManager.Init();
        BackendManager.Init();
        AdsManager.Init();
    }

    private void Release()
    {
        DataManager.Release();
        PlayerManager.Release();
        SoundManager.Release();
        ScenesManager.Release();
        UIManager.Release();
        BackendManager.Release();
        AdsManager.Release();
    }
}