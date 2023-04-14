using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Battling {
	public class CursorController : MonoBehaviour {
		[FormerlySerializedAs("monster_mode")] public bool monsterMode;
		[FormerlySerializedAs("shop_mode")] public bool shopMode;
		[FormerlySerializedAs("buy_cursor_mode")]
		public bool buyCursorMode;

		[FormerlySerializedAs("event_system")] public EventSystem eventSystem;

		public GameObject[] buttons;
		public GameObject[] monsters;

		public int active;

		public int frame;

		GameObject[] active_array;

		List<GameObject> active_list;

		int frames_since_select;

		// Update is called once per frame
		void Update() {

			frames_since_select += 1;

			if (Input.GetKeyDown(CustomInputManager.Cim.Select) && frames_since_select > 20) {
				if (!monsterMode)
					buttons[active].GetComponent<Button>().onClick.Invoke();
				frames_since_select = 0;
			}

			if (active < active_list.Count && active >= 0)
				eventSystem.SetSelectedGameObject(active_list[active]);

			frame = frame + 1;
			if (frame >= 15) {

				bool up = Input.GetKey(CustomInputManager.Cim.Up);
				bool down = Input.GetKey(CustomInputManager.Cim.Down);

				float ver = 0f;

				if (up)
					ver = 1f;
				if (down)
					ver = -1f;

				if (ver == 1f) {
					active = active - 1;

					if (active < 0)
						active = active_list.Count - 1;

					while (monsterMode && get_monster().hp <= 0 || !monsterMode && active_list[active].activeSelf == false) {
						active -= 1;

						if (active < 0)
							active = active_list.Count - 1;
					}

					frame = 0;
				}

				else if (ver == -1f) {
					active = active + 1;

					if (active >= active_list.Count)
						active = 0;

					while (monsterMode && get_monster().hp <= 0 || !monsterMode && active_list[active].activeSelf == false) {
						active += 1;

						if (active >= active_list.Count)
							active = 0;
					}

					frame = 0;
				}

				else
					frame = 35;

				Move();
			}
		}

		// Start is called before the first frame update
		void OnEnable() {
			GetComponent<SpriteRenderer>().enabled = false;

			active = 0;
			if (monsterMode) {
				active_array = monsters;
				buttons = monsters;

				for (int i = buttons.Length - 1; i > -1; i--) {
					if (buttons[i].GetComponent<Monster>().hp > 0)
						active = i;
				}
			}
			else
				active_array = buttons;

			active_list = active_array.OfType<GameObject>().ToList();

			for (int i = 0; i < buttons.Length; i++) {
				if (buttons[i].activeSelf == false)
					remove_from_list(buttons[i]);
			}

			active_list = active_array.OfType<GameObject>().ToList();

			while (monsterMode && get_monster().hp <= 0 || !monsterMode && active_list[active].activeSelf == false) {
				active += 1;

				if (active >= active_list.Count)
					active = 0;
			}

			Move();
			GetComponent<SpriteRenderer>().enabled = true;
		}

		void Move() {
			float xoffset = 0f;
			float yoffset = 0f;

			if (monsterMode) {
				xoffset = -3.5f;
				yoffset = -0.4f;
			}
			else if (shopMode && buyCursorMode) {
				xoffset = -.45f;
				yoffset = 0f;
			}
			else if (shopMode) {
				xoffset = -2.725f;
				yoffset = -1.05f;
			}
			else {
				xoffset = -4.5f;
				yoffset = -0.4f;
			}

			transform.position = new Vector3(active_list[active].transform.position.x + xoffset, active_list[active].transform.position.y + yoffset, 1f);
		}

		public void remove_from_list(GameObject obj) {
			if (monsterMode) {
				for (int i = buttons.Length - 1; i > -1; i--) {
					if (buttons[i].GetComponent<Monster>().hp > 0)
						active = i;
				}
			}
			else
				active = 0;
			active_list.Remove(obj);
		}

		public Monster get_monster() {
			return active_list[active].GetComponent<Monster>();
		}

		public Button get_button() {
			return buttons[active].GetComponent<Button>();
		}
	}
}
