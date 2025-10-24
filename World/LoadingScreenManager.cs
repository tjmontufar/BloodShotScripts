using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public Slider progressBar;

    private static string sceneToLoad;

    public static void LoadScene(string sceneName)
    {
        sceneToLoad = sceneName;

        SceneManager.LoadScene("LoadingScene");
    }
    void Start()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(sceneToLoad);

        gameLevel.allowSceneActivation = false;

        while(!gameLevel.isDone)
        {
            float progress = Mathf.Clamp01(gameLevel.progress / 0.9f);

            if(progressBar != null)
            {
                progressBar.value = progress;
            }

            if(gameLevel.progress >= 0.9f)
            {
                yield return new WaitForSeconds(2f);

                gameLevel.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
