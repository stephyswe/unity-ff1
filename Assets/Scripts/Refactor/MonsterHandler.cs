using System;

namespace Refactor {
	public abstract class MonsterHandler {
		public static string ProcessMonsterName(string mName) {
			return mName.Contains("(") ? mName[..(mName.IndexOf("(", StringComparison.Ordinal) - 1)] : mName;
		}
	}
}
