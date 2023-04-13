using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class SceneSetupTests
{
    [UnityTest]
    public IEnumerator MainScene_LoadsCorrectlyAndItsDaytime()
    {
        SceneManager.LoadScene("Assets/Scenes/Title Screen.unity", LoadSceneMode.Single);
        yield return null;

        var eventSystem = GameObject.Find("Title");

        Assert.IsTrue(eventSystem != null, "should find the 'Title' object in the scene");
    }
}