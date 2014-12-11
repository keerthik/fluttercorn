using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {
	
	public static GameObject controller = null;
	public AudioClip [] Music;
	private static int musicTrack = 0;

	public static void NextTrack() {
		MusicController musician = controller.GetComponent<MusicController>();
		musicTrack = (musicTrack < musician.Music.Length-1)? musicTrack+1:0;
		controller.GetComponent<AudioSource>().Stop();
		controller.GetComponent<AudioSource>().clip = musician.Music[musicTrack];
		controller.GetComponent<AudioSource>().Play();				
	}

	// Use this for initialization
	void Start () {
		if (controller != null) {
			Destroy(gameObject);
			return;
		}
		controller = this.gameObject;
		MusicController musician = controller.GetComponent<MusicController>();
		// Audio
		controller.GetComponent<AudioSource>().clip = musician.Music[musicTrack];
		controller.GetComponent<AudioSource>().Play();	
	}
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		//if () Debug.Log();		
		if (!controller.GetComponent<AudioSource>().isPlaying && 
			controller.GetComponent<AudioSource>().time == 0) NextTrack();
	}
}
