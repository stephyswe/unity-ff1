using System;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class OverworldUpdate : MonoBehaviour {

		[FormerlySerializedAs("need_flags_to_show")]
		public bool needFlagsToShow;
		[FormerlySerializedAs("flags_to_show")]
		public FlagCheck[] flagsToShow;

		public GameObject old;
		public GameObject updated;

		[FormerlySerializedAs("start_with_update")]
		public bool startWithUpdate;

		// Update is called once per frame
		void Update() {}

		// Start is called before the first frame update
		void OnEnable() {
			date_map();
			bool update = true;
			if (startWithUpdate)
				update = true;
			if (needFlagsToShow) {
				foreach (FlagCheck fc in flagsToShow) {
					if (!fc.Check())
						update = false;
				}
			}
			if (update)
				update_map();
		}

		public void update_map() {
			old.SetActive(false);
			updated.SetActive(true);
		}

		public void date_map() {
			old.SetActive(true);
			updated.SetActive(false);
		}

		[Serializable]
		public class FlagCheck {
			public string name;
			public bool flagValToShow;
			[FormerlySerializedAs("replacementNPC")]
			public GameObject replacementNpc;

			public bool Check() {
				return SaveSystem.GetBool(name) == flagValToShow;
			}
		}
	}
}
