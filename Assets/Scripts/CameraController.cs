using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	Vector3 origin;
	void Start () {
		origin = transform.position;
		Shake ();
	}
	
	public void Shake() {
		StartCoroutine(DoJiggle());
	}
	
	private IEnumerator DoJiggle() {
		float magnitude = 0.8f;
		float etime = 0f;
		float freq = 10f;
		while (etime < 0.7f) {
			transform.position = origin + magnitude*(new Vector3 (	Mathf.Sin(2*freq*Mathf.PI*Time.time),
																	Mathf.Sin(2*freq*Mathf.PI*Time.time),
																	Mathf.Sin(2*freq*Mathf.PI*Time.time)));
			magnitude *= 0.8f;
			etime += Time.deltaTime;
			yield return null;
		}
		transform.position = origin;
	}
	void Update () {
	
	}
}
