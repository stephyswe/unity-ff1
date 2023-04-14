namespace Refactor {
	public class MonsterHandler {
		public static string ProcessMonsterName(string mName) {
			if (!mName.Contains("("))
				return mName;
			return mName.Substring(0, mName.IndexOf("(") - 1);
		}
	}
}
