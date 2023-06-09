using UnityEngine;

namespace Utils.BattleTransitions {
	[ExecuteInEditMode]
	public class ScreenTransition : MonoBehaviour {

		public Material mat;

		float fill_value;
		public bool filling;

		// ReSharper disable once IdentifierTypo
		public bool unfilling;

		int wait_frames;
		bool wait;
		static readonly int Cutoff = Shader.PropertyToID("_Cutoff");

		void Awake() {
			mat.SetFloat(Cutoff, 0f);
			wait_frames = 0;
			wait = false;
		}

		void Update() {

			wait_frames += 1;

			if (wait_frames >= 30) {
				wait_frames = 35;
			}

			if (filling) {
				fill_value = Mathf.Lerp(mat.GetFloat(Cutoff), 1f, 8 * Time.deltaTime);
				mat.SetFloat(Cutoff, fill_value);
				if (fill_value >= .98f) {
					filling = false;
					mat.SetFloat(Cutoff, 1f);
				}
			}
			if (unfilling && wait_frames >= 30) {
				fill_value = Mathf.Lerp(mat.GetFloat(Cutoff), 0f, 4 * Time.deltaTime);
				mat.SetFloat(Cutoff, fill_value);
				if (fill_value <= .02f) {
					unfilling = false;
					wait_frames = 0;
					wait = false;

					mat.SetFloat(Cutoff, 0f);
				}
			}
			if (!(fill_value >= .98f) || wait)
				return;
			// ReSharper disable once CommentTypo
			// unfilling = true;
			filling = false;

			wait = true;
			wait_frames = 0;
		}

		public void Transition() {
			filling = true;
		}

		void OnRenderImage(RenderTexture src, RenderTexture dst) {
			if (mat != null) {
				Graphics.Blit(src, dst, mat);
			}
		}
	}
}
