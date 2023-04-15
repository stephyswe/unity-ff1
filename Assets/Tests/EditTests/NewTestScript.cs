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
		[TestCase("fighter", new[] {20f, 5f, 1f, 10f, 5f, 35f, 0.1f, 0.15f, 0f}, 3)]
		[TestCase("black_belt", new[] {5f, 5f, 5f, 20f, 5f, 33f, 0.05f, 0.1f, 0f}, 0)]
		[TestCase("red_mage", new[] {10f, 10f, 10f, 5f, 5f, 30f, 0.07f, 0.2f, 10f}, 7)]
		[TestCase("thief", new[] {5f, 15f, 5f, 5f, 10f, 30f, 0.05f, 0.15f, 0f}, 9)]
		[TestCase("white_mage", new[] {5f, 5f, 15f, 10f, 5f, 28f, 0.05f, 0.2f, 10f}, 10)]
		[TestCase("black_mage", new[] {1f, 10f, 20f, 1f, 10f, 25f, 0.055f, 0.2f, 10f}, 1)]
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
