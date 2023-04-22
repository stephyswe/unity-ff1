using System.Collections;
using System.Collections.Generic;
using Overworld.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Overworld {
	public class ShopWarp : MonoBehaviour {
		public PlayerController player;

		public string shopmode;
		[FormerlySerializedAs("inn_clinic_price")]
		public int innClinicPrice;

		public string[] products;

		public bool shopping;

		Equips equips;

		// Start is called before the first frame update
		void Start() {
			equips = new Equips();
		}

		// Update is called once per frame
		void Update() {}

		public IEnumerator Warp() {
			shopping = true;

			player.pauseMenuContainer.SetActive(false);

			player.canMove = false;

			GlobalControl.Instance.ShopProducts = new Dictionary<string, int>();

			GlobalControl.Instance.shopmode = shopmode;
			GlobalControl.Instance.innClinicPrice = innClinicPrice;
			if (GlobalControl.Instance.ShopProducts == null)
				GlobalControl.Instance.ShopProducts = new Dictionary<string, int>();
			foreach (string p in products) {
				KeyValuePair<string, int> namePrice = equips.name_price(p);
				Debug.Log(p);
				GlobalControl.Instance.ShopProducts.Add(namePrice.Key, namePrice.Value);
			}

			int countLoaded = SceneManager.sceneCount;
			if (countLoaded == 1) {
				GlobalControl.Instance.overworldSceneContainer.SetActive(false);

				SceneManager.LoadScene("Shop", LoadSceneMode.Additive);

				player.sc.change_direction("down");

				while (SceneManager.sceneCount > 1)
					yield return null;

				GlobalControl.Instance.overworldSceneContainer.SetActive(true);
			}

			countLoaded = SceneManager.sceneCount;

			while (countLoaded > 1) {
				countLoaded = SceneManager.sceneCount;
				yield return null;
			}

			player.canMove = true;

			shopping = false;

			yield return null;
		}
	}
}
