using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Overworld {
	public class FadeOut : MonoBehaviour {

		[FormerlySerializedAs("set_color")] public Color setColor;

		Image fade_img;

		bool fade_in;
		bool fade_out;

		int frame;

		// Start is called before the first frame update
		void Start() {
			fade_img = GetComponentInChildren<Image>();
			fade_in = false;
			fade_out = false;
			setColor = new Color32(0, 0, 0, 0);
		}

		// Update is called once per frame
		void Update() {

			frame += 1;

			if (fade_out && can_fade()) {
				fade_img.color = setColor;
				setColor.a = setColor.a + .01f;

				if (setColor.a >= 1f) {
					fade_out = false;
					setColor.a = 1f;
				}
			}
			else if (fade_in && can_fade()) {
				fade_img.color = setColor;
				setColor.a = setColor.a - .01f;

				if (setColor.a <= 0f) {
					fade_out = false;
					setColor.a = 0f;
				}
			}
		}

		public void start_fade(bool toBlack) {
			if (toBlack) {
				setColor.a = 0;
				fade_out = true;
				fade_in = false;
			}
			else {
				setColor.a = 1;
				fade_in = true;
				fade_out = false;
			}
		}

		bool can_fade() {
			int n = frame % 10;
			int[] allowed = {1, 3, 4, 6, 7, 8, 9};
			return allowed.Contains(n);
		}

		public bool is_fading() {
			return fade_out || fade_in;
		}
	}
}
