using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Battling {
	public class BattleSpriteController : MonoBehaviour {
		public Sprite idle;
		public Sprite walking1;
		public Sprite walking2;
		public Sprite attack1;
		public Sprite attack2;
		[FormerlySerializedAs("magic_victory1")]
		public Sprite magicVictory1;
		[FormerlySerializedAs("magic_victory2")]
		public Sprite magicVictory2;
		public Sprite tired;
		public Sprite stone;
		public Sprite dead;

		public SpriteRenderer sr;

		[FormerlySerializedAs("is_walking")] public bool isWalking;

		[FormerlySerializedAs("is_fighting")] public bool isFighting;

		[FormerlySerializedAs("is_casting")] public bool isCasting;

		bool is_victory;

		string state;

		// Start is called before the first frame update
		void Start() {
			state = "idle";

			sr = GetComponent<SpriteRenderer>();

			sr.sprite = idle;
		}

		void Update() {
			if (state == "idle") {
				if (sr.sprite != idle)
					sr.sprite = idle;
			}
		}

		public void change_state(string st, WeaponSprite ws = null) {
			state = st;
			switch (st) {
				case "idle":
					sr.sprite = idle;
					break;
				case "walk":
					StartCoroutine(Walk());
					break;
				case "fight":
					StartCoroutine(Fight(ws));
					break;
				case "magic":
					StartCoroutine(Cast());
					break;
				case "tired":
					sr.sprite = tired;
					break;
				case "stone":
					sr.sprite = stone;
					break;
				case "dead":
					sr.sprite = dead;
					//GetComponent<PartyMember>().move_point = new Vector3(sr.gameObject.transform.position.x - .66f, sr.gameObject.transform.position.y, sr.gameObject.transform.position.z);
					//sr.gameObject.transform.position = new Vector3(sr.gameObject.transform.position.x - .66f, sr.gameObject.transform.position.y, sr.gameObject.transform.position.z);
					break;
				case "run":
					sr.sprite = idle;
					sr.flipX = true;
					break;
				case "victory":
					sr.sprite = idle;
					StartCoroutine(Victory());
					break;
			}
		}

		public string get_state() {
			return state;
		}
		public IEnumerator Walk() {
			isWalking = true;
			float wait = 0.071657625f;

			sr.sprite = walking1;
			yield return new WaitForSeconds(wait);
			sr.sprite = walking2;
			yield return new WaitForSeconds(wait);
			sr.sprite = walking1;
			yield return new WaitForSeconds(wait);
			sr.sprite = walking2;
			yield return new WaitForSeconds(wait);

			isWalking = false;

			yield return null;
		}
		public IEnumerator Victory() {
			float wait = .25f;
			is_victory = true;

			for (int i = 0; i < 5; i++) {
				sr.sprite = magicVictory1;
				yield return new WaitForSeconds(wait);
				sr.sprite = magicVictory2;
				yield return new WaitForSeconds(wait);
			}

			sr.sprite = idle;

			is_victory = false;

			yield return null;
		}
		public IEnumerator Fight(WeaponSprite ws) {

			if (ws == null) {
				isFighting = true;
				float wait = 0.071657625f;

				sr.sprite = attack1;
				yield return new WaitForSeconds(wait);
				sr.sprite = attack2;
				yield return new WaitForSeconds(wait);
				sr.sprite = attack1;
				yield return new WaitForSeconds(wait);
				sr.sprite = attack2;
				yield return new WaitForSeconds(wait);

			}
			else {
				isFighting = true;
				float wait = 0.071657625f;

				ws.Show();

				ws.go_forward();
				sr.sprite = attack1;
				yield return new WaitForSeconds(wait);
				ws.go_back();
				sr.sprite = attack2;
				yield return new WaitForSeconds(wait);
				ws.go_forward();
				sr.sprite = attack1;
				yield return new WaitForSeconds(wait);
				ws.go_back();
				sr.sprite = attack2;
				yield return new WaitForSeconds(wait);

				ws.Hide();
			}

			isFighting = false;

			yield return null;
		}
		public IEnumerator Cast() {
			isCasting = true;
			Debug.Log("casting");
			isCasting = false;

			yield return null;
		}

		/*
	public bool walk_animation;

	public IEnumerator walk()
	{
		float wait = 0.13189315f;
		walk_animation = true;
		switch (direction)
		{
		    case "up":
		        sr.sprite = active_character.up1;
		        yield return new WaitForSeconds(wait);
		        sr.sprite = active_character.up2;
		        yield return new WaitForSeconds(wait);
		        break;
		    case "down":
		        sr.sprite = active_character.down1;
		        yield return new WaitForSeconds(wait);
		        sr.sprite = active_character.down2;
		        yield return new WaitForSeconds(wait);
		        break;
		    case "left":
		        sr.flipX = false;
		        sr.sprite = active_character.step1;
		        yield return new WaitForSeconds(wait);
		        sr.flipX = false;
		        sr.sprite = active_character.step2;
		        yield return new WaitForSeconds(wait);
		        break;
		    case "right":
		        sr.flipX = true;
		        sr.sprite = active_character.step1;
		        yield return new WaitForSeconds(wait);
		        sr.sprite = active_character.step2;
		        yield return new WaitForSeconds(wait);
		        break;
		}
		walk_animation = false;
	}
	*/
	}
}
