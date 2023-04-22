using System.Collections.Generic;
using Overworld.Controller;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class GlobalControl : MonoBehaviour {

		public static GlobalControl Instance;

		public MapHandler mh;

		public Boss boss;
		public bool bossvictory;

		[FormerlySerializedAs("overworld_scene_container")]
		public GameObject overworldSceneContainer;
		[FormerlySerializedAs("monster_party")]
		public GameObject monsterParty;

		public PlayerController player;

		public string shopmode;
		[FormerlySerializedAs("inn_clinic_price")]
		public int innClinicPrice;

		public bool bossmode;
		public Dictionary<string, int> ShopProducts;

		// Start is called before the first frame update
		void Awake() {
			if (Instance == null)
				Instance = this;
			else if (Instance != this)
				Destroy(gameObject);
		}

		// Update is called once per frame
		void Update() {
			if (!bossvictory)
				return;
			SaveSystem.SetBool(boss.flag, boss.flagval);

			Destroy(boss.gameObject);

			player.canMove = true;
			player.multiplier = 2f;

			player.pauseMenuContainer.SetActive(true);

			player.mapHandler.save_position();
			player.reh.battling = false;
			bossmode = false;
			bossvictory = false;

			overworldSceneContainer.SetActive(true);
		}
	}
}
