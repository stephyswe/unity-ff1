using UnityEngine;
using Utils.SaveGame.Scripts.SaveSystem;

public class MusicHandler : MonoBehaviour {

	public AudioSource classic;
	public AudioSource remaster;
	public AudioSource gba;

	// Start is called before the first frame update
	void Start() {
		if (SaveSystem.GetBool("classic_music")) {
			classic.volume = 1f;
			remaster.gameObject.SetActive(false);
			gba.gameObject.SetActive(false);
		}
		else if (SaveSystem.GetBool("remaster_music")) {
			remaster.volume = 1f;
			classic.gameObject.SetActive(false);
			gba.gameObject.SetActive(false);
		}
		else {
			gba.volume = 1f;
			classic.gameObject.SetActive(false);
			remaster.gameObject.SetActive(false);
		}
	}

	public AudioSource get_active() {
		if (classic.gameObject.activeSelf)
			return classic;
		return gba.gameObject.activeSelf ? gba : remaster;
	}
}
