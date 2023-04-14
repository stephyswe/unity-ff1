using System.Collections.Generic;
using System.Linq;
using Battling;

namespace Refactor {
	public class PartyHandler {
		readonly List<PartyMember> party;

		public PartyHandler(List<PartyMember> party) {
			this.party = party;
		}

		public float GetPartyAverageLevel() {
			float level = 0;
			float count = 0;
			foreach (PartyMember p in party.Where(p => p.hp > 0)) {
				level += p.level;
				count += 1f;
			}
			return level / count;
		}
	}
}
