using System.Collections;
using Battling;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditTests {
    public class GameTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void MonsterTests()
        {
            // Use the Assert class to test conditions
            GameObject testObject = new GameObject();
            Monster monster = testObject.AddComponent<Monster>();
            monster.damageHigh = 0;
            Assert.That(monster, Is.Not.Null);
        }



        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator GameTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
