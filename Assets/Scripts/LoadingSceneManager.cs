using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;
    [SerializeField]
    private Slider loadingProgressBar;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string _nextScene)
    {
        nextScene = _nextScene;
        SceneManager.LoadScene("Loading");
    }
    
    IEnumerator LoadScene() 
    { 
        yield return null; 
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene); 
        op.allowSceneActivation = false; 
        float timer = 0.0f; 
        while (!op.isDone) 
        { 
            yield return null; 
            timer += Time.deltaTime; 
            if (op.progress < 0.9f) 
            { 
                loadingProgressBar.value = Mathf.Lerp(loadingProgressBar.value, op.progress, timer); 
                if (loadingProgressBar.value >= op.progress) 
                { 
                    timer = 0f; 
                } 
            } 
            else 
            { 
                loadingProgressBar.value = Mathf.Lerp(loadingProgressBar.value, 1f, timer); 
                if (loadingProgressBar.value == 1.0f) 
                { 
                    op.allowSceneActivation = true; 
                    yield break; 
                } 
            } 
        } 
    }
}
