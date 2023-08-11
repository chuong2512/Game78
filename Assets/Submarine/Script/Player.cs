using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	[Header("Setup Submarine")]
	[Range(0,100)]
	public int health = 100;
	[Tooltip("This value high, the damage is less")]
	public int defendStrength = 100;
	public float force = 35;
	public float rotationSpeed = 2;
	public float rotationMaxAngle = 10;
	public AudioClip soundDamage;
	public AudioClip soundDestroy;
	[Header("Sound Engine")]
	public AudioClip soundEngine;
	[Range(0,0.5f)]
	public float volumeOff = 0.2f;
	[Range(0.2f,1f)]
	public float volumeOn = 0.5f;
	AudioSource soundEngineFX;
	bool isUseEngine = false;

	[Header("Shield")]
	public GameObject Shield;
	public float timeRecharge = 15;
	[Tooltip("time to use Shield from 100 to 0")]
	public float timeUseShield = 5;
	public float shieldEnegry{ get; set; }
	float timeBegin;
	bool isUsingShield =false;

	[Header("Magnet")]
	public Magnet Magnet;
	public float timeMagnet = 7;

	[Header("Rocket")]
	public float fireRate = 0.35f;
	float timeToFire = 0;
	[Tooltip("minimum rockets")]
	public int rocketsDefault = 3;
	public GameObject Rocket;
	public AudioClip soundRocket;
	public Transform BulletSpawnPoint;

	[Header("Gun Fire")]
	public float fireBulletRate = 0.2f;
	float timeToFireBullet = 0;
	[Tooltip("minimum rockets")]
	public int bulletsDefault = 10;
	public GameObject Bullet;
	public AudioClip soundFireBullet;
	public bool allowFireGun{ get; set; }
	public Animator GunSpawnPoint;

	[Header("Blink Effect")]
	public float timeBlink = 2;
	public float blinkSpeed = 0.2f;
	public Color blinkColor = Color.blue;
	public SpriteRenderer submarineSprite;

	//the player can be kill by anything, use this when player hit somthing and it's blinking
	bool godMode = false;

	Rigidbody2D rig;
	Animator anim;
	ShakeCamera SharkCamera;
	bool isDead = false;
	float staticX;

	// Use this for initialization
	void Start () {
		SharkCamera = FindObjectOfType<ShakeCamera> ();
		rig = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		Shield.SetActive (false);
		Magnet.gameObject.SetActive (false);
		if (GlobalValue.Rocket < rocketsDefault) {
			GlobalValue.Rocket = rocketsDefault;
		}
		if (GlobalValue.Bullet < bulletsDefault) {
			GlobalValue.Bullet = bulletsDefault;
		}
		//		GunSpawnPoint.gameObject.SetActive (false);
		rig.gravityScale = 0;
		rig.velocity = Vector2.zero;

		soundEngineFX = gameObject.AddComponent<AudioSource> ();
		soundEngineFX.clip = soundEngine;
		soundEngineFX.loop = true;
		soundEngineFX.volume = volumeOff;
		soundEngineFX.Play ();

		staticX = transform.position.x;
	}

	// Update is called once per frame
	void Update () {
		if (GameManager.Instance.State != GameManager.GameState.Playing)
			return;

		transform.position = new Vector3 (staticX, transform.position.y, transform.position.z);

		timeToFire += Time.deltaTime;
		timeToFireBullet += Time.deltaTime;

		HandleInput ();

		transform.rotation = Quaternion.Euler (0, 0, Mathf.Clamp (rig.velocity.y * rotationSpeed, -rotationMaxAngle, rotationMaxAngle));

		//Shield
		if (!isUsingShield)
			shieldEnegry = Mathf.Clamp ((Time.time - timeBegin) * 100 / timeRecharge, 0, 100);
		else {
			shieldEnegry = 100 - Mathf.Clamp ((Time.time - timeBegin) * 100 / timeUseShield, 0, 100);
			if (shieldEnegry == 0) {
				isUsingShield = false;
				Shield.GetComponent<Shield> ().Close ();
				timeBegin = Time.time;
			}
		}
		if (isUseEngine)
			soundEngineFX.volume = Mathf.Lerp (soundEngineFX.volume, volumeOn, 0.2f);

		if (rig.velocity.y < 0) {
			isUseEngine = false;
			soundEngineFX.volume = Mathf.Lerp (soundEngineFX.volume, volumeOff, 0.2f);
		}
	}

	private void HandleInput(){
		if (Input.GetKey(KeyCode.F))
			Fire ();

		if (Input.GetKey(KeyCode.Space))
			MoveUp ();

		if (Input.GetKey(KeyCode.S))
			UseShield ();

		if (Input.GetKey (KeyCode.A))
			FireBullet ();
	}

	public void MoveUp(){
		rig.AddForce (new Vector2(0,force));
		isUseEngine = true;
	}

	public void Play(){
		timeBegin = Time.time;
		rig.gravityScale = 1.5f;
		rig.velocity = Vector2.zero;

		GlobalValue.PlayGame++;
	}

	void OnTriggerStay2D(Collider2D other){
		if (isDead)
			return;

		if(other.gameObject.CompareTag("Coin")){
			other.gameObject.SendMessage ("Collect");

			GlobalValue.Coin++;
		}

		//if the player is blinking, don't hit any obstacles
		if (godMode)
			return;

		var Enemy = other.GetComponent<Enemy> ();
		if (Enemy) {
			Damage (Enemy.damage);		//Take damage
			Enemy.Hit (0);
		}
	}

	public void Damage(int damage){

		health -= (int)(damage * (100f / defendStrength));
		if (health <= 0)
			GameManager.Instance.GameOver ();
		else {
			StartCoroutine (DoBlinks (timeBlink, blinkSpeed));
			SoundManager.PlaySfx (soundDamage);
		}

		anim.SetInteger ("Health", health);

		SharkCamera.DoShake ();
	}

	public void Die(){
		SoundManager.PlaySfx (soundDestroy);
		soundEngineFX.Stop ();
		isDead = true;
		anim.SetTrigger ("Die");
		rig.gravityScale = 0;
		rig.velocity = Vector2.zero;
		GetComponent<BoxCollider2D> ().enabled = false;

		if (transform.Find ("EngineFX"))
			transform.Find ("EngineFX").gameObject.SetActive (false);

		GunSpawnPoint.transform.parent.gameObject.SetActive (false);
	}

	public void Fire(){
		if (Rocket == null) {
			Debug.LogWarning ("There is no Rocket on this Submarine, please add one");
			return;
		}

		if (GlobalValue.Rocket <= 0)
			return;

		if (timeToFire < fireRate)
			return;

		timeToFire = 0;

		if (GameManager.Instance.RocketHolder.transform.childCount == 0)
			Instantiate (Rocket, BulletSpawnPoint.position, Quaternion.identity);
		else {
			GameManager.Instance.RocketHolder.transform.GetChild (0).transform.position = BulletSpawnPoint.position;
			GameManager.Instance.RocketHolder.transform.GetChild (0).gameObject.SetActive (true);
		}

		GlobalValue.Rocket--;
		SoundManager.PlaySfx (soundRocket);
		GlobalValue.UseRocket++;

	}

	public void FireBullet(){
		//Fire Gun 

		if (timeToFireBullet < fireBulletRate)
			return;

		if (GlobalValue.Bullet <= 0)
			return;

		timeToFireBullet = 0;
		if (Bullet == null) {
			Debug.LogWarning ("There is no Bullet on this Submarine, please add one");
			return;
		}

		if (GameManager.Instance.BulletHolder.transform.childCount == 0)
			Instantiate (Bullet, GunSpawnPoint.gameObject.transform.position, Quaternion.identity);
		else {
			GameManager.Instance.BulletHolder.transform.GetChild (0).transform.position = GunSpawnPoint.gameObject.transform.position;
			GameManager.Instance.BulletHolder.transform.GetChild (0).gameObject.SetActive (true);
		}
		GunSpawnPoint.SetTrigger ("Fire");

		GlobalValue.Bullet--;
		SoundManager.PlaySfx (soundFireBullet);

	}

	public void UseShield(){
		if (shieldEnegry < 100 || isUsingShield)
			return;

		Shield.SetActive (true);
		isUsingShield = true;
		timeBegin = Time.time;
		SoundManager.PlaySfx (GameManager.Instance.SoundManager.soundPowerUpShield);
		GlobalValue.UseShield++;
	}

	public void UseMagnet(){
		Magnet.init (timeMagnet);
		Magnet.gameObject.SetActive (true);
	}

	IEnumerator DoBlinks(float time, float seconds) {
		godMode = true;
		var timer = time;
		while (timer > 0) {
			submarineSprite.color = blinkColor;
			yield return new WaitForSeconds (seconds);
			timer -= seconds;
			submarineSprite.color = Color.white;
			yield return new WaitForSeconds (seconds);
			timer -= seconds;
		}

		//make sure renderer is enabled when we exit
		submarineSprite.color = Color.white;
		godMode = false;
	}
}
