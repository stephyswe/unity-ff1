using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Utils.SaveGame.Scripts.SaveSystem;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Local

namespace Tests.PlayTests {
    public class SceneSetupTests
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlags dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [Flags]
        enum MouseEventFlags
        {
            Leftdown = 0x00000002,
            Leftup = 0x00000004,
            Middledown = 0x00000020,
            Middleup = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            Rightdown = 0x00000008,
            RIGHTUP = 0x00000010
        }

        //This simulates a left mouse click
        static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MouseEventFlags.Leftdown, xpos, ypos, 0, 0);
            mouse_event(MouseEventFlags.Leftup, xpos, ypos, 0, 0);
        }
        
        
        [UnityTest]
        public IEnumerator MainScene_LoadsCorrectlyAndItsDaytime()
        {
            SceneManager.LoadScene("Assets/Scenes/Title Screen.unity", LoadSceneMode.Single);
            
            yield return null;

            GameObject eventSystem = GameObject.Find("Title");

            Assert.IsTrue(eventSystem != null, "should find the 'Title' object in the scene");
            
            // Find x and y position of an object
            yield return new WaitForSeconds(3);
            GameObject obj = GameObject.Find("New Game");
            GameObject obj2 = GameObject.Find("Settings");
            GameObject obj3 = GameObject.Find("Continue");
            (int xValue, int yValue) = GetBothPos(obj);
            (int xValue2, int yValue2) = GetBothPos(obj2);
            (int xValue3, int yValue3) = GetBothPos(obj3);
            LeftMouseClick( (int)xValue, (int)yValue);
            yield return new WaitForSeconds(1);
            LeftMouseClick( 1020, 500);
            yield return new WaitForSeconds(10);
            
            // Load new Screen
            //GameObject eventSystem2 = GameObject.Find("Overworld");
            //Assert.IsTrue(eventSystem != null, "should find the 'Overworld' object in the scene");
            
            // Please.
            DataState dateState = SaveSystem.LoadAndReturn();
            
            // Assert that the dataState is not null
            Assert.IsTrue(dateState != null, "should find the 'saveData");
            
            // Get JSON save file
            //string json1 = File.ReadAllText("./Assets/Tests/PlayTests/CONST-SAVEDATA.json");
            //Debug.Log(json1.Length);
            
            // Turn JSON into object
            DataState dataState2 = LoadBinary("./Assets/Tests/PlayTests/CONST-SAVEDATA.json");

            // ** Compare dataState2 and dateState **
            
            // Debug datastate2 and datastate
            Debug.Log("DataState");
            Debug.Log("DataState2");
            // remove key "reh_seed"
            //dateState.items.RemoveAt(2);
            //dataState2.items.RemoveAt(2);
            
            Assert.IsTrue(dateState == dataState2, "should be same data");
            
            yield return new WaitForSeconds(10);
        }
        
        public static DataState LoadBinary(string dataPath)
        {
            string jsonData = File.ReadAllText(dataPath);
            DataState state = JsonConvert.DeserializeObject<DataState>(jsonData);
            return state;
        }

        static (int x, int y) GetBothPos(GameObject obj) {
            Vector3 position = obj.transform.localPosition;
            int x = GetXPosOnScreen(position.x, -1293, 985);
            int y = GetXPosOnScreen(position.y, -730, 420);
            return (x, y);
        }

        static int GetXPosOnScreen(float posValue, int oGt, int oGn) {
            const int diff = 47;
            float diff1 = Math.Abs(oGt - posValue);
            double diff2 = diff1 * diff;
            double diff3 = diff2 + oGn;
            
            int x = (int)diff3;
            return x;
        }
    }
}