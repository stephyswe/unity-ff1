using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayTests
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            SceneManager.LoadScene("Assets/Scenes/Title Screen.unity", LoadSceneMode.Single);
            
            yield return null;

            GameObject eventSystem = GameObject.Find("Title");

            Assert.IsTrue(eventSystem != null, "should find the 'Title' object in the scene");
            
            // Mouse movement
            
            // LeftMouseClick( (int)xPos, (int)yPos );

            yield return new WaitForSeconds(4);
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
