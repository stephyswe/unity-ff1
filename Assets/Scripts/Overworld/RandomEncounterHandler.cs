using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.SaveGame.Scripts.SaveSystem;

namespace Overworld {
	public class RandomEncounterHandler : MonoBehaviour {

		public int seed;

		public PlayerController player;
		public Transform cam;

		public bool encounters;

		public bool battling;

		bool won_boss_battle;

		// Start is called before the first frame update
		void Start() {
			if (!player == null)
				encounters = player.mapHandler.activeMap.GetComponent<Map>().encounters;
			seed = SaveSystem.GetInt("reh_seed");
			/*
		GameObject[] NPCs = FindObjectsOfType<NPC>();
		foreach(GameObject g in NPCs)
		{
			foreach (FlagCheck fc in g.GetComponent<NPC>().flags)
			{
			    if (!fc.check())
			        g.SetActive(false);
			    else
			        gameObject.SetActive(true);
			}
		}
		*/
		}

		// Update is called once per frame
		void Update() {
			if (seed <= 0 && !battling) {
				Debug.Log("Random encounter initiated");
				StartCoroutine(initiate_encounter());

				gen_seed();
				SaveSystem.SetInt("reh_seed", seed);
			}
		}

		public void set_encounters(bool onoff) {
			encounters = onoff;
		}

		public void gen_seed() {
			seed = Random.Range(50, 255);
		}

		public void Decrement(int d) {
			if (encounters)
				seed -= d;
		}

		IEnumerator initiate_encounter() {

			player.canMove = false;

			battling = true;

			while (player.transform.position != player.movePoint.position)
				yield return null;

			if (player.travelMode != "none") {

				player.pauseMenuContainer.SetActive(false);

				GlobalControl.Instance.monsterParty = player.og.get_monster_party();

				int countLoaded = SceneManager.sceneCount;
				if (countLoaded == 1) {

					player.canMove = false;
					player.multiplier = 0f;

					//gameObject.AddComponent(typeof(Camera));
					cam.transform.parent = gameObject.transform;

					GlobalControl.Instance.overworldSceneContainer.SetActive(false);

					//gameObject.AddComponent(typeof(AudioListener));

					AudioSource source = GetComponent<AudioSource>();
					source.Play();
					while (source.isPlaying)
						yield return null;

					Destroy(GetComponent<AudioListener>());

					SceneManager.LoadScene("Battle", LoadSceneMode.Additive);
					Destroy(GetComponent<Camera>());

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
				player.multiplier = 2f;

				player.pauseMenuContainer.SetActive(true);

				player.mapHandler.save_position();
				battling = false;
			}
			else
				seed = 1;

			yield return null;
		}

		public IEnumerator start_boss_battle(PlayerController player, GameObject boss, string flag, bool flagval, GameObject overworldBoss) {

			player.canMove = false;

			battling = true;
			player.pauseMenuContainer.SetActive(false);

			GlobalControl.Instance.monsterParty = boss;

			int countLoaded = SceneManager.sceneCount;
			if (countLoaded == 1) {
				player.multiplier = 0f;

				cam.transform.parent = gameObject.transform;

				GlobalControl.Instance.overworldSceneContainer.SetActive(false);
				GlobalControl.Instance.bossmode = true;

				//AudioSource source = GetComponent<AudioSource>();
				//source.Play();
				//yield return new WaitForSeconds(1.127f);

				SceneManager.LoadScene("Battle", LoadSceneMode.Additive);

				while (SceneManager.sceneCount > 1)
					yield return null;
			}



			yield return null;
		}
	}
}
