using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManagerScript : MonoBehaviour, IPauseHandle
{	
	// 30 fps sucks, so we'll aim for 60fps
	public const int 	TARGET_FRAME_RATE = 60;
	
	// score increases per every X tunnel blocks passed
	public const int 	TUNNEL_BLOCKS_PER_SCORE_INCREMENT = 10;
	
	// as long as we hit a point within this many seconds, we can rack up the bonus meter
	public const int 	SECONDS_TO_BONUS_UP = 2;
	
	// if we get this many points in a row with no delay, then we get sick ass bonus points
	public const int 	POINTS_IN_A_ROW_TO_BONUS = 20;
	
	// we get double points for 10 seconds when we've enabled bonus
	public const int 	BONUS_ENABLED_TIME = 10;
	
	// the rate at which we accelerate to the target velocity
	public const float 	ACCEL = 0.5f; // 0.01
	
	// the minimum speed that we can "reverse" in when we crash into an object
	public const float 	MIN_CRASH_VELOCITY = 12f; // 0.2
	
	// we'll reduce the target velocity by this much everytime we crash into something
	public const float 	VELOCITY_REDUCTION_AFTER_CRASH = 4f; // 0.025
	
	// our ship increases speed by this much every TUNNEL_BLOCKS_PER_SCORE_INCREMENT tunnel blocks
//	public const float 	SPEED_INCREMENT = 1.5f; // 0.01
	
	// outside objects will travel slower than tunnel blocks
	public const float 	OUTSIDE_OBJ_SPEED_RATIO = 0.33f;
	
	public const int 	MAX_SHIELD = 6; // we can hit X many obstacles

	public const int 	MAX_FUEL = GuiScript.MAX_GUI_BARS + 1; // One fuel resource per single gui "bar" (and one extra hidden one)

	public const float 	PRE_DIE_TIME_SCALE = 0.25f; // timescale slows down on pre-death

	public const float 	EXPLOSION_DELAY = 8 * PRE_DIE_TIME_SCALE; // number of seconds (after death) we can fly before exploding
	public const float 	HIT_DELAY = 1.5f; // number of seconds before we can take damage again
	
	public const int 	INVULNERABILITY_TIME = 10; // number of seconds of invulnerability

	// we'll use this to adjust the game speed when we switch controllers b/c using head tracking is extremely difficult at higher speeds
	public const float 	HEAD_TRACKER_SPEED_RATIO = 0.6f; // 0.06

	// Every point gives us two fuel resources;
	public const int FUEL_INCREMENT = 2;
	
	public GameObject 		player;
	public ParticleSystem 	stars;
	public ParticleEmitter 	wormholeParticleEmitter;
	public int 				levelDescriptionId;
	
	private LevelDescription levelDescription;
	
	private Transform 		spaceShipObj;
	private AudioManager 	audioManager;
	
	// we have constants for these, but we may need to adjust them if we're using the head tracker
	private float	maxVelocity;
	private float 	minVelocity; 
	public float 	targetVelocity; // our current speed
	private float 	speedIncrement;
	
	private float 	currVelocity;
	private int 	currShield;
	private int 	currFuel;
	
	private int numTunnelBlocksPassed;
	
	private LinkedList<GameObject> currTunnels;
	private LinkedList<GameObject> currOutsideObjects;
	
//	private GameObject 	screenFlashObj;
//	private GUITexture 	screenFlashGuiTexture;
//	private Texture2D 	screenFlashTexture2D;
//	private Color 		currScreenFlashColor; // we'll use this to store the rgb values
//	private float 		currScreenFlashAlpha; // we'll use this to update the alpha channel separately
//	private bool 		fadeScreenBlack;
	private int 		score;
	
	private GuiScript 			guiScript;
	private PreferencesScript 	prefsScript;
	private SpaceShipScript 	spaceShipScript;
	
	private bool 	isPaused;
	private bool 	isDead;
	private float 	deathTime;
	private float 	lastHitTime;
	private bool 	gameOver;
	private bool 	gameFinished;
	private float 	invulnerabilityTime;
	private bool 	isInvulnerable;
	private float 	lastPointTime;
	private int 	pointsInRow;
	private float 	bonusStartTime;
	private bool	bonusPointsEnabled;
	
	// TODO: remove after human trial
	private int numHits; // keep track of number of objects hit
	private bool trialCompleted = false;
	
	// ************************* ENUMS ************************* //
	
	public enum SpeedRange 
	{
		EyeTracker,
		Regular
	}
	
	// ************************* OVERRIDDEN METHODS ************************* //
	
	void Awake ()
	{
	    Application.targetFrameRate = TARGET_FRAME_RATE;
	}//end Awake
	
	
	private void destroyPreloadedObjs()
	{
		GameObject preloadedObjs = GameObject.Find("PreloadObjects");
		GameObject.Destroy(preloadedObjs);
	}
	
	// initialize the level by creating X many tunnel objects 
	// (depends on visibileTunnelLength var)
	void Start() 
	{
		// delete preloaded objects
		destroyPreloadedObjs();
		
		// grab the space ship object
		if ( player != null )
		{
			spaceShipObj = player.transform.FindChild("SpaceShip");
			audioManager = this.GetComponent<AudioManager>();
		}

		GameObject guiObj = GameObject.Find("GUI");
		guiScript = guiObj.GetComponent<GuiScript>();
		prefsScript = guiObj.GetComponent<PreferencesScript>();
		spaceShipScript = player.transform.FindChild("SpaceShip").GetComponent<SpaceShipScript>();
		
		// setup the level
		setLevelDescription();

		currFuel = MAX_FUEL;
		currShield = MAX_SHIELD;
		isPaused = false;
		isDead = false;
		gameOver = false;
		lastHitTime = 0;
		deathTime = 0;
		numHits = 0;
		invulnerabilityTime = -1;
		isInvulnerable = false;
		numTunnelBlocksPassed = 0;
		lastPointTime = -1;
		pointsInRow = 0;
		bonusStartTime = 0;
		bonusPointsEnabled = false;
	
		if ( wormholeParticleEmitter != null ) 
			wormholeParticleEmitter.emit = false;
		
		if ( stars != null )
			stars.enableEmission = false;
		
		maxVelocity = levelDescription.getMaxLevelSpeed();
		minVelocity = levelDescription.getMinLevelSpeed();
		speedIncrement = levelDescription.getSpeedIncrement();
		targetVelocity = levelDescription.getMinLevelSpeed();

		// create initial tunnel blocks
		currTunnels = levelDescription.initLevel();
		
		currOutsideObjects = new LinkedList<GameObject>();
		
		// set up initial velocity
		currVelocity = targetVelocity;
		if ( guiScript != null )
		{
			guiScript.setSpeedText( (currVelocity - minVelocity) / (maxVelocity - minVelocity));

			// TODO set fuel text?
//			guiScript.setPointsInARowText(0);
		}
		
		// start intro 
		IPlayerMove.init();
		if ( prefsScript != null )
			prefsScript.setupController(IPlayerMove.Controller.Start);
	}//end start
	
	// Update is called once per frame
	void Update () 
	{	
		if ( !isPaused )
		{
			// pull all objects closer to the camera
			moveWorld();

			// Recycle tunnel blocks if we need to. For every third recycled tunnel block, we'll decrement the fuel counter.
			if (recycleLevelObjs () && numTunnelBlocksPassed % 2 == 0) {
				currFuel--;
				guiScript.setFuelText(currFuel);

				if (currFuel < 0) 
				{
					die ();
				}
			}
			
//			updateGui();
			
			if ( isInvulnerable )
				updateInvulnerabilityStatus();
			
			if ( bonusPointsEnabled )
				updateBonusStatus();
			else
				updateBonusTime();
		}//end if
	}//end update
	
	// ************************* PUBLIC METHODS ************************* //
	
	public void pauseGame()
	{
		Debug.Log("Pausing Game");
		Time.timeScale = 0;
		
		audioManager.killGameAudio();
		audioManager.pauseMusic();
		audioManager.playSound(AudioManager.Sound.guiSound1);
		
		// tell every object to pause; they should all have some code to handle this
		Object[] objects = FindObjectsOfType (typeof(GameObject));
		foreach (GameObject go in objects)
			go.SendMessage ("OnPauseGame", SendMessageOptions.DontRequireReceiver);
	}
	
	public void resumeGame()
	{
		Debug.Log("Resuming Game");
		Time.timeScale = 1;
		
		audioManager.playGameMusic();
		
		// tell every object to resume; they should all have some code to handle this as well
		Object[] objects = FindObjectsOfType (typeof(GameObject));
		foreach (GameObject go in objects)
			go.SendMessage ("OnResumeGame", SendMessageOptions.DontRequireReceiver);
		
		if ( PlayerPrefs.GetInt(PreferencesScript.TAG_SOUNDS) == (int)PreferencesScript.Sounds.On )
			playSound(AudioManager.Sound.engine);
	}
	
	// ONLY GETS CALLED FROM THE STARTSCREEN. Do not call this in the actual game scene.
//	public void startScreenFadeToBlack()
//	{
//		fadeScreenBlack = true;
//	}
	
//	public void flashScreen(Color screenFlashColor, float screenFlashAlpha)
//	{
//		// setup screen flash vars
//		if ( screenFlashAlpha > 0f )
//		{
//			currScreenFlashColor = screenFlashColor;
//			currScreenFlashAlpha = screenFlashAlpha;
//		}
//	}
	
	public void toggleWormhole(Color screenFlashColor, float screenFlashAlpha)
	{	
		guiScript.flashScreen(screenFlashColor, screenFlashAlpha);
	
		if ( wormholeParticleEmitter.emit )
		{
			maxVelocity = levelDescription.getMaxLevelSpeed();
			minVelocity = levelDescription.getMinLevelSpeed();
			speedIncrement = levelDescription.getSpeedIncrement();
			
			wormholeParticleEmitter.emit = false;
			stars.enableEmission = false;
			playSound(AudioManager.Sound.arrive);
			randomizeVelocity();
		}
		else
		{
			wormholeParticleEmitter.emit = true;
			stars.enableEmission = true;
			playSound(AudioManager.Sound.warp);
			
			targetVelocity = maxVelocity;
			setEngineSoundPitch();
		}
	}
	
	public void crash(Color screenFlashColor, float screenFlashAlpha)
	{
		guiScript.flashScreen(screenFlashColor, screenFlashAlpha);
		numHits++;
		
		// play crash sound
		playSound(AudioManager.Sound.hit);

		if ( audioManager != null )
			audioManager.crash();
		
		// if we're going fast enough when we hit the object, just divide the speed by two
		// OR if we're going backwards (aka we just hit an object so now we're flying backwards)
		// and hit another object, then make sure we go forward
		if ( currVelocity >= MIN_CRASH_VELOCITY  || currVelocity < 0)
		{
			currVelocity = -(currVelocity * 0.5f);
		}
		// else, set the speed to some minimum value so keep bouncing (we'll fly right through
		// the object if we don't)
		else
		{
			currVelocity = -MIN_CRASH_VELOCITY;
		}
	
		// reduce the target velocity after crashing to make the game a bit easier (it's already pretty tough)
		if ( targetVelocity > maxVelocity / 2 )
			targetVelocity = targetVelocity - (VELOCITY_REDUCTION_AFTER_CRASH * 2);
		else
			targetVelocity = targetVelocity - VELOCITY_REDUCTION_AFTER_CRASH;

		if ( targetVelocity < minVelocity )
			targetVelocity = minVelocity;
		
		setEngineSoundPitch();
		
		if ( !isInvulnerable )
		{
			// the player gets a few seconds of invincibility after taking damage
			if ( Time.time > lastHitTime + HIT_DELAY )
			{
				lastHitTime = Time.time;
				
				// take off one hit point
				if ( !isDead )
				{
					currShield--;
	
					// play sound when we're low on shield
					if (currShield == 2) 
					{
						playSound(AudioManager.Sound.shieldLow);
					} 
					else if ( currShield == 1 ) 
					{
						playSound(AudioManager.Sound.shieldCritical);
					}
				}
				if ( currShield >= 0 )
					guiScript.setShieldText( currShield );
			}
			
			guiScript.setSpeedText( (currVelocity - minVelocity) / (maxVelocity - minVelocity));
				
			// if we have no more hit points left, then the spaceship explodes
			if ( currShield <= 0 )
				die();
		}
	}//end crash

	public void destroyDeathPickup(Color screenFlashColor, float screenFlashAlpha)
	{
		guiScript.flashScreen(screenFlashColor, screenFlashAlpha);

		playSound (AudioManager.Sound.explode);
		playSound (AudioManager.Sound.evilLaugh);
	}
	
	public void laserDamage(Color screenFlashColor, float screenFlashAlpha)
	{
		guiScript.flashScreen(screenFlashColor, screenFlashAlpha);
		numHits++;
		
		playSound(AudioManager.Sound.shock);
		
		if ( !isInvulnerable )
		{
			// the player gets a few seconds of invincibility after taking damage
			if ( Time.time > lastHitTime + HIT_DELAY )
			{
				lastHitTime = Time.time;
				
				// take off one hit point
				currShield--;
				guiScript.setShieldText( currShield );
				
				// play sound when we're low on shield
				if (currShield == 2) 
				{
					playSound(AudioManager.Sound.shieldLow);
				} 
				else if ( currShield == 1 ) 
				{
					playSound(AudioManager.Sound.shieldCritical);
				}
			}//end if
			
			// if we have no more hit points left, then we're dead
			if ( currShield <= 0 )
			{
				// if used up out last hit point, then the spaceship flies aimlessly without
				// control until we hit another obstacle
				if ( !isDead )
					preDie();
				else 
					die();
			}//end if
		}//end if
	}//end laserDamange
	
	public void incrementPointsAndAddFuel(int points, Color screenFlashColor, float screenFlashAlpha)
	{
		if ( bonusPointsEnabled )
			points = points * 2;
			
		if ( targetVelocity + 0.1f >= maxVelocity )
			points = points * 2;
		
		score += points;
		guiScript.setScoreText (score);
		guiScript.setBonusText(points);
		guiScript.flashScreen(screenFlashColor, screenFlashAlpha);

		// Add fuel
		if (bonusPointsEnabled) {
			currFuel = currFuel + (FUEL_INCREMENT * 2);
		} else {
			currFuel = currFuel + FUEL_INCREMENT;
		}
		if (currFuel > MAX_FUEL) {
			currFuel = MAX_FUEL;
		}
		guiScript.setFuelText (currFuel);
	}
		
	public void incrementPointAndFuel(int points, Color screenFlashColor, float screenFlashAlpha)
	{
		lastPointTime = Time.time;

		if (!bonusPointsEnabled) {
			pointsInRow++;
		}
		
		if ( pointsInRow >= POINTS_IN_A_ROW_TO_BONUS )
		{
			bonusStartTime = Time.time;
			bonusPointsEnabled = true;
			pointsInRow = 0;
			playSound(AudioManager.Sound.bonus);
		}
		else 
		{
			if ( bonusPointsEnabled ) 
			{
				playPointSound(1 + (1 * 0.15f));
			} 
			else 
			{
				playPointSound(1.0f + (((float)pointsInRow/POINTS_IN_A_ROW_TO_BONUS) * 0.15f));
			}
		}
			
		incrementPointsAndAddFuel(points, screenFlashColor, screenFlashAlpha);
	}
	
	public void playerRotateLeft(int points, Color screenFlashColor, float screenFlashAlpha)
	{
		incrementPointsAndAddFuel(points, screenFlashColor, screenFlashAlpha);
		
		IPlayerMove playerScript = prefsScript.getCurrentControllerScript();
		playerScript.rotateLeft();
		
		playSound(AudioManager.Sound.rotate);
	}//end playerRotateLeft
	
	public void playerRotateRight(int points, Color screenFlashColor, float screenFlashAlpha)
	{
		incrementPointsAndAddFuel(points, screenFlashColor, screenFlashAlpha);
		
		IPlayerMove playerScript = prefsScript.getCurrentControllerScript();
		playerScript.rotateRight();
		
		playSound(AudioManager.Sound.rotate);
	}//end playerRotateRight
	
	// huge speed boost
	public void speedBoost(int points, Color screenFlashColor, float screenFlashAlpha, float speedIncrement )
	{
		incrementPointsAndAddFuel(points, screenFlashColor, screenFlashAlpha);
		
		targetVelocity += speedIncrement;
		if ( targetVelocity > maxVelocity )
			targetVelocity = maxVelocity;
		
		setEngineSoundPitch();
		playSound(AudioManager.Sound.speedUp);
	}
	
	// slows back down to the slowest speed possible
	public void slowDown(int points, Color screenFlashColor, float screenFlashAlpha, float decrement)
	{
		incrementPointsAndAddFuel(points, screenFlashColor, screenFlashAlpha);

		targetVelocity -= decrement;
		if ( targetVelocity < minVelocity )
			targetVelocity = minVelocity;

		setEngineSoundPitch();
		playSound(AudioManager.Sound.slow);
	}

	public void rechargeShield(int points, Color screenFlashColor, float screenFlashAlpha)
	{
		incrementPointsAndAddFuel(points, screenFlashColor, screenFlashAlpha);

		currShield = currShield + 2;

		if ( currShield > MAX_SHIELD ) 
			currShield = MAX_SHIELD;
		
		guiScript.setShieldText( currShield );
		playSound(AudioManager.Sound.shieldRegen);
	}
	
	public void invulnerability(int points, Color screenFlashColor, float screenFlashAlpha)
	{
		incrementPointsAndAddFuel(points, screenFlashColor, screenFlashAlpha);
		
		invulnerabilityTime = Time.time;
		isInvulnerable = true;
		
		guiScript.showShieldInvulnerable(true);
		playSound(AudioManager.Sound.invulnerabilityEnabled);
	}

	public bool isPlayerInvulnerable() 
	{
		return isInvulnerable;
	}

	public void OnPauseGame()
	{
		isPaused = true;
	}
	
	public void OnResumeGame()
	{
		isPaused = false;
	}
	
	public void playPointSound(float pitch)
	{
		if ( audioManager != null )
		{
			audioManager.playPointSound(pitch);
		}		
	}
	
	public void playSound(AudioManager.Sound sound)
	{
		if ( audioManager != null )
		{
			audioManager.playSound(sound);
		}
	}
	
	public int getCurrentScore()
	{
		return score;
	}
	
	public bool checkLocalHighScore()
	{
		bool isHighScore = false;
		if ( score > PlayerPrefs.GetInt(PreferencesScript.TAG_HIGHSCORE) )
		{
			PlayerPrefs.SetInt(PreferencesScript.TAG_HIGHSCORE, score);
			isHighScore = true;
		}
		return isHighScore;
	}
	
	public void die()
	{
		Time.timeScale = 1;

		Color screenFlashColor = new Color(1,1,1);
		guiScript.flashScreen(screenFlashColor, 0.7f);

		if ( audioManager != null )
			audioManager.die();
		
		guiScript.setScoreText(score);
		
		// take away player control
		if ( !isDead )
			prefsScript.setupController(IPlayerMove.Controller.Dead);

		spaceShipScript.explode();
		
		// the human trial level has a different game over screen
		if ( levelDescription is LevelTrial )
			guiScript.showGameOverTrial(trialCompleted, numHits);
		else if ( checkLocalHighScore() )
		{
			guiScript.showNewHighScore(score);
			playSound(AudioManager.Sound.applause);
		}
		else
			guiScript.showGameOver();
		
		// disable collider so we don't keep blowing up
		BoxCollider collider = player.GetComponent<BoxCollider>();
		collider.enabled = false;
		
		isDead = true;
		gameOver = true;

		sendScoreToServer ();
	}

	private void sendScoreToServer ()
	{
		// send score to server
		string url = PreferencesScript.POST_HIGH_SCORE_URL;
		
		WWWForm form = new WWWForm();
		
		string playerName = PlayerPrefs.GetString(PreferencesScript.TAG_PLAYER_NAME, "Player");
		
		// make sure we only send alphanumeric characters or the shitty php script will blow up
		string playerNameCleaned = "";
		char curr;
		if ( playerName != null )
		{
			for ( int i = 0; i < playerName.Length; i++ )
			{
				curr = playerName[i];
				if ( char.IsNumber(curr) || char.IsLetter(curr) )
					playerNameCleaned = playerNameCleaned + curr;
			}//end for
		}//end if
		
		if ( playerNameCleaned.Equals("") )
			playerNameCleaned = "Player";
		
		form.AddField(PreferencesScript.KEY_POST_NAME, playerNameCleaned );
		form.AddField(PreferencesScript.KEY_POST_SCORE, score);
		WWW www = new WWW(url, form);
		
		StartCoroutine(postHighScore(www));
	}
	
	// ************************* PRIVATE METHODS ************************* //
	
	private void setLevelDescription()
	{
		if ( levelDescriptionId == (int)LevelDescription.LevelDescriptionId.StartScreen )
			levelDescription = ScriptableObject.CreateInstance<LevelStartScreen>();		
		else if ( levelDescriptionId == (int)LevelDescription.LevelDescriptionId.SpaceWreak )
			levelDescription = ScriptableObject.CreateInstance<LevelSurvivalAll>();
		else if ( levelDescriptionId == (int)LevelDescription.LevelDescriptionId.HumanTrial )
			levelDescription = ScriptableObject.CreateInstance<LevelTrial>();
		else
		{
			Debug.Log("Level ID " + levelDescriptionId + " does not exist! Using default level.");
			levelDescription = ScriptableObject.CreateInstance<LevelSurvivalAll>();
		}		
		levelDescription.init(this.gameObject);
	}
	
	// destroy any objects that have travelled passed the camera, and create a new
	// one as it's replacement starting at the back of the tunnel
	// Returns true if a tunnel block was recycled
	private bool recycleLevelObjs() 
	{
		bool tunnelBlockWasRecycled = false;

		// check to see if the closest tunnel block has gone passed the camera
		LinkedListNode<GameObject> closestTunnelBlock = currTunnels.First;
		if ( closestTunnelBlock != null && closestTunnelBlock.Value.transform.localPosition.z < levelDescription.getTunnelDestroyDistance() )
		{	
			tunnelBlockWasRecycled = true;
			numTunnelBlocksPassed++;
			
			// increment the score for each 10 tunnel blocks passed
			if ( !gameFinished && numTunnelBlocksPassed % TUNNEL_BLOCKS_PER_SCORE_INCREMENT == 0 )
			{
				score++;
				if ( guiScript != null ) 
					guiScript.setScoreText(score);
			}
			// check to see if we should increase speed
			if ( numTunnelBlocksPassed % levelDescription.getNumTunnelsPerSpeedIncrease() == 0 )
				speedUp();
			
			if ( guiScript != null )
			{
//				guiScript.setScoreText(score);			
				guiScript.setSpeedText( (currVelocity - minVelocity) / (maxVelocity - minVelocity));
			}
			
			// remove and destroy the closest tunnel block
			currTunnels.RemoveFirst();
			Destroy(closestTunnelBlock.Value);
			
			// if the number of tunnel blocks generated hasn't reached the limit (or if it's zero),
			// then create a new tunnel block
			if ( levelDescription.getLevelLength() == 0 || levelDescription.getNumBlocksCreated() < levelDescription.getLevelLength() )
			{
				// create a random tunnel block
				GameObject newTunnelBlock = levelDescription.createRandomTunnelBlock();
				
				// set the position of the new tunnel...
				LinkedListNode<GameObject> lastTunnelBlock = currTunnels.Last;
				float zPos = lastTunnelBlock.Value.transform.localPosition.z  + LevelDescription.BLOCK_SIZE;
				newTunnelBlock.transform.localPosition = new Vector3(0, 0, zPos);
				
				//... and add it to the back of the queue
				currTunnels.AddLast(newTunnelBlock);
				
				// Now, check to see if the closest outside object has gone passed the camera
				LinkedListNode<GameObject> closestOutsideObject = currOutsideObjects.First;
				if ( closestOutsideObject != null && closestOutsideObject.Value.transform.localPosition.z < levelDescription.getTunnelDestroyDistance() )
				{
					// destroy the object
					currOutsideObjects.RemoveFirst();
					Destroy(closestOutsideObject.Value);
				}//end if
				
				// FINALLY, also randomly generate an outside object when we recycle tunnel blocks
				GameObject newOutsideObject = levelDescription.createRandomOutsideObject();
				if ( newOutsideObject != null )
				{
					// set the position of the new setion block to be the same as the last tunnelblock position
					newOutsideObject.transform.localPosition = new Vector3(0, 0, zPos);
					
					// ...and add it to the back of the queue
					currOutsideObjects.AddLast(newOutsideObject);
				}

				// if we think the game isn't finished but the levelDescription is telling us that is is, 
				// then setup the game ending scene
				if ( !gameFinished && levelDescription.isGameFinished() ) 
				{
					gameFinished = true;
					setupFinishScene();
				}
			}//end if
		}//end if

		return tunnelBlockWasRecycled;

	}//end recycleLevelObjs
	
	// move the world closer to the camera
	private void moveWorld()
	{
		if ( !gameOver && !gameFinished )
		{

			// start accelerating if we're not moving
			if ( currVelocity <= targetVelocity )
				currVelocity += ACCEL;
			
			// make sure we don't go faster than the target velocity
			if ( currVelocity > targetVelocity )
				currVelocity = targetVelocity;
			
//			// adjust star speed
//			if ( stars != null )
//			{
//				int numParticles = stars.particleCount;
//				ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
//				stars.GetParticles(particles);
//				
//				Vector3 starSpeed = new Vector3(0, 0, currVelocity);				
//				for (int i = 0; i < particles.Length; i++)
//				{
//					particles[i].velocity = starSpeed;
//				}
//				stars.SetParticles(particles, numParticles);
//			}//end if
			
			translateWorldObjects();
			
			if ( isDead && Time.time > deathTime + EXPLOSION_DELAY )
				die();
		}
		else if ( gameFinished ) 
		{
			translateWorldObjects();
		}

	}//end moveWorld

	private void translateWorldObjects ()
	{
		GameObject[] levelObjList = GameObject.FindGameObjectsWithTag("TunnelBlock");

		float deltaTime = Time.deltaTime;
		
		// bring the level towards the cam
		foreach ( GameObject levelObj in levelObjList )
		{
			//				Debug.Log("" + (-currVelocity * (deltaTime)));
			levelObj.transform.Translate(0f, 0f, -currVelocity * deltaTime, player.transform);
		}//end for
		
		GameObject[] outsideObjects = GameObject.FindGameObjectsWithTag("OutsideObject");
		
		// bring the level towards the cam
		foreach ( GameObject obj in outsideObjects )
		{
			obj.transform.Translate(0f, 0f, -currVelocity * OUTSIDE_OBJ_SPEED_RATIO * deltaTime, player.transform);
		}//end for
	}
	
	// TODO: see if you like this or not
	private void randomizeVelocity()
	{
//		targetVelocity = Random.Range(minVelocity, ((maxVelocity + minVelocity) / 2));
		
//		if ( targetVelocity < minVelocity )
//			targetVelocity = minVelocity;

		targetVelocity = levelDescription.getZoneStartingSpeed ();
		setEngineSoundPitch();
	}
	
	private void setEngineSoundPitch()
	{
		// MAX pitch for engine sound is 1.85
		float scale = (1.85f - 1) / (maxVelocity - minVelocity);
		float newPitch = 1 + ((targetVelocity - minVelocity) * scale);

		if ( audioManager != null )
			audioManager.setEnginePitch(newPitch);
	}
	
	private void setEngineSoundPitchForPreDie() 
	{
		// MAX pitch for engine sound is 1.85
		float scale = (1.85f - 1) / (maxVelocity - minVelocity);
		float newPitch = (1 + ((targetVelocity - minVelocity) * scale)) * 0.7f;
		
		if ( audioManager != null )
			audioManager.setEnginePitch(newPitch);
	}
	
	// speeds up by a small increment
	private void speedUp()
	{
		if ( !gameOver && !isDead )
		{			
			if ( (int)(targetVelocity * 100) < (int)(maxVelocity * 100) )
				targetVelocity += speedIncrement;
		
			// speed gets capped at a certain value
			if ( targetVelocity > maxVelocity)
				targetVelocity =  maxVelocity;
			
			setEngineSoundPitch();
		}
	}
	
	private void updateInvulnerabilityStatus()
	{
		if ( Time.time > invulnerabilityTime + INVULNERABILITY_TIME )
		{
			invulnerabilityTime = -1;
			isInvulnerable = false;
			guiScript.showShieldInvulnerable(false);
			playSound(AudioManager.Sound.invulnerabilityDisabled);
		}
	}
	
	private void updateBonusTime()
	{
		if ( pointsInRow > 0 )
		{
			if ( Time.time > lastPointTime + SECONDS_TO_BONUS_UP )
			{
				pointsInRow = 0;
			}
		}
	}
	
	private void updateBonusStatus()
	{
		if ( Time.time > bonusStartTime + BONUS_ENABLED_TIME )
		{
			bonusPointsEnabled = false;
		}
	}
	
//	private void updateGui()
//	{
//		if ( fadeScreenBlack && currScreenFlashAlpha < 1f )
//		{
//			screenFlashGuiTexture.color = new Color(currScreenFlashColor.r, currScreenFlashColor.g, currScreenFlashColor.b, currScreenFlashAlpha);
//			currScreenFlashAlpha += SCREEN_FLASH_RECOV;
//			
//		}
//		else if ( currScreenFlashAlpha > 0f ) 
//		{
//			screenFlashGuiTexture.color = new Color(currScreenFlashColor.r, currScreenFlashColor.g, currScreenFlashColor.b, currScreenFlashAlpha);
//			currScreenFlashAlpha -= SCREEN_FLASH_RECOV;
//		}
//		else 
//			screenFlashGuiTexture.color = new Color(1, 1, 1, 0);	
//	}//end updateGUI
	
	private void preDie()
	{	
		audioManager.preDie();
		
		// Slow down effect
		Time.timeScale = PRE_DIE_TIME_SCALE;
		setEngineSoundPitchForPreDie();
		
   		guiScript.setScoreText(score);

		// record time of death
		deathTime = Time.time;

		// take away player control
		if ( !isDead )
		{
			prefsScript.setupController(IPlayerMove.Controller.Dead);
			isDead = true;
		}
		
		// make sure the space ship is visible for the time being
		spaceShipObj.GetComponent<MeshRenderer>().enabled = true;
		spaceShipScript.setOnFire(true);
	}

	private void setupFinishScene ()
	{
//		guiScript.flashScreen(Color.white, 1f);

		prefsScript.setupController(IPlayerMove.Controller.Finished);
		spaceShipObj.GetComponent<MeshRenderer>().enabled = true;
		guiScript.showGameFinished();

		playSound(AudioManager.Sound.applause);
		if ( stars != null )
			stars.enableEmission = true;

		if ( audioManager != null )
			audioManager.killEngineAudio();

		if ( checkLocalHighScore() )
			guiScript.showNewHighScore(score);

		sendScoreToServer ();
	}
	
	private IEnumerator postHighScore(WWW www)
	{
		yield return www;
		
		// check for errors
		if ( www.error == null )
		{
//			Debug.Log(www.text);
			
			// check if made leaderboard
			if ( www.text.Equals("1") )
				guiScript.showLeaderBoardMessage();
		}
		else
		{
			Debug.Log(www.error);			
		}
	}//end postHighScore
}//end script
