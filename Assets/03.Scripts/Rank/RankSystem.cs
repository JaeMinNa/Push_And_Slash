using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using TMPro;

public class RankSystem : MonoBehaviour
{
    private const string RANK_UUID = "731769e0-303c-11ef-8d93-2749de6cc326";
    private const int MAX_RANK_LIST = 10;

    [Header("My Rank")]
    [SerializeField] private GameObject _myRankMedals;
    [SerializeField] private TMP_Text _myRankText;
    [SerializeField] private TMP_Text _myUserNameText;
    [SerializeField] private TMP_Text _myRankPointText;
    [SerializeField] private TMP_Text _myWinLoseText;
    [SerializeField] private GameObject _myFlag;
    private GameData _gameData;

    [Header("All Rank")]
    [SerializeField] private GameObject _rankList;
    private GameObject _rankMedals;
    private TMP_Text _rankText;
    private TMP_Text _userNameText;
    private TMP_Text _rankPointText;
    private TMP_Text _winLoseText;
    private GameObject _flag;


    private int _rankPoint;
    private int _rank;
    private int _win;
    private int _lose;
    private string _userName;

    private void Start()
    {
        _gameData = GameManager.I.DataManager.GameData;
    }

    public void UpdateRank(int value)
    {
        UpdateMyRankData(value);
    }

    private void UpdateMyRankData(int value)
    {
        string rowInDate = string.Empty;

        // ��ŷ �����͸� ������Ʈ�Ϸ��� ���� �����Ϳ��� ����ϴ� �������� inDate �� �ʿ�
        BackendReturnObject bro = Backend.GameData.GetMyData("USER_DATA", new Where());
        
        if (!bro.IsSuccess())
        {
            Debug.LogError("��ŷ ������Ʈ�� ���� ������ ��ȸ �� ������ �߻��߽��ϴ�.");
            return;
        }

        Debug.Log("��ŷ ������Ʈ�� ���� ������ ��ȸ�� �����߽��ϴ�.");

        if(bro.FlattenRows().Count > 0)
        {
            rowInDate = bro.FlattenRows()[0]["inDate"].ToString();
        }
        else
        {
            Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�.");
        }

        Param param = new Param()
        {
            {"RankPoint",  value},
        };

        // �ش� ���������̺��� �����͸� �����ϰ�, ��ŷ ������ ���� ����
        bro = Backend.URank.User.UpdateUserScore(RANK_UUID, "USER_DATA", rowInDate, param);
        
        if(bro.IsSuccess())
        {
            Debug.Log("��ŷ ��Ͽ� �����߽��ϴ�.");
        }
        else
        {
            Debug.LogError("��ŷ ��Ͽ� �����߽��ϴ�." + bro);
        }

        //if(bro.IsServerError())
        //{
        //    Debug.LogError("������ ����ȭ�����̰ų� �Ҿ����� ��� �߻��մϴ�.");
        //}
    }

    public void GetRankList()
    {
        // ��ŷ ���̺� �ִ� ������ offset ~ offset + limit ���� ��ŷ ������ �ҷ���
        BackendReturnObject bro = Backend.URank.User.GetRankList(RANK_UUID, MAX_RANK_LIST, 0);

        if(bro.IsSuccess())
        {
            // JSON ������ �Ľ� ����
            try
            {
                Debug.Log("��ŷ ��ȸ�� �����߽��ϴ�.");
                JsonData rankDataJson = bro.FlattenRows();

                // �޾ƿ� �������� ������ 0 -> �����Ͱ� ����
                if(rankDataJson.Count <= 0)
                {
                    Debug.Log("��ŷ �����Ͱ� �������� �ʽ��ϴ�.");

                    for (int i = 0; i < MAX_RANK_LIST; i++)
                    {
                        _rankList.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else
                {
                    int rankCount = rankDataJson.Count;

                    // �޾ƿ� rank �������� ���ڸ�ŭ ������ ���
                    for (int i = 0; i < rankCount; i++)
                    {
                        _rankPoint = int.Parse(rankDataJson[i]["score"].ToString());
                        _rank = int.Parse(rankDataJson[i]["rank"].ToString());
                        _userName = rankDataJson[i]["nickname"].ToString();
                        string[] extraData = rankDataJson[i]["WinLose"].ToString().Split("|");
                        _win = int.Parse(extraData[0].ToString());
                        _lose = int.Parse(extraData[1].ToString());
                        
                        if(i >= 0 && i <= 2)
                        {
                            _rankMedals = _rankList.transform.GetChild(i).GetChild(6).gameObject;
                            MedalActive(_rank);
                        }

                        _rankText = _rankList.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                        _userNameText = _rankList.transform.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>();
                        _rankPointText = _rankList.transform.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>();
                        _winLoseText = _rankList.transform.GetChild(i).GetChild(5).GetComponent<TextMeshProUGUI>();
                        _rankText.text = _rank.ToString();
                        _userNameText.text = _userName;
                        _rankPointText.text = _rankPoint.ToString();

                        int percent = (_win + _lose == 0) ? 0 : (int)((float)_win / (_win + _lose) * 100);
                        _winLoseText.text = _win + " �� " + _lose + " �� (�·� : " + percent + "%)";

                        _rankList.transform.GetChild(i).gameObject.SetActive(true);

                    }
                    // rankCount�� Max����ŭ �������� ���� ��, ������ ��ŷ
                    for (int i = rankCount; i < MAX_RANK_LIST; i++)
                    {
                        _rankList.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            // JSON ������ �Ľ� ����
            catch (System.Exception e)
            {
                Debug.LogError(e);

                for (int i = 0; i < MAX_RANK_LIST; i++)
                {
                    _rankList.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogError("��ŷ ��ȸ�� �����߽��ϴ�.");

            for (int i = 0; i < MAX_RANK_LIST; i++)
            {
                _rankList.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void GetMyRank()
    {    
        // �� ��ŷ ���� �ҷ�����
        BackendReturnObject bro = Backend.URank.User.GetMyRank(RANK_UUID);

        if(bro.IsSuccess())
        {
            try
            {
                JsonData rankDataJson = bro.FlattenRows();

                // �޾ƿ� �������� ������ 0 -> �����Ͱ� ����
                if (rankDataJson.Count <= 0)
                {
                    Debug.Log("���� ��ŷ �����Ͱ� �������� �ʽ��ϴ�.");

                    MedalActive(0);
                    _myFlag.SetActive(false);
                    _myRankText.text = "-";
                    _myRankPointText.text = "-";
                    _myUserNameText.text = "-";
                    _myWinLoseText.text = "-";

                }
                else
                {
                    _rankPoint = int.Parse(rankDataJson[0]["score"].ToString());
                    _rank = int.Parse(rankDataJson[0]["rank"].ToString());

                    if(_rank >= 1 && _rank <= 3) MyMedalActive(_rank);
                    else MyMedalActive(0);

                    _myFlag.SetActive(true);
                    _myRankText.text = _rank.ToString();
                    _myRankPointText.text = _rankPoint.ToString();
                    _myUserNameText.text = GameManager.I.DataManager.GameData.UserName;

                    int percent = (_gameData.Win + _gameData.Lose == 0) ? 0 : (int)((float)_gameData.Win / (_gameData.Win + _gameData.Lose) * 100);
                    _myWinLoseText.text = _gameData.Win + " �� " + _gameData.Lose + " �� (�·� : " + percent + "%)";
                }
            }
            // ���� ��ŷ ���� JSON ������ �Ľ̿� �������� ��
            catch (System.Exception e)
            {
                Debug.LogError(e);

                _myFlag.SetActive(false);
                _myRankText.text = "-";
                _myRankPointText.text = "-";
                _myUserNameText.text = "-";
                _myWinLoseText.text = "-";
            }
        }
        else
        {
            // ���� ��ŷ ������ �ҷ����µ� �������� ��

            _myFlag.SetActive(false);
            _myRankText.text = "-";
            _myRankPointText.text = "-";
            _myUserNameText.text = "-";
            _myWinLoseText.text = "-";
        }
    }

    private void MedalActive(int rank)
    {
        for (int i = 0; i < 3; i++)
        {
            _rankMedals.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (rank == 0) return;

        _rankMedals.transform.GetChild(rank - 1).gameObject.SetActive(true);
    }

    private void MyMedalActive(int rank)
    {
        for (int i = 0; i < 3; i++)
        {
            _myRankMedals.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (rank == 0) return;

        _myRankMedals.transform.GetChild(rank - 1).gameObject.SetActive(true);
    }
}
