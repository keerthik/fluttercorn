using UnityEngine;
using System.Collections;

public interface ISpriteControllerListener {
	void AnimationComplete();
}

public class SpriteController : MonoBehaviour {
	
	private float countdown = 2.0f;
	private int frame = 0;
	//end here is actually start + N, where there are N frames. This will mean end = a[last]+1
	private int start, end;
	private bool fwd = true;
	private float cellwidth;
	private float cellheight;
	private bool ready = false;
	private float animationYOffset;
	// Settable Parameters
	public int cycles = 0;
	public bool PlayOnAwake = false;
	public bool autoflip = false;
	public bool loop = true;
	public float frametime;
	public float cell_width;
	public float cell_height;

	public ISpriteControllerListener listener;
	
	void Awake () {
		if (cellheight == 0) cellheight = cellwidth;
		cellwidth = cell_width/renderer.material.mainTexture.width;
		cellheight = cell_height/renderer.material.mainTexture.height;
		renderer.material.mainTextureScale = new Vector2(cellwidth, cellheight);
		if (PlayOnAwake) SetAnimation(2, 1f, 16);
	}
	
	void Update () {
		countdown -= Time.deltaTime;
		if (ready && countdown <= 0.0f)
			AdvanceFrame();
	}
	
	void AdvanceFrame() {
		if (fwd) frame ++;
		else frame --;
		if (frame == start-1 || frame == end) {
			if (loop) {
				frame = fwd?start:end-1;
			} else {
				frame = fwd?end-1:start;
				if (listener != null) {
					listener.AnimationComplete();
				}
			}
			cycles++;
			if (autoflip) fwd = !fwd;
		}
		renderer.material.mainTextureOffset = new Vector2(frame * cellwidth, animationYOffset);
		countdown = frametime;
	}

	public void Reverse() {
		fwd = !fwd;
	}
	public void Freeze() {
		ready = false;
	}
	public void Resume() {
		ready = true;
	}
	public void SetAnimation(int row, float totaltime, int aend, int astart = 0) {
		frame = 0;
		cycles = 0;
		frametime = totaltime/aend;
		countdown = frametime;
		start = astart;
		end = aend;
		animationYOffset = row*cellheight;
		ready = true;
		renderer.material.mainTextureOffset = new Vector2(frame * cellwidth, animationYOffset);
	}
}
