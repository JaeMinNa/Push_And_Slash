using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string LoginID;
    public string UserName;
    public int Stage;
    public int Coin;
    public int RankPoint;
    public int Win;
    public int Lose;

    public string Country;

    [Header("Settings")]
    public float SFXValume;
    public float BGMValume;
    public int CameraSettingValue;

    public void Reset()
    {
        UserName = "UserName";
        RankPoint = 0;
        Win = 0;
        Lose = 0;
    }
}

[System.Serializable]
public class DataWrapper
{
    public List<CharacterData> CharacterInventory;
    public CharacterData[] CharacterDatas;
    public EnemyData[] EnemyDatas;
}

[System.Serializable]
public class CharacterData
{
    public enum Rank
    {
        B,
        A,
        S,
        SS,
    }

    [Header("Common Stats")]
    public string Tag;
    public string KoreaTag;
    public bool IsEquip;
    public Rank CharacterRank;
    public int Star;
    public int CurrentStarExp;
    public int MaxStarExp;
    public int Level;
    public int CurrentExp;
    public int MaxExp;
    public float OriginSpeed;
    public float OriginDashImpulse;
    public float OriginDashCoolTime;
    public float OriginAtk;
    public float OriginDef;
    public float OriginSkillAtk;
    public float OriginSkillCoolTime;
    public float Speed;
    public float DashImpulse;
    public float DashCoolTime;
    public float Atk;
    public float Def;
    public float SkillAtk;
    public float SkillCoolTime;
}

[System.Serializable]
public struct EnemyData
{
    [Header("Common Stats")]
    public string Tag;
    public int Exp;
    public int Coin;
    public float Speed;
    public float Atk;
    public float AttackCoolTime;
    public float Def;
}

public class DataManager : MonoBehaviour
{
    public CharacterData PlayerData;
    public GameData GameData;
    public DataWrapper DataWrapper;

    public void Init()
    {
        DataLoad();
    }

    public void Release()
    {

    }

    [ContextMenu("Save Data")] // 컴포넌트 메뉴에 아래 함수를 호출하는 Save Data 라는 명령어가 생성됨
    public void DataSave()
    {
        ES3.Save("GameData", GameData); // Key값 설정, 선언한 class 변수명
        ES3.Save("PlayerData", PlayerData);
        ES3.Save("DataWrapper", DataWrapper);

        //ES3AutoSaveMgr.Current.Save();
    }

    [ContextMenu("Load Data")]
    public void DataLoad()
    {
        if(ES3.FileExists("SaveFile.txt"))
        {
            ES3.LoadInto("GameData", GameData); // 저장된 Key 값, 불러올 class 변수명
            ES3.LoadInto("PlayerData", PlayerData);
            ES3.LoadInto("DataWrapper", DataWrapper);

            //ES3AutoSaveMgr.Current.Load();
        }
        else
        {
            DataSave();
        }
    }

    public void DataDelete()
    {
        // 서버 데이터 삭제
        GameData.UserName = "";
        GameData.RankPoint = 0;
        GameData.Win = 0;
        GameData.Lose = 0;
        GameManager.I.BackendManager.Save();

        // 저장 파일 삭제
        ES3.DeleteFile("SaveFile.txt");

        // PlayerPrefs 초기화
        PlayerPrefs.DeleteAll();

        // Inventory 삭제
        DataWrapper.CharacterInventory.Clear();
    }
}
