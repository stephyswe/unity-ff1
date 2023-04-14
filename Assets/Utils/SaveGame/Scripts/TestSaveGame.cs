using UnityEngine;
using UnityEngine.UI;

namespace Utils.SaveGame.Scripts {
    public class TestSaveGame : MonoBehaviour
    {
        //INT (UI)
        [Header("Save int")]
        public Text countIntText;
        public InputField inputIntField;
        public int cubeIntCount = 0;
        [Space(10)]

        //Next variables
        [Header("Save next")]
        public float floatCount;
        public Vector2 vect2;
        public Vector3 vect3;
        public Color color;
        public string stringSave;
        public bool saveBool;




        // Use this for initialization
        private void Start()
        {

            //Load Save int
            cubeIntCount = SaveSystem.SaveSystem.GetInt("cubeCount");
            countIntText.text = cubeIntCount.ToString();

            //Load save Next
            floatCount = SaveSystem.SaveSystem.GetFloat("float");
            saveBool = SaveSystem.SaveSystem.GetBool("bool");
            vect2 = SaveSystem.SaveSystem.GetVector2("vect2");
            vect3 = SaveSystem.SaveSystem.GetVector3("vect3");
            color = SaveSystem.SaveSystem.GetColor("color");
            stringSave = SaveSystem.SaveSystem.GetString("string");



        }


        //Button Save INT
        public void SaveCube()
        {
            countIntText.text = inputIntField.text;
            cubeIntCount = int.Parse(inputIntField.text);

            //Save "cubeCount"
            SaveSystem.SaveSystem.SetInt("cubeCount", cubeIntCount);
        }

        //Save "NEXT"
        private void OnApplicationQuit()
        {
        
            SaveSystem.SaveSystem.SetFloat("float", floatCount);
            SaveSystem.SaveSystem.SetBool("bool", saveBool);
            SaveSystem.SaveSystem.SetVector2("vect2", vect2);
            SaveSystem.SaveSystem.SetVector3("vect3", vect3);
            SaveSystem.SaveSystem.SetColor("color", color);
            SaveSystem.SaveSystem.SetString("string", stringSave);
        }

        //Save "NEXT"
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveSystem.SaveSystem.SetFloat("float", floatCount);
                SaveSystem.SaveSystem.SetBool("bool", saveBool);
                SaveSystem.SaveSystem.SetVector2("vect2", vect2);
                SaveSystem.SaveSystem.SetVector3("vect3", vect3);
                SaveSystem.SaveSystem.SetColor("color", color);
                SaveSystem.SaveSystem.SetString("string", stringSave);
            }
        }

    }
}
