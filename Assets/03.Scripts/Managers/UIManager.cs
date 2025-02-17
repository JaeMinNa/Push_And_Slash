using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECM2;
using ECM2.Examples.Slide;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ECM2.Examples.ThirdPerson;

public class UIManager : MonoBehaviour
{
    [Header("CoolTime Image")]
    [SerializeField] private Image _dashImage;
    [SerializeField] private Image _skillImage;

    [Header("Panel")]
    [SerializeField] private GameObject _pause;
    [SerializeField] private TMP_Text _pauseChapterText;

    [Header("Setting")]
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private GameObject _userNamePanel;
    [SerializeField] private TMP_InputField _userNameInputField;
    [SerializeField] private GameObject _dataDeletePanel;

    private GameObject _player;
    private Animator _playerAnimator;
    private PlayerCharacter _playerCharacter;
    private ThirdPersonController _thirdPersonController;
    private CharacterData _playerData;
    private GameData _gameData;
    private LobbyController _lobbyController;
    private float _dashTime;
    private float _skillTime;
    private PhotonView _photonView;

    private void Update()
    {
        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1" || GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            _dashTime += Time.deltaTime;
            _skillTime += Time.deltaTime;
        }
    }

    public void PlayerSetting()
    {
        _player = GameManager.I.PlayerManager.Player;
        _playerAnimator = _player.transform.GetChild(0).GetComponent<Animator>();
        _playerCharacter = _player.GetComponent<PlayerCharacter>();
        _playerData = GameManager.I.DataManager.PlayerData;
        _photonView = _player.GetComponent<PhotonView>();
    }

    public void Init()
    {
        _player = GameManager.I.PlayerManager.Player;
        _playerCharacter = _player.GetComponent<PlayerCharacter>();
        _thirdPersonController = _player.GetComponent<ThirdPersonController>();
        _playerAnimator = _player.transform.GetChild(0).GetComponent<Animator>();
        _playerData = GameManager.I.DataManager.PlayerData;
        _gameData = GameManager.I.DataManager.GameData;

        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1" || GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            _dashTime = 0f;
            _skillTime = 0f;

            StartCoroutine(COCoolTimeRoutine(_dashImage, _playerData.DashCoolTime));
            StartCoroutine(COCoolTimeRoutine(_skillImage, _playerData.SkillCoolTime));
        }
    }

    public void Release()
    {

    }

    #region Sound
    public void SoundSetting()
    {
        float sfx = GameManager.I.DataManager.GameData.SFXValume;
        float bgm = GameManager.I.DataManager.GameData.BGMValume;

        if(GameManager.I.ScenesManager.CurrentSceneName != "StartScene")
        {
            _sfxSlider.value = sfx;
            _bgmSlider.value = bgm;
        }

        if (sfx == -40f)	// -40�� ��, ������ ����
        {
            _audioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            _audioMixer.SetFloat("SFX", sfx);
        }

        if (bgm == -40f)	// -40�� ��, ������ ����
        {
            _audioMixer.SetFloat("BGM", -80f);
        }
        else
        {
            _audioMixer.SetFloat("BGM", bgm);
        }
    }

    public void SFXControl()
    {
        float sound = _sfxSlider.value;
        GameManager.I.DataManager.GameData.SFXValume = sound;

        if (sound == -40f)	// -40�� ��, ������ ����
        {
            _audioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            _audioMixer.SetFloat("SFX", sound);
        }

        GameManager.I.DataManager.DataSave();
    }

    public void BGMControl()
    {
        float sound = _bgmSlider.value;
        GameManager.I.DataManager.GameData.BGMValume = sound;

        if (sound == -40f)	// -40�� ��, ������ ����
        {
            _audioMixer.SetFloat("BGM", -80f);
        }
        else
        {
            _audioMixer.SetFloat("BGM", sound);
        }

        GameManager.I.DataManager.DataSave();
    }
    #endregion

    #region Joystick
    public void PlayerJumpButtonUp()
    {
        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
            _player.GetComponent<Character>().StopJumping();
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            if(_player.GetComponent<PhotonView>().IsMine)
                _player.GetComponent<Character>().StopJumping();
        }

    }

    public void PlayerJumpButtonDown()
    {
        _player.GetComponent<Character>().Jump();
        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
            _player.GetComponent<Character>().Jump();
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            if (_player.GetComponent<PhotonView>().IsMine)
                _player.GetComponent<Character>().Jump();
        }
    }

    public void PlayerAttack()
    {
        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
            _playerAnimator.SetTrigger("Attack");
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            if (_player.GetComponent<PhotonView>().IsMine) _playerCharacter.PhotonView.RPC("PlayerAttackRPC", RpcTarget.AllViaServer);
        }
    }

    public void PlayerDashButtonUp()
    {
        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
            _playerCharacter.UnCrouch();
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            if (_player.GetComponent<PhotonView>().IsMine)
                _playerCharacter.UnCrouch();
        }
    }

    public void PlayerDashButtonDown()
    {
        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        {
            if (_dashTime >= _playerData.DashCoolTime)
            {
                StartCoroutine(COCoolTimeRoutine(_dashImage, _playerData.DashCoolTime));
                _playerAnimator.SetTrigger("Dash");
                _playerCharacter.Crouch();
                _dashTime = 0f;
            }
            else GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
        }
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            if (_player.GetComponent<PhotonView>().IsMine)
            {
                if (_dashTime >= _playerData.DashCoolTime)
                {
                    StartCoroutine(COCoolTimeRoutine(_dashImage, _playerData.DashCoolTime));
                    _playerCharacter.PhotonView.RPC("PlayerDashRPC", RpcTarget.AllViaServer);
                    _playerCharacter.Crouch();
                    _dashTime = 0f;
                }
                else GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
            }
        }
    }

    public void PlayerSkillButton()
    {
        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        {
            if (_skillTime >= _playerData.SkillCoolTime)
            {
                StartCoroutine(COCoolTimeRoutine(_skillImage, _playerData.SkillCoolTime));
                StartCoroutine(COIsSkillFalse());
                _playerAnimator.SetTrigger("Skill");
                _skillTime = 0f;
                _playerCharacter.IsSkill = true;
            }
            else GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
        }
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            if (_player.GetComponent<PhotonView>().IsMine)
            {
                if (_skillTime >= _playerData.SkillCoolTime)
                {
                    StartCoroutine(COCoolTimeRoutine(_skillImage, _playerData.SkillCoolTime));
                    StartCoroutine(COIsSkillFalse());
                    _playerCharacter.PhotonView.RPC("PlayerSkillRPC", RpcTarget.AllViaServer);
                    _skillTime = 0f;
                    _playerCharacter.IsSkill = true;
                }
                else GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
            }
        }
    }

    private IEnumerator COCoolTimeRoutine(Image image, float coolTime)
    {
        float time = coolTime;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            float per = timer / time;
            image.fillAmount = per;

            if (timer >= time)
            {
                image.fillAmount = 1f;
                break;
            }
            yield return null;
        }
    }

    private IEnumerator COIsSkillFalse()
    {
        yield return new WaitForSeconds(1f);

        _playerCharacter.IsSkill = false;
    }
    #endregion

    #region Pause
    public void PauseStartButton()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");

        if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        {
            Time.timeScale = 0f;
            _pauseChapterText.text = "CHAPTER 1-" + _gameData.Stage;
        }
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            _pauseChapterText.text = "MULTI PLAY";
        }

        _pause.SetActive(true);
    }

    public void PauseStopButton()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        Time.timeScale = 1f;
        _pause.SetActive(false);
    }
    #endregion

    #region Setting
    public void SettingActive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _settingPanel.SetActive(true);
    }

    public void SettingInactive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _settingPanel.SetActive(false);
    }

    public void ExitGame()
    {
        // ���� ���� ȯ���� �������̸� ������ �÷��̸�� ����
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        // ���� ���� ȯ���� �����Ͱ� �ƴϸ� ���α׷� ����
        #else
        Application.Quit();
        #endif
    }

    public void UserNameSettingActive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _userNameInputField.text = GameManager.I.DataManager.GameData.UserName;
        _userNamePanel.SetActive(true);
    }

    public void UserNameInput()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        GameManager.I.DataManager.GameData.UserName = _userNameInputField.text;

        if(GameManager.I.ScenesManager.CurrentSceneName == "LobbyScene")
        {
            _lobbyController = GameObject.FindWithTag("LobbyController").GetComponent<LobbyController>();
            _lobbyController.UserNameSetting();
        }

        _userNamePanel.SetActive(false);
        GameManager.I.DataManager.DataSave();
        GameManager.I.BackendManager.Save();
    }

    public void DataDeleteSettingActive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _dataDeletePanel.SetActive(true);
    }

    public void DataDeleteSettingInactive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _dataDeletePanel.SetActive(false);
    }

    public void DataDelete()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        GameManager.I.DataManager.DataDelete();
        ExitGame();
    }

    public void LobbyButton()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        DisConnect();
        GameManager.I.ScenesManager.LoadLoadingScene("LobbyScene");
    }

    private void DisConnect()
    {
        PhotonNetwork.Disconnect();
    }
    #endregion
}
