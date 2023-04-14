using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TitleScreen {
	public class ControlsHandler : MonoBehaviour {
		Text buttonText;

		Event keyEvent;
		KeyCode newKey;

		bool waitingForKey;


		void Update() {}


		void OnEnable() {
			//Assign menuPanel to the Panel object in our Canvas
			//Make sure it's not active when the game starts
			waitingForKey = false;

			/*iterate through each child of the panel and check
		 * the names of each one. Each if statement will
		 * set each button's text component to display
		 * the name of the key that is associated
		 * with each command. Example: the ForwardKey
		 * button will display "W" in the middle of it
		 */

			Debug.Log(CustomInputManager.Cim.Down.ToString());

			Text[] buttonTexts = GetComponentsInChildren<Text>();

			for (int i = 0; i < buttonTexts.Length; i++) {

				switch (buttonTexts[i].gameObject.name) {
					case "upkey":
						buttonTexts[i].text = CustomInputManager.Cim.Up.ToString().ToUpper();
						break;
					case "downkey":
						buttonTexts[i].text = CustomInputManager.Cim.Down.ToString().ToUpper();
						break;
					case "leftkey":
						buttonTexts[i].text = CustomInputManager.Cim.Left.ToString().ToUpper();
						break;
					case "rightkey":
						buttonTexts[i].text = CustomInputManager.Cim.Right.ToString().ToUpper();
						break;
					case "backkey":
						buttonTexts[i].text = CustomInputManager.Cim.Back.ToString().ToUpper();
						break;
					case "selectkey":
						buttonTexts[i].text = CustomInputManager.Cim.Select.ToString().ToUpper();
						break;
				}
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
			if (keyEvent.isKey && waitingForKey) {
				newKey = keyEvent.keyCode; //Assigns newKey to the key user presses
				waitingForKey = false;
			}
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
		public IEnumerator AssignKey(string keyName) {
			waitingForKey = true;

			yield return WaitForKey(); //Executes endlessly until user presses a key

			switch (keyName) {
				case "up":
					CustomInputManager.Cim.Up = newKey; //Set forward to new keycode
					buttonText.text = CustomInputManager.Cim.Up.ToString().ToUpper(); //Set button text to new key
					PlayerPrefs.SetString("upkey", CustomInputManager.Cim.Up.ToString()); //save new key to PlayerPrefs
					break;
				case "down":
					CustomInputManager.Cim.Down = newKey; //set backward to new keycode
					buttonText.text = CustomInputManager.Cim.Down.ToString().ToUpper(); //set button text to new key
					PlayerPrefs.SetString("downkey", CustomInputManager.Cim.Down.ToString()); //save new key to PlayerPrefs
					break;
				case "left":
					CustomInputManager.Cim.Left = newKey; //set left to new keycode
					buttonText.text = CustomInputManager.Cim.Left.ToString().ToUpper(); //set button text to new key
					PlayerPrefs.SetString("leftkey", CustomInputManager.Cim.Left.ToString()); //save new key to playerprefs
					break;
				case "right":
					CustomInputManager.Cim.Right = newKey; //set right to new keycode
					buttonText.text = CustomInputManager.Cim.Right.ToString().ToUpper(); //set button text to new key
					PlayerPrefs.SetString("rightkey", CustomInputManager.Cim.Right.ToString()); //save new key to playerprefs
					break;
				case "back":
					CustomInputManager.Cim.Back = newKey; //set jump to new keycode
					buttonText.text = CustomInputManager.Cim.Back.ToString().ToUpper(); //set button text to new key
					PlayerPrefs.SetString("backkey", CustomInputManager.Cim.Back.ToString()); //save new key to playerprefs
					break;
				case "select":
					CustomInputManager.Cim.Select = newKey; //set jump to new keycode
					buttonText.text = CustomInputManager.Cim.Select.ToString().ToUpper(); //set button text to new key
					PlayerPrefs.SetString("selectkey", CustomInputManager.Cim.Select.ToString()); //save new key to playerprefs
					break;
			}

			yield return null;
		}
	}
}
