using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

public class StageController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private TMP_Text _stageText;
    [HideInInspector] public int StageCoin;

    [Header("GameClear")]
    [SerializeField] private GameObject _gameClear;
    [SerializeField] private GameObject _starEffect;
    [SerializeField] private TMP_Text _stageClearCoinText;
    [SerializeField] private TMP_Text _stageCoinBonusText;
    [SerializeField] private GameObject[] _stageClearStarObjects;
    [SerializeField] private TMP_Text _stageClearLevelText;
    [SerializeField] private TMP_Text _stageClearExpText;
    [SerializeField] private Slider _stageClearExpSlider;
    private bool _isGameClear;
    [HideInInspector] public int StageCoinBonus;

    [Header("GameOver")]
    [SerializeField] private GameObject _gameOver;
    [SerializeField] private TMP_Text _stageGameOverCoinText;
    [SerializeField] private TMP_Text _stageGameOverLevelText;
    [SerializeField] private TMP_Text _stageGameOverExpText;
    [SerializeField] private Slider _stageGameOverExpSlider;
    private bool _isGameOver;

    [Header("StageTitle")]
    [SerializeField] private TMP_Text _stageTitleText;

    [Header("Camera")]
    [SerializeField] private GameObject _virtualCamera;
    [SerializeField] private TMP_Text _cameraSettingValue;
    private Camera _mainCamera;
    private CameraController _cameraController;

    [HideInInspector] public int StageExp;
    [HideInInspector] public float _time;
    private GameObject _player;
    private GameData _gameData;
    private DataManager _dataManager;
    private List<CharacterData> _inventory;
    private NetworkManager _networkManager;
    private bool _isAd;

    private void Start()
    {
        _player = GameManager.I.PlayerManager.Player;
        _gameData = GameManager.I.DataManager.GameData;
        _dataManager = GameManager.I.DataManager;
        _inventory = _dataManager.DataWrapper.CharacterInventory;
        _mainCamera = Camera.main;
        _cameraController = _mainCamera.GetComponent<CameraController>();
        _isGameClear = false;
        _isGameOver = false;
        _isAd = false;
        StageCoin = 0;
        StageCoinBonus = 0;

        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        {
            _time = 180f;
            StageSetting();
            GameManager.I.SoundManager.StartBGM("BattleScene");
        }
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            _networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
            _time = 0;
            GameManager.I.SoundManager.StartBGM("MultiScene");
        }

        StageTextSetting();
        CoinSetting();
        StartCoroutine(COStartCameraSetting());
        CameraSetting();
    }

    private void Update()
    {
        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        {
            _time -= Time.deltaTime;
            TimeTextUpdate();

            if (!IsEnemy() && !_isGameClear)
            {
                _isGameClear = true;
                GameClear();
            }

            if ((_time <= 0 || !IsSurvive()) && !_isGameOver)
            {
                _isGameOver = true;
                GameOver();
            }
        }
        else if(GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            _time += Time.deltaTime;
            TimeTextUpdate();
        }
    }

    #region UI
    private void TimeTextUpdate()
    {
        string text;
        if (Mathf.Floor(_time % 60) < 10)
        {
            text = "0" + Mathf.Floor(_time % 60).ToString();
        }
        else text = Mathf.Floor(_time % 60).ToString();

        _timeText.text = Mathf.Floor(_time / 60).ToString() + ":" + text;
    }

    private void StageTextSetting()
    {
        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        {
            string name = GameManager.I.ScenesManager.CurrentSceneName.Substring(11);
            _stageTitleText.text = _stageText.text = "Chapter " + name + "-" + _gameData.Stage;
            _stageText.text = "Chapter " + name + "-" + _gameData.Stage;
        }
        else if(GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            _stageTitleText.text = "MULTI PLAY";
            _stageText.text = "MULTI PLAY";
        }
        
    }

    public void CoinSetting()
    {
        _coinText.text = StageCoin.ToString();
    }
    #endregion

    #region Game
    public void GameClear()
    {
        Time.timeScale = 0f;
        GameManager.I.SoundManager.StartBGM("Victory");
        GameManager.I.SoundManager.StartSFX("Win");
        GameManager.I.AdsManager.DestroyAd();

        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        {
            _stageClearCoinText.text = StageCoin.ToString();
            if (_time >= 120)
            {
                StarEffectSetting(3);
                StageCoinBonus = 300;
            }
            else if (_time >= 60)
            {
                StarEffectSetting(2);
                StageCoinBonus = 200;
            }
            else
            {
                StarEffectSetting(1);
                StageCoinBonus = 100;
            }

            LevelExpUp(StageExp);
            GameManager.I.DataManager.GameData.Coin += StageCoin;
            if (_gameData.Stage <= 19) GameManager.I.DataManager.GameData.Stage++;
        }
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            _stageClearCoinText.text = "500";

            if (_time <= 60)
            {
                StarEffectSetting(3);
                StageCoinBonus = 300;
                LevelExpUp(40);
            }
            else if (_time <= 120)
            {
                StarEffectSetting(2);
                StageCoinBonus = 200;
                LevelExpUp(30);
            }
            else
            {
                StarEffectSetting(1);
                StageCoinBonus = 100;
                LevelExpUp(20);
            }

            GameManager.I.DataManager.GameData.Coin += 500;
            GameManager.I.DataManager.GameData.RankPoint++;
            GameManager.I.DataManager.GameData.Win++;
        }

        _stageCoinBonusText.text = StageCoinBonus.ToString();
        GameManager.I.DataManager.GameData.Coin += StageCoinBonus;

        PlayerDataToInventoryData();
        _stageClearLevelText.text = _dataManager.PlayerData.Level.ToString();
        _stageClearExpText.text = _dataManager.PlayerData.CurrentExp.ToString() + "/" + _dataManager.PlayerData.MaxExp.ToString();
        _stageClearExpSlider.value = (float)_dataManager.PlayerData.CurrentExp / _dataManager.PlayerData.MaxExp;

        _gameClear.SetActive(true);
        GameManager.I.DataManager.DataSave();
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        GameManager.I.SoundManager.StopBGM();
        GameManager.I.SoundManager.StartSFX("Lose");
        GameManager.I.AdsManager.DestroyAd();

        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        {
            _stageGameOverCoinText.text = StageCoin.ToString();
            GameManager.I.DataManager.GameData.Coin += StageCoin;

            LevelExpUp(StageExp);
        }
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            _stageGameOverCoinText.text = "300";
            GameManager.I.DataManager.GameData.Coin += 300;
            GameManager.I.DataManager.GameData.Lose++;
            if(_gameData.RankPoint >= 1) GameManager.I.DataManager.GameData.RankPoint--;

            LevelExpUp(10);
        }

        PlayerDataToInventoryData();
        _stageGameOverLevelText.text = _dataManager.PlayerData.Level.ToString();
        _stageGameOverExpText.text = _dataManager.PlayerData.CurrentExp.ToString() + "/" + _dataManager.PlayerData.MaxExp.ToString();
        _stageGameOverExpSlider.value = (float)_dataManager.PlayerData.CurrentExp / _dataManager.PlayerData.MaxExp;

        _gameOver.SetActive(true);
        GameManager.I.DataManager.DataSave();
    }

    private void StageSetting()
    {
        transform.GetChild(_gameData.Stage - 1).gameObject.SetActive(true);
    }

    private bool IsEnemy()
    {
        int enemyCount = transform.GetChild(_gameData.Stage - 1).childCount;

        for (int i = 0; i < enemyCount; i++)
        {
            if (transform.GetChild(_gameData.Stage - 1).GetChild(i).gameObject.activeSelf) return true;
        }

        return false;
    }

    private bool IsSurvive()
    {
        if (_player.transform.position.y <= -10f) return false;

        return true;
    }

    public void TimeScaleStart()
    {
        Time.timeScale = 1f;
    }

    public void LobbySceneButton()
    {
        if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1") _networkManager.DisConnect();

        GameManager.I.SoundManager.StartSFX("ButtonClick");
        GameManager.I.AdsManager.DestroyAd();
        GameManager.I.ScenesManager.LoadLoadingScene("LobbyScene");
    }

    public void GiveUpMultiButton()
    {
        if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1") _networkManager.DisConnect();

        if (GameManager.I.DataManager.GameData.RankPoint >= 1) GameManager.I.DataManager.GameData.RankPoint--;
        if (GameManager.I.DataManager.GameData.Lose >= 1) GameManager.I.DataManager.GameData.Lose++;
        
        GameManager.I.DataManager.DataSave();
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        GameManager.I.AdsManager.DestroyAd();
        GameManager.I.ScenesManager.LoadLoadingScene("LobbyScene");
    }

    private void StarEffectSetting(int num)
    {
        int count = _starEffect.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            _starEffect.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < 3; i++)
        {
            _stageClearStarObjects[i].SetActive(false);
        }

        if (num == 0) return;
        else if (num == 1 || num == 2)
        {
            for (int i = 0; i < num; i++)
            {
                _starEffect.transform.GetChild(i).gameObject.SetActive(true);
                _stageClearStarObjects[i].SetActive(true);
            }
        }
        else
        {
            _starEffect.transform.GetChild(2).gameObject.SetActive(true);
            for (int i = 0; i < num; i++)
            {
                _stageClearStarObjects[i].SetActive(true);
            }
        }
    }

    private void LevelExpUp(int exp)
    {
        if (_dataManager.PlayerData.Level >= 30)
        {
            GameManager.I.DataManager.PlayerData.CurrentExp += 0;
            return;
        }

        GameManager.I.DataManager.PlayerData.CurrentExp += exp;

        if(_dataManager.PlayerData.CurrentExp >= _dataManager.PlayerData.MaxExp)
        {
            while (true)
            {
                GameManager.I.DataManager.PlayerData.Level++;
                GameManager.I.DataManager.PlayerData.CurrentExp = _dataManager.PlayerData.CurrentExp - _dataManager.PlayerData.MaxExp;
                GameManager.I.DataManager.PlayerData.MaxExp = 20 + (_dataManager.PlayerData.Level * 10);

                if (_dataManager.PlayerData.CurrentExp < _dataManager.PlayerData.MaxExp) break;
            }
        }
    }

    private void PlayerDataToInventoryData()
    {
        int inventoryOrder = FindInventoryOrder(_dataManager.PlayerData);

        GameManager.I.DataManager.DataWrapper.CharacterInventory[inventoryOrder].CurrentExp = _dataManager.PlayerData.CurrentExp;
        GameManager.I.DataManager.DataWrapper.CharacterInventory[inventoryOrder].MaxExp = _dataManager.PlayerData.MaxExp;
        GameManager.I.DataManager.DataWrapper.CharacterInventory[inventoryOrder].Level = _dataManager.PlayerData.Level;
    }

    private int FindInventoryOrder(CharacterData data)
    {
        int count = _inventory.Count;

        for (int i = 0; i < count; i++)
        {
            if (data.Tag == _inventory[i].Tag) return i;
            else continue;
        }

        return -1;
    }

    public void RewardAdButton()
    {
        if(!_isAd)
        {
            _isAd = true;
            GameManager.I.SoundManager.StartSFX("ButtonClick");
            GameManager.I.AdsManager.LoadRewardedAd();
        }
        else GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
    }

    public void RewardCoinSetting()
    {
        _stageClearCoinText.text = (StageCoin * 2).ToString();
        _stageCoinBonusText.text = (StageCoinBonus * 2).ToString();
        _stageGameOverCoinText.text = (StageCoin * 2).ToString();
    }
    #endregion

    #region Camera
    IEnumerator COStartCameraSetting()
    {
        yield return new WaitForSecondsRealtime(4f);

        _virtualCamera.SetActive(false);
        _mainCamera.transform.rotation = Quaternion.Euler(_cameraController.OriginCameraRotation);
    }

    public void CameraSettingButtonUp()
    {
        if(_gameData.CameraSettingValue == 5)
        {
            GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
        }
        else
        {
            GameManager.I.SoundManager.StartSFX("ButtonClick");

            GameManager.I.DataManager.GameData.CameraSettingValue++;
            CameraSetting();
        }
    }

    public void CameraSettingButtonDown()
    {
        if (_gameData.CameraSettingValue == 1)
        {
            GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
        }
        else
        {
            GameManager.I.SoundManager.StartSFX("ButtonClick");

            GameManager.I.DataManager.GameData.CameraSettingValue--;
            CameraSetting();
        }
    }

    private void CameraSetting()
    {
        _cameraSettingValue.text = _gameData.CameraSettingValue.ToString();

        if(_gameData.CameraSettingValue == 1)
        {
            _cameraController.Offset.y = 5f;
            _cameraController.Offset.z = -5f;
            _cameraController.OriginCameraRotation.x = 45f;
            _mainCamera.transform.rotation = Quaternion.Euler(_cameraController.OriginCameraRotation);
        }
        else if(_gameData.CameraSettingValue == 2)
        {
            _cameraController.Offset.y = 6f;
            _cameraController.Offset.z = -5f;
            _cameraController.OriginCameraRotation.x = 50f;
            _mainCamera.transform.rotation = Quaternion.Euler(_cameraController.OriginCameraRotation);
        }
        else if (_gameData.CameraSettingValue == 3)
        {
            _cameraController.Offset.y = 7.5f;
            _cameraController.Offset.z = -5.5f;
            _cameraController.OriginCameraRotation.x = 53f;
            _mainCamera.transform.rotation = Quaternion.Euler(_cameraController.OriginCameraRotation);
        }
        else if (_gameData.CameraSettingValue == 4)
        {
            _cameraController.Offset.y = 9f;
            _cameraController.Offset.z = -6.5f;
            _cameraController.OriginCameraRotation.x = 53f;
            _mainCamera.transform.rotation = Quaternion.Euler(_cameraController.OriginCameraRotation);
        }
        else if (_gameData.CameraSettingValue == 5)
        {
            _cameraController.Offset.y = 11f;
            _cameraController.Offset.z = -7f;
            _cameraController.OriginCameraRotation.x = 56f;
            _mainCamera.transform.rotation = Quaternion.Euler(_cameraController.OriginCameraRotation);
        }
    }
    #endregion

}
