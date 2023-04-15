using UnityEngine;

namespace Utils.SaveGame.Scripts {
	public class SaveSystemSetup : MonoBehaviour {

		[SerializeField] string fileName = "Profile.json"; // file to save with the specified resolution
		[SerializeField] bool dontDestroyOnLoad; // the object will move from one scene to another (you only need to add it once)

		void Awake()
		{
			SaveSystem.SaveSystem.Initialize(fileName);
			if(dontDestroyOnLoad) DontDestroyOnLoad(transform.gameObject);
		}
	}
}
