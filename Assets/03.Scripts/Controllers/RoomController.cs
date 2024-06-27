using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class RoomController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private bool _myIsReady;
    [SerializeField] private bool _enemyIsReady;
    private PhotonView _photonView;
    private NetworkManager _networkManager;
    private bool _isJoin;
    private bool _isGameStart;
    private bool _isReady;
    private string _roomEnemyCharacterName;
    private int _roomEnemyCharacterLevel;
    private string _roomEnemyCharacterRank;
    private string _roomEnemyUserName;
    private int _roomEnemyRankPoint;
    private int _roomEnemyWin;
    private int _roomEnemyLose;
    private int _roomEnemyCharacterStar;
    private string _roomEnemyCharacterKorTag;
    private TMP_Text[] _chatTexts;
    private Button _sendChatButton;


    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.AutomaticallySyncScene = true;

        _networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
        _photonView = GetComponent<PhotonView>();
        _chatTexts = _networkManager.ChatTexts;
        _sendChatButton = _networkManager.SendChatButton;
    }

    private void Start()
    {
        _isJoin = false;
        _isGameStart = false;
        _isReady = false;
        _myIsReady = false;
        _enemyIsReady = false;
        _sendChatButton.onClick.AddListener(SendChat);

        if (_photonView.IsMine)
            _photonView.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + PhotonNetwork.NickName + "¥‘¿Ã ¬¸∞°«œºÃΩ¿¥œ¥Ÿ.</color>");

        StartCoroutine(COUpdate());
    }

    private void Update()
    {
        _myIsReady = _networkManager.IsReady;

        if (!_photonView.IsMine)
        {
            if (_enemyIsReady && !_isReady)
            {
                _isReady = true;
                _networkManager.EnemyReadyActive(true);
            }
            else if(!_enemyIsReady && _isReady)
            {
                _isReady = false;
                _networkManager.EnemyReadyActive(false);
            }
        }

        if(PhotonNetwork.IsMasterClient)
        {
            if (_myIsReady && _enemyIsReady && !_isGameStart)
            {
                _isGameStart = true;
                GameManager.I.DataManager.DataSave();
                PhotonNetwork.LoadLevel("MultiBattleScene1");
            }
        }
    }

    #region Join
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_photonView.IsMine)
            _photonView.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "¥‘¿Ã ≈¿Â«œºÃΩ¿¥œ¥Ÿ.</color>");
    }

    IEnumerator COUpdate()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            if (!_photonView.IsMine)
            {
                if (PhotonNetwork.InRoom && !_isJoin)
                {
                    _isJoin = true;

                    if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                    {
                        _networkManager.EnemyDataSettingInRoom(_roomEnemyCharacterName, _roomEnemyCharacterLevel, _roomEnemyCharacterRank.ToString(),
                            _roomEnemyUserName, _roomEnemyRankPoint, _roomEnemyWin, _roomEnemyLose, _roomEnemyCharacterStar, _roomEnemyCharacterKorTag);
                    }             
                }
            }

            yield return null;
        }
    }
    #endregion

    #region Chat
    private void SendChat()
    {
        if (_photonView.IsMine)
        {
            GameManager.I.SoundManager.StartSFX("ButtonClick");

            string chat = PhotonNetwork.NickName + " : " + _networkManager.ChatInputText.text;
            _photonView.RPC("ChatRPC", RpcTarget.All, chat);
            _networkManager.ChatInputText.text = "";
        }
    }

    [PunRPC]
    public void ChatRPC(string str)
    {
        bool isInput = false;

        for (int i = 0; i < _chatTexts.Length; i++)
        {
            if (_chatTexts[i].text == "")
            {
                isInput = true;
                _chatTexts[i].text = str;
                break;
            }
        }

        if (!isInput)
        {
            for (int i = 1; i < _chatTexts.Length; i++)
            {
                _chatTexts[i - 1].text = _chatTexts[i].text;
            }

            _chatTexts[_chatTexts.Length - 1].text = str;
        }
    }
    #endregion

    #region Data
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(GameManager.I.DataManager.PlayerData.KoreaTag);
            stream.SendNext(GameManager.I.DataManager.PlayerData.Level);
            stream.SendNext(GameManager.I.DataManager.PlayerData.CharacterRank.ToString());
            stream.SendNext(GameManager.I.DataManager.GameData.UserName);
            stream.SendNext(GameManager.I.DataManager.GameData.RankPoint);
            stream.SendNext(GameManager.I.DataManager.GameData.Win);
            stream.SendNext(GameManager.I.DataManager.GameData.Lose);
            stream.SendNext(GameManager.I.DataManager.PlayerData.Star);
            stream.SendNext(GameManager.I.DataManager.PlayerData.Tag);
            stream.SendNext(_myIsReady);
        }
        else
        {
            _roomEnemyCharacterName = (string)stream.ReceiveNext();
            _roomEnemyCharacterLevel = (int)stream.ReceiveNext();
            _roomEnemyCharacterRank = (string)stream.ReceiveNext();
            _roomEnemyUserName = (string)stream.ReceiveNext();
            _roomEnemyRankPoint = (int)stream.ReceiveNext();
            _roomEnemyWin = (int)stream.ReceiveNext();
            _roomEnemyLose = (int)stream.ReceiveNext();
            _roomEnemyCharacterStar = (int)stream.ReceiveNext();
            _roomEnemyCharacterKorTag = (string)stream.ReceiveNext();
            _enemyIsReady = (bool)stream.ReceiveNext();
        }
    }
    #endregion
}
