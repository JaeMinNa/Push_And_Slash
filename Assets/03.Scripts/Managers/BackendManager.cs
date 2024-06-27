using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd; // 뒤끝 SDK namespace 추가
using UnityEngine.Events;
using LitJson;

public class BackendManager : MonoBehaviour
{
    public void Init()
    {
        BackendSetup();

        if (GameManager.I.ScenesManager.CurrentSceneName != "StartScene")
        {
            Login();
        }
    }

    public void Release()
    {

    }

    private void BackendSetup()
    {
        BackendReturnObject bro = Backend.Initialize(true); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            Debug.Log("뒤끝 서버 연동 성공 : " + bro); // 성공일 경우 statusCode 204 Success
        }
        else
        {
            Debug.LogError("뒤끝 서버 연동 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        }
    }

    public void SignUp()
    {
        BackendReturnObject bro = Backend.BMember.CustomSignUp(GameManager.I.DataManager.GameData.LoginID, "1234");
        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니다.");
            InsertData();
            Login();
        }
        else
        {
            Debug.Log("회원가입에 실패했습니다.");
        }
    }

    public void Login()
    {
        BackendReturnObject bro = Backend.BMember.CustomLogin(GameManager.I.DataManager.GameData.LoginID, "1234");
        if (bro.IsSuccess())
        {
            Debug.Log("로그인에 성공했습니다.");

            if (GameManager.I.ScenesManager.CurrentSceneName == "StartScene")
            {
                GameManager.I.DataManager.DataSave();
                GameManager.I.ScenesManager.LoadLoadingScene("LobbyScene");
            }
        }
        else
        {
            Debug.Log("로그인에 실패했습니다. 회원가입을 진행합니다.");
            SignUp();
        }
    }

    public void AutoLogin()
    {
        BackendReturnObject bro = Backend.BMember.LoginWithTheBackendToken();
        if (bro.IsSuccess())
        {
            Debug.Log("자동 로그인에 성공했습니다.");
        }
        else
        {
            Debug.Log("로그인에 실패했습니다.");
        }
    }

    // 회원 가입을 할 때, 데이터 테이블에 추가하는 함수
    public void InsertData()
    {
        Param param = GetUserDataParam();
        BackendReturnObject bro = Backend.GameData.Insert("USER_DATA", param); // 데이터 테이블의 이름

        if (bro.IsSuccess())
        {
            Debug.Log("데이터 추가를 성공했습니다.");
        }
        else
        {
            Debug.Log("데이터 추가를 실패했습니다.");
        }
    }

    // param : 데이터를 송수신할 때 사용하는 class
    private Param GetUserDataParam()
    {
        Param param = new Param();
        param.Add("UserName", GameManager.I.DataManager.GameData.UserName);
        param.Add("RankPoint", GameManager.I.DataManager.GameData.RankPoint);
        param.Add("WinLose", GameManager.I.DataManager.GameData.Win.ToString() + "|" + GameManager.I.DataManager.GameData.Lose.ToString());

        return param;
    }

    public void Save()
    {
        if (!Backend.IsInitialized)
        {
            Debug.LogError("현재 서버와 연결이 끊겼습니다.");
            return;
        }

        Param param = GetUserDataParam();
        BackendReturnObject bro = Backend.GameData.Update("USER_DATA", new Where(), param);

        if (bro.IsSuccess())
        {
            Debug.Log("데이터 저장 성공했습니다.");

            // 닉네임 설정
            bro = Backend.BMember.UpdateNickname(GameManager.I.DataManager.GameData.UserName);
        }
        else
        {
            Debug.Log("데이터 저장 실패했습니다.");
        }
    }

    // 서버로부터 데이터를 불러와서 Parsing하는 함수
    public void Load()
    {
        if (!Backend.IsInitialized)
        {
            Debug.LogError("현재 서버와 연결이 끊겼습니다.");
            return;
        }

        BackendReturnObject bro = Backend.GameData.GetMyData("USER_DATA", new Where());
        
        if (bro.IsSuccess())
        {
            Debug.Log("데이터 로드 성공했습니다.");
            ParsingData(bro.GetReturnValuetoJSON()["rows"][0]);
            // 서버에서 불러온 Json 데이터를 파싱
            // Json 데이터 중, rows의 값만 가져옴
        }
        else
        {
            Debug.Log("데이터 로드 실패했습니다.");
        }
    }

    private void ParsingData(JsonData json)
    {
        GameManager.I.DataManager.GameData.UserName = json["UserName"][0].ToString();
        GameManager.I.DataManager.GameData.RankPoint = int.Parse(json["RankPoint"][0].ToString());

        string[] extraData = json["extraData"].ToString().Split("|");
        GameManager.I.DataManager.GameData.Win = int.Parse(extraData[0].ToString());
        GameManager.I.DataManager.GameData.Lose = int.Parse(extraData[1].ToString());
    }

    public bool IsConnect()
    {
        if (Backend.IsInitialized) return true;

        return false;
    }
}