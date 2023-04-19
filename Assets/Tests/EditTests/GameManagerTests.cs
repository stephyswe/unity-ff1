using NUnit.Framework;
using TitleScreen;
using UnityEngine;

namespace Tests.EditTests {

	public class GameManagerTests {
		[Test]
		// Test cases for GetCharacterValues
		[TestCase("fighter", new[] {20, 5, 1, 10, 5, 35, 0.1f, .15f, 0}, 3)]
		[TestCase("black_belt", new[] {5, 5, 5, 20, 5, 33, .05f, .1f, 0}, 0)]
		[TestCase("red_mage", new[] {10, 10, 10, 5, 5, 30, .07f, .2f, 10}, 7)]
		[TestCase("thief", new[] {5, 10, 5, 5, 15, 30, .05f, .15f, 0}, 9)]
		[TestCase("white_mage", new[] {5, 5, 15, 10, 5, 28, .05f, .2f, 10}, 10)]
		[TestCase("black_mage", new[] {1, 10, 20, 1, 10, 25, .055f, .2f, 10}, 1)]
		[TestCase("invalid", null, 0)]
		public void _01_GetCharacterValues(string pClass, float[] expectedValues, int expectedIndex) {
			// Act
			(float[] actualValues, int actualIndex) = GameManager.GetCharacterValues(pClass);

			// Assert
			Assert.AreEqual(expectedValues, actualValues);
			Assert.AreEqual(expectedIndex, actualIndex);
		}

		[Test]
		public void _02_ContainerToggle() {
			// Load the scene
			Config.LoadEditScene("Menu");

			// Instantiate the objects in the scene
			GameObject container = new GameObject();
			GameObject title = new GameObject();

			// Add the script to one of the objects
			container.AddComponent<GameManager>();

			// Call the method on the script
			GameManager.ContainerToggle(container, title);

			// Assert the expected behavior
			Assert.IsTrue(title.activeSelf);
			Assert.IsFalse(container.activeSelf);
		}
	}
}
