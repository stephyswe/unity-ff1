using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TitleScreen {
	public class ControlsHandler : MonoBehaviour {
		Text buttonText;
		Event keyEvent;
		KeyCode newKey;
		bool waitingForKey;

		void OnEnable() {
			//Assign menuPanel to the Panel object in our Canvas
			//Make sure it's not active when the game starts
			waitingForKey = false;


			Debug.Log("ControlsHandler: " +CustomInputManager.Cim.Down.ToString());

			// Get all the buttons
			Text[] buttonTexts = GetComponentsInChildren<Text>();

			// Iterate through all the buttons
			foreach (Text t in buttonTexts) {
				t.text = t.gameObject.name switch {
					"upkey" => CustomInputManager.Cim.Up.ToString().ToUpper(),
					"downkey" => CustomInputManager.Cim.Down.ToString().ToUpper(),
					"leftkey" => CustomInputManager.Cim.Left.ToString().ToUpper(),
					"rightkey" => CustomInputManager.Cim.Right.ToString().ToUpper(),
					"backkey" => CustomInputManager.Cim.Back.ToString().ToUpper(),
					"selectkey" => CustomInputManager.Cim.Select.ToString().ToUpper(),
					_ => t.text
				};
			}
		}

		void OnGUI() {
			/*keyEvent dictates what key our user presses
		 * bt using Event.current to detect the current
		 * event
		 */
			keyEvent = Event.current;

			//Executes if a button gets pressed and
			//the user presses a key
			if (!keyEvent.isKey || !waitingForKey)
				return;
			newKey = keyEvent.keyCode; //Assigns newKey to the key user presses
			waitingForKey = false;
		}

		/*Buttons cannot call on Coroutines via OnClick().
	 * Instead, we have it call StartAssignment, which will
	 * call a coroutine in this script instead, only if we
	 * are not already waiting for a key to be pressed.
	 */
		public void StartAssignment(string keyName) {
			if (!waitingForKey)
				StartCoroutine(AssignKey(keyName));
		}

		//Assigns buttonText to the text component of
		//the button that was pressed
		public void SendText(Text text) {
			buttonText = text;
		}

		//Used for controlling the flow of our below Coroutine
		IEnumerator WaitForKey() {
			while (!keyEvent.isKey)
				yield return null;
		}

		/*AssignKey takes a keyName as a parameter. The
	 * keyName is checked in a switch statement. Each
	 * case assigns the command that keyName represents
	 * to the new key that the user presses, which is grabbed
	 * in the OnGUI() function, above.
	 */
		IEnumerator AssignKey(string keyName) {
			waitingForKey = true;

			yield return WaitForKey(); //Executes endlessly until user presses a key

			if (keyName == "up")
				CustomInputManager.Cim.Up = newKey;
			else if (keyName == "down")
				CustomInputManager.Cim.Down = newKey;
			else if (keyName == "left")
				CustomInputManager.Cim.Left = newKey;
			else if (keyName == "right")
				CustomInputManager.Cim.Right = newKey;
			else if (keyName == "back")
				CustomInputManager.Cim.Back = newKey;
			else if (keyName == "select")
				CustomInputManager.Cim.Select = newKey;
			
			//set button text to new key
			buttonText.text = newKey.ToString().ToUpper(); //set button text to new key
			
			// save new key to player prefs
			PlayerPrefs.SetString(keyName + "key", newKey.ToString()); 

			yield return null;
		}
	}
}
