using UnityEngine;
using System.Collections;

public class BackgroundScroller : MonoBehaviour {
	
	public float speed;
	float yval;
	void Start () {
		yval = renderer.material.mainTextureOffset.y;
	}
	
	private float time = 0.0f;
	void Update () {
		if (!Director.gamePaused) {
			time += speed*Time.deltaTime;
			renderer.material.mainTextureOffset = new Vector2 (1.0f-time, yval);
			if (time > 1.0f) time = 0.0f;
		}
	}
}
