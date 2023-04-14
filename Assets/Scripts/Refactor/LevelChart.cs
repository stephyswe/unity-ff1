using System.Collections.Generic;
using System.Linq;

namespace Refactor {
	public class LevelChart {
        /// <summary>
        ///     Returns the level corresponding to the given amount of experience points.
        /// </summary>
        /// <param name="exp">The amount of experience points.</param>
        /// <returns>The level corresponding to the given experience points.</returns>
        public static int GetLevelFromExp(int exp) {
			// Retrieve the level chart from the GetLevelChart method
			Dictionary<int, int> levelChart = GetLevelChart();

			// Set the initial level to 1
			int level = 1;

			// Iterate over the level chart until finding the level that corresponds to the given experience
			foreach (KeyValuePair<int, int> entry in levelChart.TakeWhile(entry => entry.Value <= exp)) {
				// Otherwise, update the level to the current level
				level = entry.Key;
			}

			// Return the resulting level
			return level;
		}

		// Returns a dictionary with the level chart
		public static Dictionary<int, int> GetLevelChart() {
			return new Dictionary<int, int> {
				{2, 40},
				{3, 196},
				{4, 547},
				{5, 1171},
				{6, 2146},
				{7, 3550},
				{8, 5461},
				{9, 7957},
				{10, 11116},
				{11, 15016},
				{12, 19753},
				{13, 25351},
				{14, 31942},
				{15, 39586},
				{16, 48361},
				{17, 58345},
				{18, 69617},
				{19, 82253},
				{20, 96332},
				{21, 111932},
				{22, 129131},
				{23, 148008},
				{24, 168639},
				{25, 191103},
				{26, 215479},
				{27, 241843},
				{28, 270275},
				{29, 300851},
				{30, 333651},
				{31, 366450},
				{32, 399250},
				{33, 432049},
				{34, 464849},
				{35, 497648},
				{36, 530448},
				{37, 563247},
				{38, 596047},
				{39, 628846},
				{40, 661646},
				{41, 694445},
				{42, 727245},
				{43, 760044},
				{44, 792844},
				{45, 825643},
				{46, 858443},
				{47, 891242},
				{48, 924042},
				{49, 956841},
				{50, 989641}
			};
		}
	}
}
