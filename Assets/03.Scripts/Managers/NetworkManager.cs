using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using ECM2.Examples.Slide;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _multiPlayPanel;
    [HideInInspector] public bool IsReady;

    [Header("RoomMyCharacterInfo")]
    [SerializeField] private GameObject[] _roomMyInfoObjects;
    [SerializeField] private TMP_Text _roomMyCharacterNameText;
    [SerializeField] private TMP_Text _roomMyCharacterLevelText;
    [SerializeField] private TMP_Text _roomMyCharacterRankText;
    [SerializeField] private TMP_Text _roomMyUserNameText;
    [SerializeField] private TMP_Text _roomMyWinLoseText;
    [SerializeField] private TMP_Text _roomMyRankPointText;
    [SerializeField] private GameObject _roomMyCharacterImageObject;
    [SerializeField] private GameObject[] _roomMyCharacterUpgradeStars;
    [SerializeField] private GameObject _roomMyReadyText;

    [Header("RoomEnemyCharacterInfo")]
    [SerializeField] private GameObject[] _roomEnemyInfoObjects;
    [SerializeField] private TMP_Text _roomEnemyCharacterNameText;
    [SerializeField] private TMP_Text _roomEnemyCharacterLevelText;
    [SerializeField] private TMP_Text _roomEnemyCharacterRankText;
    [SerializeField] private TMP_Text _roomEnemyUserNameText;
    [SerializeField] private TMP_Text _roomEnemyWinLoseText;
    [SerializeField] private TMP_Text _roomEnemyRankPointText;
    [SerializeField] private GameObject _roomEnemyCharacterImageObject;
    [SerializeField] private GameObject[] _roomEnemyCharacterUpgradeStars;
    [SerializeField] private GameObject _roomEnemyReadyText;
    private bool _isNoEnemy;

    [Header("Chat")]
    public TMP_InputField ChatInputText;
    public TMP_Text[] ChatTexts;
    public Button SendChatButton;

    [Header("MultiScene")]
    [SerializeField] private Transform _startposition1;
    [SerializeField] private Transform _startposition2;
    private bool _isMultiFull;
    private StageController _stageController;

    private CharacterData _playerData;
    private CameraController _cameraControler;
    private GameObject _player;
    private GameData _gameData;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _isNoEnemy = true;
        IsReady = false;
        _isMultiFull = false;
        _playerData = GameManager.I.DataManager.PlayerData;
        _player = GameManager.I.PlayerManager.Player;
        _gameData = GameManager.I.DataManager.GameData;

        if (GameManager.I.ScenesManager.CurrentSceneName == "LobbyScene")
        {
            PhotonNetwork.SendRate = 20;
            PhotonNetwork.SerializationRate = 10;
        }
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 60;
            _cameraControler = Camera.main.GetComponent<CameraController>();
            _stageController = GameObject.FindWithTag("StageController").GetComponent<StageController>();

            if (PhotonNetwork.IsMasterClient)
            {
                GameObject playerPrefab = PhotonNetwork.Instantiate("Prefabs/Characters/" + _playerData.Tag, _startposition1.position, Quaternion.identity);
                GameManager.I.PlayerManager.Player = playerPrefab;
            }
            else
            {
                GameObject playerPrefab = PhotonNetwork.Instantiate("Prefabs/Characters/" + _playerData.Tag, _startposition2.position, Quaternion.identity);
                GameManager.I.PlayerManager.Player = playerPrefab;
            }
            _cameraControler.CameraSetting();
            GameManager.I.UIManager.PlayerSetting();
        }
    }

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
    }

    private void Update()
    {
        if(GameManager.I.ScenesManager.CurrentSceneName == "LobbyScene")
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && _isNoEnemy)
                {
                    _isNoEnemy = false;
                    NoEnemyInRoom();
                }
                else if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && !_isNoEnemy)
                {
                    _isNoEnemy = true;
                }
            }
        }
        else if(GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                _isMultiFull = true;
            }
            else if(PhotonNetwork.CurrentRoom.PlayerCount == 1 && _isMultiFull && !_player.GetComponent<PlayerCharacter>().IsFinish)
            {
                _isMultiFull = false;
                _stageController.GameClear();
            }
        }
    }

    public void MultiSceneTestButton()
    {
        GameManager.I.DataManager.DataSave();
        PhotonNetwork.LoadLevel("MultiBattleScene1");
    }

    #region Common
    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()  // 콜백 : 성공적으로 실행되었다면, 함수가 실행
    {
        Debug.Log("서버접속완료");
        PhotonNetwork.LocalPlayer.NickName = GameManager.I.DataManager.GameData.UserName;

        if(GameManager.I.ScenesManager.CurrentSceneName == "LobbyScene")
        {
            MyDataSettingInRoom();
            JoinRandomOrCreateRoom();
        }
        else if (GameManager.I.ScenesManager.CurrentSceneName == "MultiBattleScene1")
        {
            
        }
        //else if (GameManager.I.ScenesManager.CurrentSceneName == "BattleScene1")
        //{
        //    CreateRoom();
        //}
    }

    public void DisConnect()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("서버연결끊김");
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("로비접속완료");
    }

    public void CreateRoom()
    {
        string roomName = "Room";
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 });
    }

    public void JoinRoom()
    {
        string roomName = "Room";
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinOrCreateRoom()
    {
        string roomName = "";
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2 }, null);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRandomOrCreateRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom(expectedMaxPlayers : 2, roomOptions : new RoomOptions() { MaxPlayers = 2 });
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방만들기완료");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방참가완료");

        if (GameManager.I.ScenesManager.CurrentSceneName == "LobbyScene")
        {
            PhotonNetwork.Instantiate("PUN2/Room/RoomController", transform.position, Quaternion.identity);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방만들기실패");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방참가실패");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방랜덤참가실패");
    }

    [ContextMenu("Multi Info")]
    public void Info()
    {
        if(PhotonNetwork.InRoom) // 방에 있다면
        {
            Debug.Log("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            Debug.Log("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            Debug.Log("현재 방 최대 인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);
            Debug.Log("현재 방의 마스터 클라이언트인지? : " + PhotonNetwork.IsMasterClient);

            string playerStr = "방에 있는 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            }
            Debug.Log(playerStr);
        }
        else
        {
            Debug.Log("접속한 인원수 : " + PhotonNetwork.CountOfPlayers);
            Debug.Log("방 개수 : " + PhotonNetwork.CountOfRooms);
            Debug.Log("모든 방에 있는 인원수 : " + PhotonNetwork.CountOfPlayersInRooms);
            Debug.Log("로비에 있는지? : " + PhotonNetwork.InLobby);
            Debug.Log("연결됐는지? : " + PhotonNetwork.IsConnected);
        }
    }
    #endregion

    #region Room
    public void MultiPlayActive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        GameManager.I.SoundManager.StartBGM("MultiReady");

        for (int i = 0; i < _roomMyInfoObjects.Length; i++)
        {
            _roomMyInfoObjects[i].SetActive(false);
        }

        for (int i = 0; i < _roomEnemyInfoObjects.Length; i++)
        {
            _roomEnemyInfoObjects[i].SetActive(false);
        }

        for (int i = 0; i < ChatTexts.Length; i++)
        {
            ChatTexts[i].text = "";
        }

        IsReady = false;
        _roomMyReadyText.SetActive(false);
        _roomEnemyReadyText.SetActive(false);

        _multiPlayPanel.SetActive(true);
        Connect();
    }

    public void MultiPlayInactive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        GameManager.I.SoundManager.StartBGM("LobbyScene");
        _multiPlayPanel.SetActive(false);
        DisConnect();
    }

    private void MyDataSettingInRoom()
    {
        _roomMyCharacterNameText.text = GameManager.I.DataManager.PlayerData.KoreaTag;
        _roomMyCharacterLevelText.text = "Lv " + GameManager.I.DataManager.PlayerData.Level.ToString();
        _roomMyCharacterRankText.text = GameManager.I.DataManager.PlayerData.CharacterRank.ToString();
        _roomMyUserNameText.text = PhotonNetwork.LocalPlayer.NickName;
        _roomMyRankPointText.text = GameManager.I.DataManager.GameData.RankPoint.ToString();

        int percent = (_gameData.Win + _gameData.Lose == 0) ? 0 : (int)((float)_gameData.Win / (_gameData.Win + _gameData.Lose) * 100);
        _roomMyWinLoseText.text = _gameData.Win + " 승 " + _gameData.Lose + " 패 (승률 : " + percent + "%)";

        MultiPlayMyImageSetting();
        ActiveMyStar(GameManager.I.DataManager.PlayerData.Star);

        for (int i = 0; i < _roomEnemyInfoObjects.Length; i++)
        {
            _roomMyInfoObjects[i].SetActive(true);
        }
    }

    public void EnemyDataSettingInRoom(string characterName, int level, string rank, string userName, int rankPoint, int win, int lose, int star, string tag)
    {
        _roomEnemyCharacterNameText.text = characterName;
        _roomEnemyCharacterLevelText.text = "Lv " + level.ToString();
        _roomEnemyCharacterRankText.text = rank;
        _roomEnemyUserNameText.text = userName;
        _roomEnemyRankPointText.text = rankPoint.ToString();

        int percent = (win + lose == 0) ? 0 : (int)((float)win / (win + lose) * 100);
        _roomEnemyWinLoseText.text =  win + " 승 " + lose + " 패 (승률 : " + percent + "%)";
        
        MultiPlayEnemyImageSetting(tag);
        ActiveEnemyStar(star);

        for (int i = 0; i < _roomEnemyInfoObjects.Length; i++)
        {
            _roomEnemyInfoObjects[i].SetActive(true);
        }
    }

    private void MultiPlayMyImageSetting()
    {
        int count = _roomMyCharacterImageObject.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            _roomMyCharacterImageObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        _roomMyCharacterImageObject.transform.Find(GameManager.I.DataManager.PlayerData.Tag).gameObject.SetActive(true);
        _roomMyCharacterImageObject.transform.Find("LenderCamera").gameObject.SetActive(true);
    }

    private void MultiPlayEnemyImageSetting(string tag)
    {
        int count = _roomEnemyCharacterImageObject.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            _roomEnemyCharacterImageObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        _roomEnemyCharacterImageObject.transform.Find(tag).gameObject.SetActive(true);
        _roomEnemyCharacterImageObject.transform.Find("LenderCamera").gameObject.SetActive(true);
    }

    private void ActiveMyStar(int starNum)
    {
        for (int i = 0; i < 5; i++)
        {
            _roomMyCharacterUpgradeStars[i].SetActive(false);
        }

        if (starNum == 0) return;
        else _roomMyCharacterUpgradeStars[starNum - 1].SetActive(true);
    }

    private void ActiveEnemyStar(int starNum)
    {
        for (int i = 0; i < 5; i++)
        {
            _roomEnemyCharacterUpgradeStars[i].SetActive(false);
        }

        if (starNum == 0) return;
        else _roomEnemyCharacterUpgradeStars[starNum - 1].SetActive(true);
    }

    private void NoEnemyInRoom()
    {
        for (int i = 0; i < _roomEnemyInfoObjects.Length; i++)
        {
            _roomEnemyInfoObjects[i].SetActive(false);
        }

        _roomEnemyReadyText.SetActive(false);
    }

    public void ReadyButton()
    {
        if (PhotonNetwork.InRoom)
        {
            if (IsReady)
            {
                GameManager.I.SoundManager.StartSFX("ButtonClick");
                IsReady = false;
                _roomMyReadyText.SetActive(false);
            }
            else
            {
                GameManager.I.SoundManager.StartSFX("Ready");
                IsReady = true;
                _roomMyReadyText.SetActive(true);
            }
        }
        else GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
    }
    
    public void EnemyReadyActive(bool bo)
    {
        if (bo)
        {
            GameManager.I.SoundManager.StartSFX("Ready");
            _roomEnemyReadyText.SetActive(true);
        }
        else
        {
            _roomEnemyReadyText.SetActive(false);
        }
    }
    #endregion

}
