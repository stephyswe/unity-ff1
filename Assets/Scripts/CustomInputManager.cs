using System;
using UnityEngine;

public class CustomInputManager : MonoBehaviour {

	public static CustomInputManager Cim;

	public KeyCode Up { get; set; }
	public KeyCode Down { get; set; }
	public KeyCode Left { get; set; }
	public KeyCode Right { get; set; }
	public KeyCode Back { get; set; }
	public KeyCode Select { get; set; }

	// Start is called before the first frame update
	void Awake() {
		if (Cim == null) {
			DontDestroyOnLoad(gameObject);
			Cim = this;
		}
		else if (Cim != this)
			Destroy(gameObject);

		Up = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("upkey", "W"));
		Down = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("downkey", "S"));
		Left = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftkey", "A"));
		Right = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightkey", "D"));
		Back = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("backkey", "O"));
		Select = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("selectkey", "P"));
	}
}
