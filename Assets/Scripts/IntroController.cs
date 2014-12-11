using UnityEngine;
using System.Collections;

public class IntroController : MonoBehaviour {
	
	public GUIText textbox;
	public GameObject [] sceneParts;
	public CameraController cam;
	string [] message = {
		"To the one and only Chen Wang...",
		"Wait, there's probably like...\n" +
			"half a million Chen Wangs out there...",
		"But anyway, this is a game I made for you",
		"I hope you enjoy it!",
	};
	int mindex = 0;
	string currentText = "";
	void Start () {
		currentText = message[0];
	}
	bool textDisplayed = false;
	float timer = 0f;
	float clicktime = 0.05f;
	int displayedLength = 0;
	void UpdateText() {
		if (textDisplayed) return;
		if (timer == 0) {
			displayedLength++;
		}
		// Timer
		timer += Time.deltaTime;
		if (timer > clicktime) timer = 0;
		textbox.text = currentText.Substring(0, displayedLength);
		if (displayedLength == currentText.Length) {
			displayedLength = 0;
			textDisplayed = true;
		}
	}
	void Update () {
		UpdateText();
		if (Input.GetKeyUp(KeyCode.Space)) {
			Application.LoadLevel("Gameplay");
		}
		if (Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.Z)) {
			if (mindex < message.Length-1) {
				mindex++;
				currentText = message[mindex];
				sceneParts[mindex-1].SetActiveRecursively(true);
				cam.Shake();
				textbox.text = "";
				textDisplayed = false;
				textbox.transform.Translate(new Vector3(0.1f, -0.1f, 0f));
			} else Application.LoadLevel("Gameplay");
		}
	}
}
