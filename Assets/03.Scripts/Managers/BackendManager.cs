using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd; // �ڳ� SDK namespace �߰�
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
        BackendReturnObject bro = Backend.Initialize(true); // �ڳ� �ʱ�ȭ

        // �ڳ� �ʱ�ȭ�� ���� ���䰪
        if (bro.IsSuccess())
        {
            Debug.Log("�ڳ� ���� ���� ���� : " + bro); // ������ ��� statusCode 204 Success
        }
        else
        {
            Debug.LogError("�ڳ� ���� ���� ���� : " + bro); // ������ ��� statusCode 400�� ���� �߻�
        }
    }

    public void SignUp()
    {
        BackendReturnObject bro = Backend.BMember.CustomSignUp(GameManager.I.DataManager.GameData.LoginID, "1234");
        if (bro.IsSuccess())
        {
            Debug.Log("ȸ�����Կ� �����߽��ϴ�.");
            InsertData();
            Login();
        }
        else
        {
            Debug.Log("ȸ�����Կ� �����߽��ϴ�.");
        }
    }

    public void Login()
    {
        BackendReturnObject bro = Backend.BMember.CustomLogin(GameManager.I.DataManager.GameData.LoginID, "1234");
        if (bro.IsSuccess())
        {
            Debug.Log("�α��ο� �����߽��ϴ�.");

            if (GameManager.I.ScenesManager.CurrentSceneName == "StartScene")
            {
                GameManager.I.DataManager.DataSave();
                GameManager.I.ScenesManager.LoadLoadingScene("LobbyScene");
            }
        }
        else
        {
            Debug.Log("�α��ο� �����߽��ϴ�. ȸ�������� �����մϴ�.");
            SignUp();
        }
    }

    public void AutoLogin()
    {
        BackendReturnObject bro = Backend.BMember.LoginWithTheBackendToken();
        if (bro.IsSuccess())
        {
            Debug.Log("�ڵ� �α��ο� �����߽��ϴ�.");
        }
        else
        {
            Debug.Log("�α��ο� �����߽��ϴ�.");
        }
    }

    // ȸ�� ������ �� ��, ������ ���̺� �߰��ϴ� �Լ�
    public void InsertData()
    {
        Param param = GetUserDataParam();
        BackendReturnObject bro = Backend.GameData.Insert("USER_DATA", param); // ������ ���̺��� �̸�

        if (bro.IsSuccess())
        {
            Debug.Log("������ �߰��� �����߽��ϴ�.");
        }
        else
        {
            Debug.Log("������ �߰��� �����߽��ϴ�.");
        }
    }

    // param : �����͸� �ۼ����� �� ����ϴ� class
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
            Debug.LogError("���� ������ ������ ������ϴ�.");
            return;
        }

        Param param = GetUserDataParam();
        BackendReturnObject bro = Backend.GameData.Update("USER_DATA", new Where(), param);

        if (bro.IsSuccess())
        {
            Debug.Log("������ ���� �����߽��ϴ�.");

            // �г��� ����
            bro = Backend.BMember.UpdateNickname(GameManager.I.DataManager.GameData.UserName);
        }
        else
        {
            Debug.Log("������ ���� �����߽��ϴ�.");
        }
    }

    // �����κ��� �����͸� �ҷ��ͼ� Parsing�ϴ� �Լ�
    public void Load()
    {
        if (!Backend.IsInitialized)
        {
            Debug.LogError("���� ������ ������ ������ϴ�.");
            return;
        }

        BackendReturnObject bro = Backend.GameData.GetMyData("USER_DATA", new Where());
        
        if (bro.IsSuccess())
        {
            Debug.Log("������ �ε� �����߽��ϴ�.");
            ParsingData(bro.GetReturnValuetoJSON()["rows"][0]);
            // �������� �ҷ��� Json �����͸� �Ľ�
            // Json ������ ��, rows�� ���� ������
        }
        else
        {
            Debug.Log("������ �ε� �����߽��ϴ�.");
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