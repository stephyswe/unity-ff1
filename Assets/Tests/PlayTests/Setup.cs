using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.PlayTests {
	public abstract class Setup
	{
		public static void Setup_01(out string testFilePath, out string sceneName, out string findObj, out string randomKey, out int[][] positions) {
			testFilePath = "./Assets/Tests/Files/const-party.json";
			sceneName = "Menu";
			findObj = "Title";
			randomKey = "reh_seed";
			positions = new[] {
				new[] {900, 500},
				new[] {1100, 600},
			};
		}
	}
}


