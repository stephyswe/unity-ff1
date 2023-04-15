using System;

namespace TitleScreen {
	public partial class TitleScreenHandler {
		public static (float[] values, int characterIndex) GetCharacterValues(string pClass) {
			float[] values = null;
			int characterIndex = 0;
			switch (pClass) {
				case "fighter":
					values = new[] {20f, 5f, 1f, 10f, 5f, 35f, 0.1f, 0.15f, 0f};
					characterIndex = 3;
					break;
				case "black_belt":
					values = new[] {5f, 5f, 5f, 20f, 5f, 33f, 0.05f, 0.1f, 0f};
					characterIndex = 0;
					break;
				case "red_mage":
					values = new[] {10f, 10f, 10f, 5f, 5f, 30f, 0.07f, 0.2f, 10f};
					characterIndex = 7;
					break;
				case "thief":
					values = new[] {5f, 15f, 5f, 5f, 10f, 30f, 0.05f, 0.15f, 0f};
					characterIndex = 9;
					break;
				case "white_mage":
					values = new[] {5f, 5f, 15f, 10f, 5f, 28f, 0.05f, 0.2f, 10f};
					characterIndex = 10;
					break;
				case "black_mage":
					values = new[] {1f, 10f, 20f, 1f, 10f, 25f, 0.055f, 0.2f, 10f};
					characterIndex = 1;
					break;
				default:
					Console.WriteLine("Invalid class name.");
					break;
			}
			return (values, characterIndex);
		}
	}
}
