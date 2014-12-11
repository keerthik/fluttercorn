using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Director : MonoBehaviour {
	
	private List<GameObject> levelElements = new List<GameObject>();
	private List<float> elementTimes = new List<float>();
	private float right = 17.0f;
	private float left = -19.0f;
	
	public float levelSpeed = 0.5f;
	
	public GameObject BG1, BG2, floor;
	public GameObject dashable;
	public GameObject GUIbg;
	public CameraController cam;
	public CharController pony;
	
	public GUIText textBlob;
	
	public enum Element{
		CHANGER,
		MEANNICE,
		EVENTS
	};
	public static float normalSpeed = 0.5f;
	
	public void SetSpeed(float speed) {
		if (speed == 0)
			levelSpeed = normalSpeed;
		else
			levelSpeed = speed;
		floor.GetComponent<BackgroundScroller>().speed = levelSpeed;
		BG1.GetComponent<BackgroundScroller>().speed = levelSpeed*.75f;
		BG2.GetComponent<BackgroundScroller>().speed = levelSpeed*.15f;
	}
	
	int crashCounter = 0;
	int dashCounter = 0;
	public void ShowHitMessage(FaceController face) {
		string message;
		if ((pony.GetCurrentState() & CharController.State.BURST) == CharController.State.BURST)  {
			message = dashEffects[dashCounter];
			dashCounter = Mathf.Min (dashEffects.Length-1, ++dashCounter);
			//GUIbg.renderer.material.SetColor("_TintColor", Color.blue);
			StartCoroutine(fadeToFrom(Color.black, Color.blue));
			face.Explode(true);
			pony.Explode();
		} else {
			message = crashEffects[crashCounter];
			cam.Shake();
			face.Explode(true);
			crashCounter = Mathf.Min (crashEffects.Length-1, ++crashCounter);
			StartCoroutine(fadeToFrom(Color.black, Color.red));
			//GUIbg.renderer.material.SetColor("_TintColor", Color.red);
		}
		if (dashCounter+crashCounter >= crashEffects.Length + dashEffects.Length - 2) {
			ended = true;
			message = "That's all I got for you, Chen! :)";
		}
		textBlob.text = message;
	}
	
	private IEnumerator fadeToFrom(Color a, Color b) {
		float completion = 0f;
		while (completion < 0.8f) {
			completion += Time.deltaTime;
			Color c = Color.Lerp(a, b, completion);
			GUIbg.renderer.material.SetColor("_TintColor", c);
			yield return null;
		}
		// Return
		completion = 0.2f;
		while (completion < 1.0f) {
			completion += Time.deltaTime;
			Color c = Color.Lerp(b, a, completion);
			GUIbg.renderer.material.SetColor("_TintColor", c);
			yield return null;
		}
		yield return null;
	}
	void Start () {
		Init ();
		//print(GUIbg.renderer.material.GetColor("_TintColor") );
	}

	
	public static bool gamePaused = false;
	public static bool ended = false;
	public GUIText credits;
	void Update () {
		SpawnManager();
		if (Input.GetKeyUp(KeyCode.Space)) {
			Time.timeScale = 1.0f - Time.timeScale;
			gamePaused = !gamePaused;
			credits.text = (gamePaused||ended)?"Credits:\nAll game development: Keerthik\n" +
							"Inspiration for raw material: Roydan\nA friend worth doing this for: CHYENN"
							:"Press space to pause";
			if (gamePaused) textBlob.text = "Press Space to upause the game";
			else textBlob.text = "Z = Dash\nX = Jump";
		}
		/*
		// Control game pacing
		if (time > elementTimes[(int)(elementTimes.Count/2)])
			normalSpeed = 0.750f;
		*/
	}
		
	void OnGUI() {
		if (gamePaused) {
			if (GUI.Button(RelRect(0.3f, 0.7f, 0.15f, 0.05f), "Change Music")) {
				MusicController.NextTrack();
			}
			if (GUI.Button(RelRect(0.55f, 0.7f, 0.15f, 0.05f), "Rewatch Intro")) {
				Application.LoadLevel("IntroScene");
			}
		}
	}

	void Init() {
		// Set parameters
		levelSpeed = 0.5f;
		normalSpeed = 0.5f;
		levelElements = new List<GameObject>();
		elementTimes = new List<float>();
		for (int i = 0; i < crashEffects.Length+dashEffects.Length; i++) {
			int thisY = Random.Range(1, 3);
			levelElements.Add(Instantiate(dashable, new Vector3(30, floor.transform.position.y + 4 + 1.5f*thisY, pony.transform.position.z), Quaternion.identity) as GameObject);
			levelElements[i].GetComponent<FaceController>().director = this;
			elementTimes.Add(4.0f + i*(3.5f + 0.1f*i));
		}
	}
		
	private float time = 0.0f;
	private void SpawnManager() {
		time += levelSpeed*Time.deltaTime;
		// Manage adding new elements for leftover
		if (elementTimes.Count < dashEffects.Length + crashEffects.Length - dashCounter - crashCounter) {
			int thisY = Random.Range(1, 3);
			levelElements.Add(Instantiate(dashable, new Vector3(30, floor.transform.position.y + 4 + 1.5f*thisY, pony.transform.position.z), Quaternion.identity) as GameObject);
			levelElements[levelElements.Count-1].GetComponent<FaceController>().director = this;
			elementTimes.Add(elementTimes[elementTimes.Count-1] + 5f);
		}
		// Manage spawning level Elements into game
		for (int i=0; i<elementTimes.Count; i++) {
			float timePos = time - elementTimes[i];
			if (timePos > 0) {
				GameObject element = levelElements[i];
				element.transform.position = new Vector3(Mathf.Lerp(right, left, 0.5f*timePos), element.transform.position.y, pony.transform.position.z);
				if (element.transform.position.x < left/2f) {
					// dodge-effect
				}
				if (element.transform.position.x < left+.01f) {
					Destroy(element);
					levelElements.RemoveAt(i);
					elementTimes.RemoveAt(i);
				}
			}
		}
	}
	
	
	private Rect RelRect(float left, float top, float width, float height) {
		return new Rect(left*Screen.width, top*Screen.height, width*Screen.width, height*Screen.height);
	}
	
	private string [] dashEffects = {
			"You have forgiven me for all the mean things\n I've said to you (right?) ^_^\n" +
				"[ Keep dashing through for me to not be mean ]",
			"You are the first person who bought me a gift\n I always wanted [Oppa Gundam Style]",
			"You were always a good friend to talk to \nwhen I was lonely and stuff",
			"You are the only person I remembered to get\n anything(Halwa) for, even when \n" +
				"I forgot to get anyone else stuff",
			"I had a blast all the time we hung out\nwinter 2012! Quality single friends' time :) Thanks!",
			"I just wanted to let you know, even when\nyou thought you were being whiny\n" +
				"and sad, I was happy to talk to and\nhang out with you, and I hope \nI made you feel better~",
			"You made it this far! I hope we can be\nthere for each other always!",
			"Congratulations!!!\n You have just experienced the \nnicest thing I've done/made for anyone :)",
			"This game took a total of around 75 hours for me to make\n - about 3 months of bart rides :O\n" +
				"I hope you enjoyed it :)\n[ Feel free to keep playing \nto see the rest of the mean stuff ]",
	};
	private string [] crashEffects = {
		"I enjoyed making you throw a shoe at Roydan ^_^\n\n" +
			"[ Dash through these for nicer things, \n" +
			"otherwise I'll say mean things ]",
		"Mean as it was, that thing I said \ncomparing your writing papers to \n" +
			"making relationships work was darn straight clever,\n" +
			"right?",
		"You're a jerk with how terribly you respond on IM",
		"I found this on your facebook wall from me:\nYou're fugly\n -Albert",
		"Oh that was when QED took over your wall for a night",
		"Shortly before QED issued you an infraction notice teehee >:D",
		"You dropped comparch in sophomore year \nand we gave you much crap about it :D",
		"YOU'RE AS CRAZY AS ROYDAN SOMETIMES",
		"Damn I should have more mean things to say to you...",
		"Oh right. You're fat. Hehe. Moo",
		"Gee, stop running into them *dash* through them~",
		"Ok seriously, figure out the game already, and stop getting hit\n" +
			"...unless you already finished it and just want to read the mean stuff :D",
	};
}