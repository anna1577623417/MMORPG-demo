﻿using SceneName;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneManager : MonoSingleton<SceneManager>
{
    UnityAction<float> onProgress = null;

    public UnityEngine.Events.UnityAction ReadyToClickPlayButtonAgain;//About UICharacterSelect.cs
    
    protected override void OnStart()
    {
        
    }

   
    void Update () {
		
	}

    public void LoadScene(string name)
    {
        StartCoroutine(LoadLevel(name));
    }

    IEnumerator LoadLevel(string name)
    {
        Debug.LogFormat("LoadLevel: {0}", name);
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = true;
        async.completed += LevelLoadCompleted;
        while (!async.isDone)
        {
            if (onProgress != null)
                onProgress(async.progress);
            yield return null;
        }
        if ((name == Scene.Name.CharacterSelect.ToString())){
            ReadyToClickPlayButtonAgain();
        }
    }
    
    private void LevelLoadCompleted(AsyncOperation obj)
    {
        if (onProgress != null)
            onProgress(1f);
        Debug.Log("LevelLoadCompleted:" + obj.progress);
    }
}
