using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private int _characterNum;
    private CharacterData _slotData;
    private GameObject _equipFrame;
    private GameObject _levelBackground;
    private GameObject _slotUpgradeStar;
    private DataWrapper _dataWrapper;
    private LobbyController _lobbyController;
    private bool _isEquip;
    private List<CharacterData> _inventory;
    private TMP_Text _levelText;

    private void Start()
    {
        _dataWrapper = GameManager.I.DataManager.DataWrapper;
        _slotData = GameManager.I.DataManager.DataWrapper.CharacterDatas[_characterNum];
        _lobbyController = GameObject.FindWithTag("LobbyController").GetComponent<LobbyController>();
        _inventory = _dataWrapper.CharacterInventory;
        _equipFrame = transform.parent.parent.transform.GetChild(1).gameObject;
        _levelText = transform.parent.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        _levelBackground = transform.parent.transform.GetChild(2).gameObject;
        _slotUpgradeStar = transform.parent.transform.GetChild(4).gameObject;
        _isEquip = false;

        if (!CharacterIsGet())
        {
            transform.GetComponent<RawImage>().color = new Color(20 / 255f, 20 / 255f, 20 / 255f, 255 / 255f);
            _levelText.text = "";
            _levelBackground.SetActive(false);
            ActiveStar(0);
        }
        else
        {
            transform.GetComponent<RawImage>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
            int inventoryOrder = _lobbyController.FindInventoryOrder(_slotData);
            _levelText.text = "LV." + _inventory[inventoryOrder].Level.ToString();
            _levelBackground.SetActive(true);

            ActiveStar(_dataWrapper.CharacterInventory[inventoryOrder].Star);

            if (!CharacterIsEquip())
            {
                _equipFrame.SetActive(false);
                _isEquip = false;
            }
            else
            {
                _equipFrame.SetActive(true);
                _isEquip = true;
            }
        }
    }

    private void OnEnable()
    {
        if (_dataWrapper == null) return;

        if (!CharacterIsGet())
        {
            transform.GetComponent<RawImage>().color = new Color(20 / 255f, 20 / 255f, 20 / 255f, 255 / 255f);
            _levelText.text = "";
            _levelBackground.SetActive(false);
            ActiveStar(0);
        }
        else
        {
            transform.GetComponent<RawImage>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
            int inventoryOrder = _lobbyController.FindInventoryOrder(_slotData);
            _levelText.text = "LV." + _inventory[inventoryOrder].Level.ToString();
            _levelBackground.SetActive(true);

            ActiveStar(_dataWrapper.CharacterInventory[inventoryOrder].Star);

            if (!CharacterIsEquip())
            {
                _equipFrame.SetActive(false);
                _isEquip = false;
            }
            else
            {
                _equipFrame.SetActive(true);
                _isEquip = true;
            }
        }
    }

    private void Update()
    {
        if (CharacterIsGet() && !CharacterIsEquip() && !_isEquip)
        {
            _equipFrame.SetActive(false);
            _isEquip = true;
        }
        else if(CharacterIsGet() && CharacterIsEquip() && _isEquip)
        {
            _equipFrame.SetActive(true);
            _isEquip = false;
        }
    }

    private bool CharacterIsGet()
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (_slotData.Tag == _inventory[i].Tag) return true;
        }

        return false;
    }

    private bool CharacterIsEquip()
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (_slotData.Tag == _inventory[i].Tag)
            {
                if (_inventory[i].IsEquip == true) return true;
                else return false;
            }
        }

        return false;
    }

    private void ActiveStar(int starNum)
    {
        for (int i = 0; i < 5; i++)
        {
            _slotUpgradeStar.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (starNum == 0) return;
        else _slotUpgradeStar.transform.GetChild(starNum - 1).gameObject.SetActive(true);
    }
}
