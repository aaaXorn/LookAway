using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField]
	private Image image;
    [SerializeField]
	private Text text;

    private void Start()
    {
        print(SaveSystem.NextScene);

        //começa o load
        StartCoroutine(LoadAsync(SaveSystem.NextScene));
    }

    private IEnumerator LoadAsync(string sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            image.fillAmount = progress;
            text.text = progress * 100f + "%";

            yield return null;
        }
    }
}