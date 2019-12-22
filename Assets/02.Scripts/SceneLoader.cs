﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public CanvasGroup fadeCg;
    [Range(0.5f, 2.0f)]
    public float fadeDuration = 1.0f;

    public Dictionary<string, LoadSceneMode> loadScenes = new Dictionary<string, LoadSceneMode>();

    void InitSceneInfo()
    {
        loadScenes.Add("Level1", LoadSceneMode.Additive);
        loadScenes.Add("Play", LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        InitSceneInfo();

        fadeCg.alpha = 1.0f;

        foreach (var loadScene in loadScenes)
        {
            yield return StartCoroutine(LoadScene(loadScene.Key, loadScene.Value));
        }

        StartCoroutine(Fade(0.9f));
    }

    IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, mode);

        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(loadedScene);
    }

    IEnumerator Fade(float finalAlpha)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level1"));
        fadeCg.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(fadeCg.alpha - finalAlpha) / fadeDuration;

        while (!Mathf.Approximately(fadeCg.alpha, finalAlpha))
        {
            fadeCg.alpha = Mathf.MoveTowards(fadeCg.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        fadeCg.blocksRaycasts = false;

        SceneManager.UnloadSceneAsync("SceneLoader");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
