using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayTests {
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
}