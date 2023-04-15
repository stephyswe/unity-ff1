using System.Collections;
using Battling;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditTests {
	public class GameTest {
		// A Test behaves as an ordinary method
		[Test]
		public void MonsterTests() {
			// Use the Assert class to test conditions
			GameObject testObject = new GameObject();
			Monster monster = testObject.AddComponent<Monster>();
			monster.damageHigh = 0;
			Assert.That(monster, Is.Not.Null);
		}
		
		[Test]
		// Test cases for GetCharacterValues
		[TestCase("fighter", new[] {20, 5, 1, 10, 5, 35, 0.1f, .15f, 0}, 3)]
		[TestCase("black_belt", new[] {5, 5, 5, 20, 5, 33, .05f, .1f, 0}, 0)]
		[TestCase("red_mage", new[] {10, 10, 10, 5, 5, 30, .07f, .2f, 10}, 7)]
		[TestCase("thief", new[] {5, 10, 5, 5, 15, 30, .05f, .15f, 0}, 9)]
		[TestCase("white_mage", new[] {5, 5, 15, 10, 5, 28, .05f, .2f, 10}, 10)]
		[TestCase("black_mage", new[] {1, 10, 20, 1, 10, 25, .055f, .2f, 10}, 1)]
		[TestCase("invalid", null, 0)]
		public void TestGetCharacterValues(string pClass, float[] expectedValues, int expectedIndex) {
			// Act
			(float[] actualValues, int actualIndex) = TitleScreen.TitleScreenHandler.GetCharacterValues(pClass);

			// Assert
			Assert.AreEqual(expectedValues, actualValues);
			Assert.AreEqual(expectedIndex, actualIndex);
		}

		// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
		// `yield return null;` to skip a frame.
		[UnityTest]
		public IEnumerator GameTestWithEnumeratorPasses() {
			// Use the Assert class to test conditions.
			// Use yield to skip a frame.
			yield return null;
		}
	}
}
