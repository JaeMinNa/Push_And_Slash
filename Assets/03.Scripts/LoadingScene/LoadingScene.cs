using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    public static string NextScene;
    [SerializeField] Slider _loadingBar;
    [SerializeField] TMP_Text _loadingText;

    private void Start()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadScene());
    }

    private void Update()
    {
        _loadingText.text = "Loading... " + (int)(_loadingBar.value * 100) + "%";
    }

    public static void LoadScene(string sceneName)
    {
        NextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(NextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;

        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
           
            if (op.progress < 0.9f)
            {
                _loadingBar.value = Mathf.Lerp(_loadingBar.value, op.progress, timer);
                if (_loadingBar.value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                _loadingBar.value = Mathf.Lerp(_loadingBar.value, 1f, timer);
                if (_loadingBar.value == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}