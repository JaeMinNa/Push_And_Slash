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

        // 랭킹 데이터를 업데이트하려면 게임 데이터에서 사용하는 데이터의 inDate 값 필요
        BackendReturnObject bro = Backend.GameData.GetMyData("USER_DATA", new Where());
        
        if (!bro.IsSuccess())
        {
            Debug.LogError("랭킹 업데이트를 위한 데이터 조회 중 문제가 발생했습니다.");
            return;
        }

        Debug.Log("랭킹 업데이트를 위한 데이터 조회에 성공했습니다.");

        if(bro.FlattenRows().Count > 0)
        {
            rowInDate = bro.FlattenRows()[0]["inDate"].ToString();
        }
        else
        {
            Debug.LogError("데이터가 존재하지 않습니다.");
        }

        Param param = new Param()
        {
            {"RankPoint",  value},
        };

        // 해당 데이터테이블의 데이터를 갱신하고, 랭킹 데이터 정보 갱신
        bro = Backend.URank.User.UpdateUserScore(RANK_UUID, "USER_DATA", rowInDate, param);
        
        if(bro.IsSuccess())
        {
            Debug.Log("랭킹 등록에 성공했습니다.");
        }
        else
        {
            Debug.LogError("랭킹 등록에 실패했습니다." + bro);
        }

        //if(bro.IsServerError())
        //{
        //    Debug.LogError("서버가 과부화상태이거나 불안정할 경우 발생합니다.");
        //}
    }

    public void GetRankList()
    {
        // 랭킹 테이블에 있는 유저의 offset ~ offset + limit 순위 랭킹 정보를 불러옴
        BackendReturnObject bro = Backend.URank.User.GetRankList(RANK_UUID, MAX_RANK_LIST, 0);

        if(bro.IsSuccess())
        {
            // JSON 데이터 파싱 성공
            try
            {
                Debug.Log("랭킹 조회에 성공했습니다.");
                JsonData rankDataJson = bro.FlattenRows();

                // 받아온 데이터의 개수가 0 -> 데이터가 없음
                if(rankDataJson.Count <= 0)
                {
                    Debug.Log("랭킹 데이터가 존재하지 않습니다.");

                    for (int i = 0; i < MAX_RANK_LIST; i++)
                    {
                        _rankList.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else
                {
                    int rankCount = rankDataJson.Count;

                    // 받아온 rank 데이터의 숫자만큼 데이터 출력
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
                        _winLoseText.text = _win + " 승 " + _lose + " 패 (승률 : " + percent + "%)";

                        _rankList.transform.GetChild(i).gameObject.SetActive(true);

                    }
                    // rankCount가 Max값만큼 존재하지 않을 때, 나머지 랭킹
                    for (int i = rankCount; i < MAX_RANK_LIST; i++)
                    {
                        _rankList.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            // JSON 데이터 파싱 실패
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
            Debug.LogError("랭킹 조회에 실패했습니다.");

            for (int i = 0; i < MAX_RANK_LIST; i++)
            {
                _rankList.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void GetMyRank()
    {    
        // 내 랭킹 정보 불러오기
        BackendReturnObject bro = Backend.URank.User.GetMyRank(RANK_UUID);

        if(bro.IsSuccess())
        {
            try
            {
                JsonData rankDataJson = bro.FlattenRows();

                // 받아온 데이터의 개수가 0 -> 데이터가 없음
                if (rankDataJson.Count <= 0)
                {
                    Debug.Log("나의 랭킹 데이터가 존재하지 않습니다.");

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
                    _myWinLoseText.text = _gameData.Win + " 승 " + _gameData.Lose + " 패 (승률 : " + percent + "%)";
                }
            }
            // 나의 랭킹 정보 JSON 데이터 파싱에 실패했을 때
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
            // 나의 랭킹 정보를 불러오는데 실패했을 때

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
