using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private TMP_Text _userNameText;
    [SerializeField] private TMP_Text _selectCharacterNameText;
    [SerializeField] private Slider _expSlider;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _expText;
    [SerializeField] private TMP_Text _stageText_Chapter1;

    [Header("Inventory")]
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject[] _playerUpgradeStars;
    [SerializeField] private TMP_Text _playerTagText;
    [SerializeField] private TMP_Text _playerLevelText;
    [SerializeField] private Slider _playerExpSlider;
    [SerializeField] private TMP_Text _playerExpPercent;
    [SerializeField] private Slider _playerUpgradeSlider;
    [SerializeField] private TMP_Text _playerUpgradeText;
    [SerializeField] private TMP_Text _rankText;
    [SerializeField] private TMP_Text _atkText;
    [SerializeField] private TMP_Text _defText;
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private TMP_Text _skillAtkText;
    [SerializeField] private TMP_Text _skillCoolTimeText;
    [SerializeField] private TMP_Text _dashPowerText;
    [SerializeField] private TMP_Text _dashCoolTimeText;
    [SerializeField] private GameObject _InventoryImageObject;
    [SerializeField] private RawImage _InventoryImage;
    private int _characterNum;
    private List<CharacterData> _inventory;

    [Header("CharacterSelect")]
    [SerializeField] private GameObject _characterSelectPanel;
    [SerializeField] private GameObject _characterSelectOKPanel;
    [SerializeField] private TMP_Text _character1AtkText;
    [SerializeField] private TMP_Text _character1DefText;
    [SerializeField] private TMP_Text _character1SpeedText;
    [SerializeField] private TMP_Text _character2AtkText;
    [SerializeField] private TMP_Text _character2DefText;
    [SerializeField] private TMP_Text _character2SpeedText;
    [SerializeField] private TMP_Text _character3AtkText;
    [SerializeField] private TMP_Text _character3DefText;
    [SerializeField] private TMP_Text _character3SpeedText;
    private int _charactetSelectNum;
    private CharacterData _inventorySelectData;

    [Header("Shop")]
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private TMP_Text _shopCoinText;
    [SerializeField] private GameObject _heroPanel;
    [SerializeField] private TMP_Text _drawCharacterText;
    [SerializeField] private Sprite[] _frameImages;
    [SerializeField] private Image _heroPanelSlotImage;
    [SerializeField] private Image _heroPanelFrameImage;
    [SerializeField] private RawImage _heroPanelSlotRawImage;
    [SerializeField] private TMP_Text _heroPanelRankText;
    [SerializeField] private TMP_Text _heroPanelAtkText;
    [SerializeField] private TMP_Text _heroPanelDefText;
    [SerializeField] private TMP_Text _heroPanelSpeedText;
    [SerializeField] private TMP_Text _heroPanelSkillAtkText;
    [SerializeField] private TMP_Text _heroPanelSkillCoolTimeText;
    [SerializeField] private TMP_Text _heroPanelDashPowerText;
    [SerializeField] private TMP_Text _heroPanelDashCoolTimeText;
    [SerializeField] private GameObject _heroPanelOKButton;
    private CharacterData _drawCharacter;
    private int _drawCount;

    [Header("Rank")]
    [SerializeField] private GameObject _rankPanel;

    private GameData _gameData;
    private CharacterData _playerData;
    private DataWrapper _dataWrapper;
    private RankSystem _rankSystem;

    [SerializeField] private TMP_Text text1;
    [SerializeField] private TMP_Text text2;

    private void Awake()
    {
        _rankSystem = GetComponent<RankSystem>();
    }

    private void Start()
    {
        _gameData = GameManager.I.DataManager.GameData;
        _dataWrapper = GameManager.I.DataManager.DataWrapper;
        _playerData = GameManager.I.DataManager.PlayerData;
        _inventory = _dataWrapper.CharacterInventory;
        _inventorySelectData = _playerData;
        _characterNum = -1;
        _charactetSelectNum = -1;
        _drawCount = 0;

        if (PlayerPrefs.GetInt("Tutorial") == 0) CharacterSelectActive();

        GameManager.I.AdsManager.DestroyAd();
        CoinSetting();
        UserNameSetting();
        StageSetting();
        CharacterSetting();
        CharacterStatSetting();
        EquipSetting();
        GameManager.I.SoundManager.StartBGM("LobbyScene");

        GameManager.I.BackendManager.Save();
        UpdateRank(GameManager.I.DataManager.GameData.RankPoint);
        GameManager.I.DataManager.DataSave();
    }

    private void Update()
    {
        text1.text = "µÚ³¡ ¿¬°á ? : " + GameManager.I.BackendManager.IsConnect();
        text2.text = "GameData.LoginID : " + GameManager.I.DataManager.GameData.LoginID;
    }

    public void ButtonClickMiss()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
    }

    #region Loby
    public void Chapter1Button()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        GameManager.I.DataManager.DataSave();
        GameManager.I.ScenesManager.LoadLoadingScene("BattleScene1");
    }

    public void CoinSetting()
    {
        _coinText.text = GameManager.I.DataManager.GameData.Coin.ToString();
    }

    public void UserNameSetting()
    {
        _userNameText.text = GameManager.I.DataManager.GameData.UserName;
        _selectCharacterNameText.text = GameManager.I.DataManager.PlayerData.KoreaTag.ToString();
    }

    private void StageSetting()
    {
        _stageText_Chapter1.text = "Stage " + _gameData.Stage.ToString();
    }

    private void LevelSetting()
    {
        _levelText.text = "Lv " + GameManager.I.DataManager.PlayerData.Level.ToString();
    }

    private void ExpSetting()
    {
        _expSlider.value = (float)GameManager.I.DataManager.PlayerData.CurrentExp / GameManager.I.DataManager.PlayerData.MaxExp;
        _expText.text = GameManager.I.DataManager.PlayerData.CurrentExp.ToString() + "/" + GameManager.I.DataManager.PlayerData.MaxExp.ToString();
    }

    private void CharacterSetting()
    {
        LevelSetting();
        ExpSetting();
    }

    private void CharacterStatSetting()
    {
        int inventoryCount = _inventory.Count;

        for (int i = 0; i < inventoryCount; i++)
        {
            GameManager.I.DataManager.DataWrapper.CharacterInventory[i].Atk
                = _inventory[i].OriginAtk + ((_inventory[i].Level * 0.1f) - 0.1f) + ((_inventory[i].Star * 0.5f) - 0.5f);
            GameManager.I.DataManager.DataWrapper.CharacterInventory[i].Def
                = _inventory[i].OriginDef + ((_inventory[i].Star * 0.1f) - 0.1f);
            GameManager.I.DataManager.DataWrapper.CharacterInventory[i].Speed
               = _inventory[i].OriginSpeed + ((_inventory[i].Level * 0.1f) - 0.1f) + ((_inventory[i].Star * 0.5f) - 0.5f);
            GameManager.I.DataManager.DataWrapper.CharacterInventory[i].SkillAtk
               = _inventory[i].OriginSkillAtk + ((_inventory[i].Level * 0.1f) - 0.1f) + ((_inventory[i].Star * 0.5f) - 0.5f);
            GameManager.I.DataManager.DataWrapper.CharacterInventory[i].SkillCoolTime
               = _inventory[i].OriginSkillCoolTime - ((_inventory[i].Level * 0.1f) - 0.1f) - ((_inventory[i].Star * 0.5f) - 0.5f);
            GameManager.I.DataManager.DataWrapper.CharacterInventory[i].DashImpulse
               = _inventory[i].OriginDashImpulse + ((_inventory[i].Level * 0.1f) - 0.1f) + ((_inventory[i].Star * 0.5f) - 0.5f);
            GameManager.I.DataManager.DataWrapper.CharacterInventory[i].DashCoolTime
               = _inventory[i].OriginDashCoolTime - ((_inventory[i].Level * 0.1f) - 0.1f) - ((_inventory[i].Star * 0.5f) - 0.5f);
        }
    }

    //private int FindCharacterDataOrder(CharacterData data)
    //{
    //    int count = _dataWrapper.CharacterDatas.Length;

    //    for (int i = 0; i < count; i++)
    //    {
    //        if (data.Tag == _dataWrapper.CharacterDatas[i].Tag) return i;
    //        else continue;
    //    }

    //    return -1;
    //}

    public void RewardAdButton()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        GameManager.I.AdsManager.LoadRewardedAd();
    }
    #endregion

    #region Inventory
    public void InventoryActive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _inventory = _dataWrapper.CharacterInventory;
        InventorySetting();
        CharacterStatSetting();
        _inventoryPanel.SetActive(true);
    }

    public void InventoryInactive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _inventoryPanel.SetActive(false);
    }

    private void InventorySetting()
    {
        ActiveStar(_playerData.Star);

        _playerTagText.text = GameManager.I.DataManager.PlayerData.KoreaTag.ToString();
        _playerLevelText.text = GameManager.I.DataManager.PlayerData.Level.ToString();
        _playerExpSlider.value = (float)GameManager.I.DataManager.PlayerData.CurrentExp / GameManager.I.DataManager.PlayerData.MaxExp;
        _playerExpPercent.text = (((float)GameManager.I.DataManager.PlayerData.CurrentExp / GameManager.I.DataManager.PlayerData.MaxExp) * 100).ToString("N1") + "%";
        _playerUpgradeSlider.value = (float)GameManager.I.DataManager.PlayerData.CurrentStarExp / GameManager.I.DataManager.PlayerData.MaxStarExp;
        _playerUpgradeText.text = GameManager.I.DataManager.PlayerData.CurrentStarExp.ToString() + "/" + GameManager.I.DataManager.PlayerData.MaxStarExp.ToString();
        _rankText.text = GameManager.I.DataManager.PlayerData.CharacterRank.ToString();
        _atkText.text = GameManager.I.DataManager.PlayerData.Atk.ToString();
        _defText.text = GameManager.I.DataManager.PlayerData.Def.ToString();
        _speedText.text = GameManager.I.DataManager.PlayerData.Speed.ToString();
        _skillAtkText.text = GameManager.I.DataManager.PlayerData.SkillAtk.ToString();
        _skillCoolTimeText.text = GameManager.I.DataManager.PlayerData.SkillCoolTime.ToString();
        _dashPowerText.text = GameManager.I.DataManager.PlayerData.DashImpulse.ToString();
        _dashCoolTimeText.text = GameManager.I.DataManager.PlayerData.DashCoolTime.ToString();
        InventoryImageSetting();
    }

    public void InventorySlotButton(int num)
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _inventorySelectData = _dataWrapper.CharacterDatas[num];
        _characterNum = num;
        
        if (!CharacterIsGet(_inventorySelectData))
        {
            ActiveStar(0);

            _playerTagText.text = _inventorySelectData.KoreaTag.ToString();
            _playerLevelText.text = _inventorySelectData.Level.ToString();
            _playerExpSlider.value = (float)_inventorySelectData.CurrentExp / _inventorySelectData.MaxExp;
            _playerExpPercent.text = (((float)_inventorySelectData.CurrentExp / _inventorySelectData.MaxExp) * 100).ToString("N1") + "%";
            _playerUpgradeSlider.value = (CharacterIsGet(_inventorySelectData)) ? (float)_inventorySelectData.CurrentStarExp / _inventorySelectData.MaxStarExp : 0;
            _playerUpgradeText.text = (CharacterIsGet(_inventorySelectData)) ? _inventorySelectData.CurrentStarExp.ToString() + "/" + _inventorySelectData.MaxStarExp.ToString()
                : 0 + "/" + _inventorySelectData.MaxStarExp.ToString();
            _rankText.text = _inventorySelectData.CharacterRank.ToString();
            _atkText.text = _inventorySelectData.Atk.ToString();
            _defText.text = _inventorySelectData.Def.ToString();
            _speedText.text = _inventorySelectData.Speed.ToString();
            _skillAtkText.text = _inventorySelectData.SkillAtk.ToString();
            _skillCoolTimeText.text = _inventorySelectData.SkillCoolTime.ToString();
            _dashPowerText.text = _inventorySelectData.DashImpulse.ToString();
            _dashCoolTimeText.text = _inventorySelectData.DashCoolTime.ToString();
        }
        else
        {
            int inventoryOrder = FindInventoryOrder(_inventorySelectData);
            ActiveStar(_dataWrapper.CharacterInventory[inventoryOrder].Star);

            _playerTagText.text = _dataWrapper.CharacterInventory[inventoryOrder].KoreaTag.ToString();
            _playerLevelText.text = _dataWrapper.CharacterInventory[inventoryOrder].Level.ToString();
            _playerExpSlider.value = (float)_dataWrapper.CharacterInventory[inventoryOrder].CurrentExp / _dataWrapper.CharacterInventory[inventoryOrder].MaxExp;
            _playerExpPercent.text = (((float)_dataWrapper.CharacterInventory[inventoryOrder].CurrentExp / _dataWrapper.CharacterInventory[inventoryOrder].MaxExp) * 100).ToString("N1") + "%";
            _playerUpgradeSlider.value = (float)_dataWrapper.CharacterInventory[inventoryOrder].CurrentStarExp / _dataWrapper.CharacterInventory[inventoryOrder].MaxStarExp;
            _playerUpgradeText.text = _dataWrapper.CharacterInventory[inventoryOrder].CurrentStarExp.ToString() + "/" + _dataWrapper.CharacterInventory[inventoryOrder].MaxStarExp.ToString();  
            _rankText.text = _dataWrapper.CharacterInventory[inventoryOrder].CharacterRank.ToString();
            _atkText.text = _dataWrapper.CharacterInventory[inventoryOrder].Atk.ToString();
            _defText.text = _dataWrapper.CharacterInventory[inventoryOrder].Def.ToString();
            _speedText.text = _dataWrapper.CharacterInventory[inventoryOrder].Speed.ToString();
            _skillAtkText.text = _dataWrapper.CharacterInventory[inventoryOrder].SkillAtk.ToString();
            _skillCoolTimeText.text = _dataWrapper.CharacterInventory[inventoryOrder].SkillCoolTime.ToString();
            _dashPowerText.text = _dataWrapper.CharacterInventory[inventoryOrder].DashImpulse.ToString();
            _dashCoolTimeText.text = _dataWrapper.CharacterInventory[inventoryOrder].DashCoolTime.ToString();
        }

        InventoryImageChange();
    }

    private void InventoryImageSetting()
    {
        int count = _InventoryImageObject.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            _InventoryImageObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        _InventoryImageObject.transform.Find("LenderCamera").gameObject.SetActive(true);
        _InventoryImageObject.transform.Find(GameManager.I.DataManager.PlayerData.Tag).gameObject.SetActive(true);
        _InventoryImage.color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
    }

    private void InventoryImageChange()
    {
        int count = _InventoryImageObject.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            _InventoryImageObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        _InventoryImageObject.transform.Find("LenderCamera").gameObject.SetActive(true);
        _InventoryImageObject.transform.Find(_inventorySelectData.Tag).gameObject.SetActive(true);

        if (!CharacterIsGet(_inventorySelectData)) _InventoryImage.color = new Color(20 / 255f, 20 / 255f, 20 / 255f, 255 / 255f);
        else _InventoryImage.color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
    }

    public void EquipButton()
    {
        if (_characterNum == -1)
        {
            GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
        }
        else if (CharacterIsGet(_inventorySelectData))
        {
            GameManager.I.SoundManager.StartSFX("EquipButton");

            for (int i = 0; i < _inventory.Count; i++)
            {
                if (_inventorySelectData.Tag == _inventory[i].Tag)
                {
                    _dataWrapper.CharacterInventory[i].IsEquip = true;
                    GameManager.I.DataManager.PlayerData = _dataWrapper.CharacterInventory[i];
                }
                else _dataWrapper.CharacterInventory[i].IsEquip = false;
            }
        }
        else
        {
            GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
        }

        UserNameSetting();
        LevelSetting();
        ExpSetting();
        GameManager.I.DataManager.DataSave();
    }

    private void EquipSetting()
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (!_inventory[i].IsEquip) continue;
            else
            {
                GameManager.I.DataManager.PlayerData = _inventory[i];
                break;
            }
        }
    }

    private bool CharacterIsGet(CharacterData data)
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (data.Tag == _inventory[i].Tag) return true;
        }

        return false;
    }

    public void CaracterSelectButton(int num)
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _charactetSelectNum = num;
        _characterSelectOKPanel.SetActive(true);
    }

    private void ActiveStar(int starNum)
    {
        for (int i = 0; i < 5; i++)
        {
            _playerUpgradeStars[i].SetActive(false);
        }

        if (starNum == 0) return;
        else _playerUpgradeStars[starNum - 1].SetActive(true);
    }
    #endregion

    #region CharacterSelect
    public void CharacterSelectCancleButton()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _characterSelectOKPanel.SetActive(false);
    }

    public void CharacterSelectConfirmedButton()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");

        if(_charactetSelectNum == 0)
        {
            GameManager.I.DataManager.DataWrapper.CharacterInventory.Add(_dataWrapper.CharacterDatas[0]);
            GameManager.I.DataManager.DataWrapper.CharacterInventory[0].IsEquip = true;
            GameManager.I.DataManager.PlayerData = _inventory[0];
        }
        else if (_charactetSelectNum == 1)
        {
            GameManager.I.DataManager.DataWrapper.CharacterInventory.Add(_dataWrapper.CharacterDatas[3]);
            GameManager.I.DataManager.DataWrapper.CharacterInventory[0].IsEquip = true;
            GameManager.I.DataManager.PlayerData = _inventory[0];
        }
        else if (_charactetSelectNum == 2)
        {
            GameManager.I.DataManager.DataWrapper.CharacterInventory.Add(_dataWrapper.CharacterDatas[1]);
            GameManager.I.DataManager.DataWrapper.CharacterInventory[0].IsEquip = true;
            GameManager.I.DataManager.PlayerData = _inventory[0];
        }

        UserNameSetting();
        _characterSelectPanel.SetActive(false);
        GameManager.I.DataManager.DataSave();
    }

    private void CharacterSelectActive()
    {
        PlayerPrefs.SetInt("Tutorial", -1);
        GameManager.I.DataManager.GameData.UserName = GameManager.I.DataManager.GameData.LoginID;
        _character1AtkText.text = _dataWrapper.CharacterDatas[0].OriginAtk.ToString();
        _character1DefText.text = _dataWrapper.CharacterDatas[0].OriginDef.ToString();
        _character1SpeedText.text = _dataWrapper.CharacterDatas[0].OriginSpeed.ToString();
        _character2AtkText.text = _dataWrapper.CharacterDatas[3].OriginAtk.ToString();
        _character2DefText.text = _dataWrapper.CharacterDatas[3].OriginDef.ToString();
        _character2SpeedText.text = _dataWrapper.CharacterDatas[3].OriginSpeed.ToString();
        _character3AtkText.text = _dataWrapper.CharacterDatas[1].OriginAtk.ToString();
        _character3DefText.text = _dataWrapper.CharacterDatas[1].OriginDef.ToString();
        _character3SpeedText.text = _dataWrapper.CharacterDatas[1].OriginSpeed.ToString();
        _characterSelectPanel.SetActive(true);
        GameManager.I.UIManager.UserNameSettingActive();
    }
    #endregion

    #region Shop
    public void ShopActive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _shopCoinText.text = _gameData.Coin.ToString();
        _shopPanel.SetActive(true);
    }

    public void ShopInactive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        CoinSetting();
        _shopPanel.SetActive(false);
    }

    public void DrawCharacter(int B, int A, int S, int SS)
    {
        int count = _dataWrapper.CharacterDatas.Length;

        while (true)
        {
            int CharacterRank = Random.Range(0, 4);
            int CharacterNum = Random.Range(0, count);
            int randomValue = Random.Range(1, 101);

            if(CharacterRank == 0)  // B·©Å©
            {
                if (_dataWrapper.CharacterDatas[CharacterNum].CharacterRank != CharacterData.Rank.B) continue;
                else
                {
                    if (randomValue <= B)  // B·©Å© È®·ü
                    {
                        _drawCharacter = _dataWrapper.CharacterDatas[CharacterNum];

                        if (!CharacterIsGet(_drawCharacter)) GameManager.I.DataManager.DataWrapper.CharacterInventory.Add(_drawCharacter);
                        else
                        {
                            int inventoryOrder = FindInventoryOrder(_drawCharacter);
                            StarExpUp(inventoryOrder);
                        }
                        break;
                    }
                    else continue;
                }
            }
            else if(CharacterRank == 1) // A·©Å©
            {
                if (_dataWrapper.CharacterDatas[CharacterNum].CharacterRank != CharacterData.Rank.A) continue;
                else
                {
                    if (randomValue <= A)  // A·©Å© È®·ü
                    {
                        _drawCharacter = _dataWrapper.CharacterDatas[CharacterNum];

                        if (!CharacterIsGet(_drawCharacter)) GameManager.I.DataManager.DataWrapper.CharacterInventory.Add(_drawCharacter);
                        else
                        {
                            int inventoryOrder = FindInventoryOrder(_drawCharacter);
                            StarExpUp(inventoryOrder);
                        }
                        break;
                    }
                    else continue;
                }
            }
            else if(CharacterRank == 2) // S·©Å©
            {
                if (_dataWrapper.CharacterDatas[CharacterNum].CharacterRank != CharacterData.Rank.S) continue;
                else
                {
                    if (randomValue <= S)  // S·©Å© È®·ü
                    {
                        _drawCharacter = _dataWrapper.CharacterDatas[CharacterNum];

                        if (!CharacterIsGet(_drawCharacter)) GameManager.I.DataManager.DataWrapper.CharacterInventory.Add(_drawCharacter);
                        else
                        {
                            int inventoryOrder = FindInventoryOrder(_drawCharacter);
                            StarExpUp(inventoryOrder);
                        }
                        break;
                    }
                    else continue;
                }
            }
            else if (CharacterRank == 3) // SS·©Å©
            {
                if (_dataWrapper.CharacterDatas[CharacterNum].CharacterRank != CharacterData.Rank.SS) continue;
                else
                {
                    if (randomValue <= SS)  // SS·©Å© È®·ü
                    {
                        _drawCharacter = _dataWrapper.CharacterDatas[CharacterNum];

                        if (!CharacterIsGet(_drawCharacter)) GameManager.I.DataManager.DataWrapper.CharacterInventory.Add(_drawCharacter);
                        else
                        {
                            int inventoryOrder = FindInventoryOrder(_drawCharacter);
                            StarExpUp(inventoryOrder);
                        }
                        break;
                    }
                    else continue;
                }
            }
        }
    }

    private void HeroPanelSetting()
    {
        _drawCharacterText.text = _drawCharacter.KoreaTag.ToString();
        _heroPanelSlotRawImage.texture = Resources.Load<Texture>("RenderTextures/CharacterRenderTexture_Slot_" + _drawCharacter.Tag.ToString());

        if (_drawCharacter.CharacterRank == CharacterData.Rank.B)
        {
            _heroPanelFrameImage.sprite = _frameImages[0];
            _heroPanelSlotImage.color = new Color(40 / 255f, 36 / 255f, 29 / 255f, 255 / 255f);
        }
        else if (_drawCharacter.CharacterRank == CharacterData.Rank.A)
        {
            _heroPanelFrameImage.sprite = _frameImages[1];
            _heroPanelSlotImage.color = new Color(20 / 255f, 46 / 255f, 34 / 255f, 255 / 255f);
        }
        else if (_drawCharacter.CharacterRank == CharacterData.Rank.S)
        {
            _heroPanelFrameImage.sprite = _frameImages[2];
            _heroPanelSlotImage.color = new Color(39 / 255f, 10 / 255f, 8 / 255f, 255 / 255f);
        }
        else if (_drawCharacter.CharacterRank == CharacterData.Rank.SS)
        {
            _heroPanelFrameImage.sprite = _frameImages[3];
            _heroPanelSlotImage.color = new Color(33 / 255f, 13 / 255f, 52 / 255f, 255 / 255f);
        }

        _heroPanelRankText.text = _drawCharacter.CharacterRank.ToString();
        _heroPanelAtkText.text = _drawCharacter.Atk.ToString();
        _heroPanelDefText.text = _drawCharacter.Def.ToString();
        _heroPanelSpeedText.text = _drawCharacter.Speed.ToString();
        _heroPanelSkillAtkText.text = _drawCharacter.SkillAtk.ToString();
        _heroPanelSkillCoolTimeText.text = _drawCharacter.SkillCoolTime.ToString();
        _heroPanelDashPowerText.text = _drawCharacter.DashImpulse.ToString();
        _heroPanelDashCoolTimeText.text = _drawCharacter.DashCoolTime.ToString();
    }

    public void DrawNormalCharacterButton(int num)
    {
        if(num == 1)
        {
            if (_gameData.Coin < 200)
            {
                GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
            }
            else
            {
                GameManager.I.SoundManager.StartSFX("CharacterGetButton");
                GameManager.I.DataManager.GameData.Coin -= 200;
                _shopCoinText.text = _gameData.Coin.ToString();

                // »Ì±â È®·ü ¼³Á¤
                DrawCharacter(70, 25, 4, 1);
                
                HeroPanelSetting();
                HeroDrawOKButtonSetting(1);

                _heroPanel.SetActive(true);
            }
        }
        else if (num == 10)
        {
            if(_drawCount == 0)
            {
                if (_gameData.Coin < 1800)
                {
                    GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
                }
                else
                {
                    GameManager.I.SoundManager.StartSFX("CharacterGetButton");
                    GameManager.I.DataManager.GameData.Coin -= 1800;
                    _shopCoinText.text = _gameData.Coin.ToString();

                    // »Ì±â È®·ü ¼³Á¤
                    DrawCharacter(70, 25, 4, 1);

                    HeroPanelSetting();
                    HeroDrawOKButtonSetting(2);

                    _heroPanel.SetActive(true);
                }
            }
            else
            {
                GameManager.I.SoundManager.StartSFX("CharacterGetButton");

                // »Ì±â È®·ü ¼³Á¤
                DrawCharacter(70, 25, 4, 1);

                HeroPanelSetting();
                HeroDrawOKButtonSetting(2);

                _heroPanel.SetActive(true);
            }
        }

        //CharacterStatSetting();
        GameManager.I.DataManager.DataSave();
    }

    public void DrawRareCharacterButton(int num)
    {
        if (num == 1)
        {
            if (_gameData.Coin < 500)
            {
                GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
            }
            else
            {
                GameManager.I.SoundManager.StartSFX("CharacterGetButton");
                GameManager.I.DataManager.GameData.Coin -= 500;
                _shopCoinText.text = _gameData.Coin.ToString();

                // »Ì±â È®·ü ¼³Á¤
                DrawCharacter(50, 35, 10, 5);

                HeroPanelSetting();
                HeroDrawOKButtonSetting(3);

                _heroPanel.SetActive(true);
            }
        }
        else if (num == 10)
        {
            if (_drawCount == 0)
            {
                if (_gameData.Coin < 4500)
                {
                    GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
                }
                else
                {
                    GameManager.I.SoundManager.StartSFX("CharacterGetButton");
                    GameManager.I.DataManager.GameData.Coin -= 4500;
                    _shopCoinText.text = _gameData.Coin.ToString();

                    // »Ì±â È®·ü ¼³Á¤
                    DrawCharacter(50, 35, 10, 5);

                    HeroPanelSetting();
                    HeroDrawOKButtonSetting(4);

                    _heroPanel.SetActive(true);
                }
            }
            else
            {
                GameManager.I.SoundManager.StartSFX("CharacterGetButton");

                // »Ì±â È®·ü ¼³Á¤
                DrawCharacter(50, 35, 10, 5);

                HeroPanelSetting();
                HeroDrawOKButtonSetting(4);

                _heroPanel.SetActive(true);
            }
        }

        //CharacterStatSetting();
        GameManager.I.DataManager.DataSave();
    }

    public void DrawUniqueCharacterButton(int num)
    {
        if (num == 1)
        {
            if (_gameData.Coin < 1000)
            {
                GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
            }
            else
            {
                GameManager.I.SoundManager.StartSFX("CharacterGetButton");
                GameManager.I.DataManager.GameData.Coin -= 1000;
                _shopCoinText.text = _gameData.Coin.ToString();

                // »Ì±â È®·ü ¼³Á¤
                DrawCharacter(40, 30, 20, 10);

                HeroPanelSetting();
                HeroDrawOKButtonSetting(5);

                _heroPanel.SetActive(true);
            }
        }
        else if (num == 10)
        {
            if (_drawCount == 0)
            {
                if (_gameData.Coin < 9000)
                {
                    GameManager.I.SoundManager.StartSFX("ButtonClickMiss");
                }
                else
                {
                    GameManager.I.SoundManager.StartSFX("CharacterGetButton");
                    GameManager.I.DataManager.GameData.Coin -= 9000;
                    _shopCoinText.text = _gameData.Coin.ToString();

                    // »Ì±â È®·ü ¼³Á¤
                    DrawCharacter(40, 30, 20, 10);

                    HeroPanelSetting();
                    HeroDrawOKButtonSetting(6);

                    _heroPanel.SetActive(true);
                }
            }
            else
            {
                GameManager.I.SoundManager.StartSFX("CharacterGetButton");

                // »Ì±â È®·ü ¼³Á¤
                DrawCharacter(40, 30, 20, 10);

                HeroPanelSetting();
                HeroDrawOKButtonSetting(6);

                _heroPanel.SetActive(true);
            }
        }

        //CharacterStatSetting();
        GameManager.I.DataManager.DataSave();
    }

    public void DrawNormalCharacterOKButton(int num)
    {
        if(num == 1)
        {
            GameManager.I.SoundManager.StartSFX("ButtonClick");
            _heroPanel.SetActive(false);
        }
        else if(num == 10)
        {
            _drawCount++;

            if (_drawCount >= 10)
            {
                GameManager.I.SoundManager.StartSFX("ButtonClick");
                _drawCount = 0;
                _heroPanel.SetActive(false);
            }
            else
            {
                _heroPanel.SetActive(false);
                DrawNormalCharacterButton(10);
            }
        }

        GameManager.I.DataManager.DataSave();
    }

    public void DrawRareCharacterOKButton(int num)
    {
        if (num == 1)
        {
            GameManager.I.SoundManager.StartSFX("ButtonClick");
            _heroPanel.SetActive(false);
        }
        else if (num == 10)
        {
            _drawCount++;

            if (_drawCount >= 10)
            {
                GameManager.I.SoundManager.StartSFX("ButtonClick");
                _drawCount = 0;
                _heroPanel.SetActive(false);
            }
            else
            {
                _heroPanel.SetActive(false);
                DrawRareCharacterButton(10);
            }
        }

        GameManager.I.DataManager.DataSave();
    }

    public void DrawUniqueCharacterOKButton(int num)
    {
        if (num == 1)
        {
            GameManager.I.SoundManager.StartSFX("ButtonClick");
            _heroPanel.SetActive(false);
        }
        else if (num == 10)
        {
            _drawCount++;

            if (_drawCount >= 10)
            {
                GameManager.I.SoundManager.StartSFX("ButtonClick");
                _drawCount = 0;
                _heroPanel.SetActive(false);
            }
            else
            {
                _heroPanel.SetActive(false);
                DrawUniqueCharacterButton(10);
            }
        }

        GameManager.I.DataManager.DataSave();
    }

    public int FindInventoryOrder(CharacterData data)
    {
        int count = _inventory.Count;

        for (int i = 0; i < count; i++)
        {
            if (data.Tag == _inventory[i].Tag) return i;
            else continue;
        }

        return -1;
    }

    private void StarExpUp(int inventoryOrder)
    {
        if (_inventory[inventoryOrder].Star == 5) return;
        
        GameManager.I.DataManager.DataWrapper.CharacterInventory[inventoryOrder].CurrentStarExp++;

        if (_inventory[inventoryOrder].MaxStarExp == _inventory[inventoryOrder].CurrentStarExp)
        {
            GameManager.I.DataManager.DataWrapper.CharacterInventory[inventoryOrder].Star++;
            GameManager.I.DataManager.DataWrapper.CharacterInventory[inventoryOrder].CurrentStarExp = 1;
            GameManager.I.DataManager.DataWrapper.CharacterInventory[inventoryOrder].MaxStarExp = 3 * _inventory[inventoryOrder].Star;
        }
    }

    private void HeroDrawOKButtonSetting(int num)
    {
        int count = _heroPanelOKButton.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            _heroPanelOKButton.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (num == 0) return;

        _heroPanelOKButton.transform.GetChild(num - 1).gameObject.SetActive(true);
    }
    #endregion

    #region Rank
    public void RankActive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _rankSystem.GetMyRank();
        _rankSystem.GetRankList();
        _rankPanel.SetActive(true);
    }

    public void RankInactive()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");
        _rankPanel.SetActive(false);
    }

    public void UpdateRank(int value)
    {
        _rankSystem.UpdateRank(value);
    }
    #endregion
}
