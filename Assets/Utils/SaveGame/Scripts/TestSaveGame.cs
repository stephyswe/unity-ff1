using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Utils.SaveGame.Scripts {
	public class TestSaveGame : MonoBehaviour {
		//INT (UI)
		[Header("Save int")]
		public Text countIntText;
		public InputField inputIntField;
		public int cubeIntCount = 0;
		[Space(10)]

		//Next variables
		[Header("Save next")]
		public float floatCount;
		[FormerlySerializedAs("vect2")] public Vector2 vector2;
		[FormerlySerializedAs("vect3")] public Vector3 vector3;
		public Color color;
		public string stringSave;
		public bool saveBool;




		// Use this for initialization
		void Start() {
			//Load Save int
			cubeIntCount = SaveSystem.SaveSystem.GetInt("cubeCount");
			countIntText.text = cubeIntCount.ToString();

			//Load save Next
			floatCount = SaveSystem.SaveSystem.GetFloat("float");
			saveBool = SaveSystem.SaveSystem.GetBool("bool");
			vector2 = SaveSystem.SaveSystem.GetVector2("vector2");
			vector3 = SaveSystem.SaveSystem.GetVector3("vector3");
			color = SaveSystem.SaveSystem.GetColor("color");
			stringSave = SaveSystem.SaveSystem.GetString("string");
		}


		//Button Save INT
		public void SaveCube() {
			countIntText.text = inputIntField.text;
			cubeIntCount = int.Parse(inputIntField.text);

			//Save "cubeCount"
			SaveSystem.SaveSystem.SetInt("cubeCount", cubeIntCount);
		}

		//Save "NEXT"
		void OnApplicationQuit() {
			SaveSystem.SaveSystem.SetFloat("float", floatCount);
			SaveSystem.SaveSystem.SetBool("bool", saveBool);
			SaveSystem.SaveSystem.SetVector2("vector2", vector2);
			SaveSystem.SaveSystem.SetVector3("vector3", vector3);
			SaveSystem.SaveSystem.SetColor("color", color);
			SaveSystem.SaveSystem.SetString("string", stringSave);
		}

		//Save "NEXT"
		void OnApplicationPause(bool pause) {
			if (!pause)
				return;
			SaveSystem.SaveSystem.SetFloat("float", floatCount);
			SaveSystem.SaveSystem.SetBool("bool", saveBool);
			SaveSystem.SaveSystem.SetVector2("vector2", vector2);
			SaveSystem.SaveSystem.SetVector3("vector3", vector3);
			SaveSystem.SaveSystem.SetColor("color", color);
			SaveSystem.SaveSystem.SetString("string", stringSave);
		}
	}
}
