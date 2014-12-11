using UnityEngine;
using System.Collections;

public class FaceController : MonoBehaviour {
	
	public Director director;
	public Director.Element element;
	
	public bool hit = false;
	void Start () {
	}
	
	void Update () {
	
	}
	
	public void Explode (bool dash = false) {
		GetComponent<AudioSource>().Play();
		hit = true;
	}
	
	void OnTriggerEnter(Collider other) {
		// Not another interactable
		if (other.GetComponent<CharController>()) {
			director.ShowHitMessage(this);
		}
	}	
}
