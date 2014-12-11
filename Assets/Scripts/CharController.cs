using UnityEngine;
using System.Collections;

public class CharController : MonoBehaviour {
	
	public enum State{
		NORMAL = 0,
		BURST = 1,
		JUMPING = 4,
		DOUBLEJUMPING = 8,
	}
	
	private Director director;
	private SpriteController sprite;
	private AudioSource characterSound;
	
	public AudioClip [] goodSounds;
	public AudioClip [] badSounds;
	
	private State currentState = State.NORMAL;
	public State GetCurrentState() {
		return currentState;
	}
	
	
	private float dashSpeed = 3.0f;
	private float dashTimer = 0.0f;
	private float dashCooldown = 0.0f;
	private float maxDashCooldown = 1.0f;
	private float maxDashTime = 2.0f;
	void Dash() {
		// No dashing if
		if (dashCooldown > 0.0f) {
			dashCooldown -= Time.deltaTime;
			return;
		}
		if ((currentState & State.BURST) == State.NORMAL) {
			director.SetSpeed(0);
			return;
		}
		dashTimer += Time.deltaTime;
		director.SetSpeed(dashSpeed);
		// Unburst at end of timer
		if (dashTimer > maxDashTime) {
			dashCooldown = maxDashCooldown;
			dashTimer = 0.0f;
			director.SetSpeed(0);
			if ((currentState & State.JUMPING) != State.JUMPING) sprite.SetAnimation(2, 1.0f/Director.normalSpeed, 16);
			currentState = (currentState & ~State.BURST);
		}
	}
	
	private const float jumpSpeed = 1.0f;
	private float jumpTimer = 0.0f;
	private float jumpStage = 0.0f;
	private float jumpHeight = 7.0f;
	private float startHeight = 0.0f;
	private bool falling = false;
	void Jump() {
		// No jumping if prancing along
		if (currentState == State.NORMAL) return;
		// Switch to fall state if you're bursting
		if ((currentState & State.BURST) == State.BURST) {
			if ((currentState & State.JUMPING) == State.JUMPING) {
				jumpTimer = 0.5f/jumpSpeed;
				startHeight = transform.position.y;
				falling = true;
			}
			return;
		}
		jumpTimer += Time.deltaTime;
		jumpStage = jumpTimer*jumpSpeed;
		// Up
		// Use double jumper bool
		if (jumpStage < 0.5f) {
			transform.position = startPosition + Vector3.Slerp(new Vector3(0, startHeight, 0),
											new Vector3(0, jumpHeight, 0),
											jumpStage*2);
		// Fall
		} else {
			if (!falling) {
				falling = true;
				startHeight = transform.position.y - startPosition.y;
			}
			transform.position = startPosition + Vector3.Slerp(new Vector3(0, startHeight, 0),
											new Vector3(0, 0, 0),
											(jumpStage-0.5f)*2);
											
		}
		// End jump
		if (jumpStage > 0.999f) {
			jumpTimer = 0.0f;
			falling = false;
			startHeight = transform.position.y - startPosition.y;
			sprite.SetAnimation(2, 1.0f/director.levelSpeed, 16);
			currentState = currentState & ~(State.JUMPING | State.DOUBLEJUMPING);
		}
		transform.position = new Vector3(transform.position.x, transform.position.y, startPosition.z);
	}
	
	public ParticleEmitter explode;
	public ParticleEmitter dashBurst;
	public void Explode() {
		characterSound.PlayOneShot(goodSounds[Random.Range(0, goodSounds.Length)]);
		explode.Emit();
	}
	Vector3 startPosition;
	void Start () {
		director = GameObject.Find ("Director").GetComponent<Director>();
		startPosition = transform.position;
		sprite = GetComponent<SpriteController>();
		sprite.SetAnimation(2, 1.0f/director.levelSpeed, 16);
		sprite.Reverse();
		characterSound = GetComponent<AudioSource>();
	}
	
	void Update () {
		if (!Director.gamePaused) {
			// Manage glow-light
			Light light = GetComponentInChildren<Light>();
			light.color = Color.white;
			// Manage Input
			if (Input.GetKey(KeyCode.Z) && dashCooldown <= 0.0f) {
				if ((currentState & State.BURST) != State.BURST) {
					sprite.SetAnimation(1, 1.0f/dashSpeed, 16);
					dashBurst.Emit();
				}
				currentState = currentState | State.BURST;
			} else if ((currentState & State.BURST) == State.BURST) {
				if ((currentState & State.JUMPING) != State.JUMPING) sprite.SetAnimation(2, 1.0f/Director.normalSpeed, 16);
				currentState = (currentState & ~State.BURST);
			}
			if (Input.GetKeyDown(KeyCode.X)) {
				if ((currentState & State.JUMPING) == State.NORMAL) {
					sprite.SetAnimation(1, 1.0f/director.levelSpeed, 16);
					AudioClip jumpSound = Resources.Load("Sounds/Positive/fluttershy_yay") as AudioClip;
					characterSound.PlayOneShot(jumpSound);
					currentState = State.JUMPING;
				}
			}
			// Manage actions
			Dash ();
			Jump ();
		} else {
			Light light = GetComponentInChildren<Light>();
			light.color = Color.blue;
		}
	}
}
