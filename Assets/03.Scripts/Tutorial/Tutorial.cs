using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject _tutorialObject;
    [SerializeField] private GameObject[] _tutorialTextObject;

    public int ButtonClickCount;

    private void Start()
    {
        ButtonClickCount = 1;

        if(PlayerPrefs.GetInt("BattleSceneTutorial") == 0) StartCoroutine(COStartTutorial());
    }

    public void NextButton()
    {
        GameManager.I.SoundManager.StartSFX("ButtonClick");

        if (ButtonClickCount >= _tutorialTextObject.Length)
        {
            PlayerPrefs.SetInt("BattleSceneTutorial", 1);
            _tutorialObject.SetActive(false);
            Time.timeScale = 1f;
            return;
        }

        TutorialActive(ButtonClickCount);
        ButtonClickCount++;
    }

    private void TutorialActive(int num)
    {
        for (int i = 0; i < _tutorialTextObject.Length; i++)
        {
            _tutorialTextObject[i].SetActive(false);
        }

        _tutorialTextObject[num].SetActive(true);
        GameManager.I.SoundManager.StartSFX("Text");
    }

    IEnumerator COStartTutorial()
    {
        yield return new WaitForSeconds(4f);

        Time.timeScale = 0f;
        _tutorialObject.SetActive(true);
        TutorialActive(0);
    }
}
