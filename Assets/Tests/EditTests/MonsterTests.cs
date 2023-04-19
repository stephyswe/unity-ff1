using Battling;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditTests {
	public class MonsterTests
	{
		// A Test behaves as an ordinary method
		[Test]
		public void _01_Find_Monster() {
			// Use the Assert class to test conditions
			GameObject testObject = new GameObject();
			Monster monster = testObject.AddComponent<Monster>();
			monster.damageHigh = 0;
			Assert.That(monster, Is.Not.Null);
		}
	}
}


