using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        // Get the target scene name from the static class
        string nextSceneName = SceneLoader.NextSceneName;

        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneName);

        // Wait until the scene is fully loaded
        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
