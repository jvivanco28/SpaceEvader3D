using UnityEngine;
using System.Collections;

public class GuiScript : MonoBehaviour {
	
	// TODO: there should be a superclass for GuiScript and StartScreenGuiScript
	
	public const int GAME_OVER_DELAY = 2; // 2 second wait until we can restart after we die
	public const float MENU_LERP_SPEED = 0.1f;
	public const float POSITION_CHECK = 0.002f;
	
	// the rate at which the flash (after taking damage) returns back to it's normal state
	public const float 	SCREEN_FLASH_RECOV = 0.01f;
	
	public const int MAX_GUI_BARS = 24;

	private const int BONUS_TEXT_ARRAY_SIZE = 5;
	
	public Transform pauseMenu;
	public GUITexture pauseButton;
	public GUIText resumeButton;
//	public GUIText touchControllerButton;
	public GUIText soundsButton;
	public GUIText musicButton;
	public GUIText quitButton;

	public GUIText highScoreText;
	public GUIText scoreText;

	public GUIText speedText;
	public GUIText maxSpeedBonusText;
	
	public GUIText shieldTextRed;
	public GUIText shieldTextBlue;
	public GUIText shieldInvulnerableText;

	public GUIText fuelRemainingText;
	
	public Transform gameOverGui;
	public GUIText gameOverText;
	public GUIText gameOverTextSub;
	
	public GUIText noFaceDetectedText;
	private bool wasFaceDetected;
	
	private LevelManagerScript levelMgr;
	private AudioManager audioManager;
//	private PreferencesScript prefsScript;
	private GameObject player; // so we can change controllers
	private float gameOverTime;
	
	private bool pauseMenuEnabled;
	private Vector2 activeMenuPosition;
	private Vector2 disabledMenuPosition;
	private Vector2 activePauseButtonPosition;
	private Vector2 disabledPauseButtonPosition;
		
	private float bonusTime;
	private ArrayList bonusPointsList;
	
	private GameObject 	screenFlashObj;
	private GUITexture 	screenFlashGuiTexture;
	private Texture2D 	screenFlashTexture2D;
	private Color 		currScreenFlashColor; // we'll use this to store the rgb values
	private float 		currScreenFlashAlpha; // we'll use this to update the alpha channel separately

	private int currentScore;
	private int displayedScore;
	
	void Start ()
	{
		disablePauseMenu();
		gameOverText.enabled = false;
		gameOverTextSub.enabled = false;
		pauseMenuEnabled = false;
		
		activeMenuPosition = new Vector2(0, 0);
		disabledMenuPosition = new Vector2(-1,0);
		
		activePauseButtonPosition = new Vector2(pauseButton.transform.localPosition.x, pauseButton.transform.localPosition.y);
		disabledPauseButtonPosition = new Vector2(1.2f, pauseButton.transform.localPosition.y);
			
		player = GameObject.Find("Player");
		levelMgr = GameObject.Find("Level").GetComponent<LevelManagerScript>();
		audioManager = GameObject.Find("Level").GetComponent<AudioManager>();
		
//		prefsScript = this.GetComponent<PreferencesScript>();
		
		if ( highScoreText != null )
			highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0);
		
		maxSpeedBonusText.enabled = false;
		
		bonusTime = 0;
		bonusPointsList = new ArrayList(BONUS_TEXT_ARRAY_SIZE);
		
		showShieldInvulnerable(false);
		
//		if ( PlayerPrefs.GetInt(PreferencesScript.TAG_CONTROLLER) == (int)IPlayerMove.Controller.EyeTracker )
//			touchControllerButton.enabled = false;
//		else
//		{
//			int savedTouchController = PlayerPrefs.GetInt(PreferencesScript.TAG_TOUCH_CONTROLLER, (int)IPlayerMove.Controller.Delta);
//			if ( savedTouchController == (int)IPlayerMove.Controller.Absolute )
//				touchControllerButton.text = "Touch control: Absolute";
//			else
//				touchControllerButton.text = "Touch control: Swipe";
//		}
		
		int soundsEnabled = PlayerPrefs.GetInt(PreferencesScript.TAG_SOUNDS, (int)PreferencesScript.Sounds.On);
		if ( soundsEnabled == (int)PreferencesScript.Sounds.On )
			soundsButton.text = "Sounds: On";
		else
			soundsButton.text = "Sounds: Off";
		
		int musicEnabled = PlayerPrefs.GetInt(PreferencesScript.TAG_MUSIC, (int)PreferencesScript.Music.On);
		if ( musicEnabled == (int)PreferencesScript.Music.On )
			musicButton.text = "Music: On";
		else
			musicButton.text = "Music: Off";
		
		// this should always be true if we're using touch controls
		wasFaceDetected = true;
		noFaceDetectedText.enabled = false;
		
		// create our damage texture of size 1 and 1
		screenFlashTexture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		
		// set pixel values
		screenFlashTexture2D.SetPixel(0, 0, Color.white);

		// Apply all SetPixel calls
		screenFlashTexture2D.Apply();
		
	    // connect texture to material of GUI
		screenFlashObj = new GameObject("DamageGUI");
		screenFlashObj.transform.localScale = new Vector3(1, 1, 0);
		screenFlashObj.transform.localPosition = new Vector3(0, 0, -10);
		
		screenFlashGuiTexture = screenFlashObj.AddComponent<GUITexture>();
	    screenFlashGuiTexture.pixelInset = new Rect(0 , 0 , Screen.width , Screen.height );
		screenFlashGuiTexture.texture = screenFlashTexture2D;
		screenFlashGuiTexture.enabled = true;

		currentScore = 0;
		displayedScore = 0;
	}
	
	// Update is called once per frame
	void LateUpdate () {

		updateGui();
		
		// clear the bonus point text after 2 seconds
		if ( Time.time > bonusTime + 2 )
		{
			bonusPointsList.Clear();
		}
		// whenever the user clicks or touches the screen
		if ( Input.GetMouseButtonDown(0) )
		{		
			// pause the game
			if ( pauseButton.enabled && !pauseMenuEnabled && pauseButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				pauseGame();
			}//end if
			
			// resume the game
			else if ( resumeButton.enabled && resumeButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				resumeGame();
			}//end if
//			else if ( touchControllerButton.enabled && touchControllerButton.HitTest(Input.mousePosition) )
//			{
//				audioManager.playSound(AudioManager.Sound.guiSound1);
//				
//				// if the last saved touch controller was absolute touch, then swap in delta touch
//				if ( PlayerPrefs.GetInt(PreferencesScript.TAG_TOUCH_CONTROLLER) == (int)IPlayerMove.Controller.Absolute ) 
//				{
//					PlayerPrefs.SetInt(PreferencesScript.TAG_TOUCH_CONTROLLER, (int)IPlayerMove.Controller.Delta);
//					prefsScript.setupController(IPlayerMove.Controller.Delta);
//					touchControllerButton.text = "Touch control: Swipe";
//				}
//				else
//				{
//					PlayerPrefs.SetInt(PreferencesScript.TAG_TOUCH_CONTROLLER, (int)IPlayerMove.Controller.Absolute);
//					prefsScript.setupController(IPlayerMove.Controller.Absolute);
//					touchControllerButton.text = "Touch control: Absolute";
//				}
//			}
			else if ( soundsButton.enabled && soundsButton.HitTest(Input.mousePosition) )
			{
				// if saved value is sounds enabled, then toggle it to disabled
				if ( PlayerPrefs.GetInt(PreferencesScript.TAG_SOUNDS) == (int)PreferencesScript.Sounds.On ) 
				{
					PlayerPrefs.SetInt(PreferencesScript.TAG_SOUNDS, (int)PreferencesScript.Sounds.Off);
					soundsButton.text = "Sounds: Off";
				}
				else
				{
					PlayerPrefs.SetInt(PreferencesScript.TAG_SOUNDS, (int)PreferencesScript.Sounds.On);
					soundsButton.text = "Sounds: On";
				}
				audioManager.playSound(AudioManager.Sound.guiSound1);
			}
			else if ( musicButton.enabled && musicButton.HitTest(Input.mousePosition) )
			{
				// if saved value is sounds enabled, then toggle it to disabled
				if ( PlayerPrefs.GetInt(PreferencesScript.TAG_MUSIC) == (int)PreferencesScript.Music.On ) 
				{
					PlayerPrefs.SetInt(PreferencesScript.TAG_MUSIC, (int)PreferencesScript.Music.Off);
					musicButton.text = "Music: Off";
					audioManager.stopAllMusic();
				}
				else
				{
					PlayerPrefs.SetInt(PreferencesScript.TAG_MUSIC, (int)PreferencesScript.Music.On);
					musicButton.text = "Music: On";
					audioManager.pauseMusic();
				}
				audioManager.playSound(AudioManager.Sound.guiSound1);
				
			}
			else if ( quitButton.enabled && quitButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				resumeGame();
				levelMgr.die();
			}
			else if ( gameOverTime > 0 && Time.time > gameOverTime + GAME_OVER_DELAY )
			{
				Application.LoadLevel(0);
			}
		}//end if
		
		// for animating the GUI
		if ( pauseMenuEnabled )
		{
			if ( pauseMenu.localPosition.x < activeMenuPosition.x - POSITION_CHECK )
				pauseMenu.localPosition = Vector2.Lerp(pauseMenu.localPosition, activeMenuPosition, MENU_LERP_SPEED);
			
			if ( pauseButton.transform.localPosition.x < disabledPauseButtonPosition.x - POSITION_CHECK )
			{
				pauseButton.transform.localPosition = Vector2.Lerp(pauseButton.transform.localPosition, disabledPauseButtonPosition, MENU_LERP_SPEED);
			}
		}
		else if ( !pauseMenuEnabled )
		{
			if ( pauseMenu.localPosition.x > disabledMenuPosition.x + POSITION_CHECK )
				pauseMenu.localPosition = Vector2.Lerp(pauseMenu.localPosition, disabledMenuPosition, MENU_LERP_SPEED);
			
			if ( pauseButton.transform.localPosition.x > activePauseButtonPosition.x + POSITION_CHECK )
			{
				pauseButton.transform.localPosition = Vector2.Lerp(pauseButton.transform.localPosition, activePauseButtonPosition, MENU_LERP_SPEED);
			}
		}
		
		if ( gameOverTime > 0 )
		{
			gameOverGui.localPosition = Vector2.Lerp(gameOverGui.localPosition, activeMenuPosition, MENU_LERP_SPEED);
			
			if ( pauseButton.transform.localPosition.x < disabledPauseButtonPosition.x - POSITION_CHECK )
				pauseButton.transform.localPosition = Vector2.Lerp(pauseButton.transform.localPosition, disabledPauseButtonPosition, MENU_LERP_SPEED);
		}		

	}//end Update
	
	public void setScoreText(int score)
	{
		currentScore = score;
	}
	
	public void setBonusText(int bonusPoints)
	{
		bonusPointsList.Add(bonusPoints);
		
		if ( bonusPointsList.Count > BONUS_TEXT_ARRAY_SIZE ) 
			bonusPointsList.RemoveAt(0);
		
		string text = "";
		for (int i = bonusPointsList.Count - 1; i >= 0; i-- )
		{
			text = text + "+" + bonusPointsList[i] + "\n";
		}
		
		bonusTime = Time.time;
	}
	
	public void showShieldInvulnerable(bool show)
	{
		if ( show )
		{
			shieldInvulnerableText.enabled = true;
		}
		else
			shieldInvulnerableText.enabled = false;
	}

	public void setFuelText(int fuel) 
	{
		if ( fuel == 0 )
			fuelRemainingText.text = "";
		else
		{
			string text = "";
			for ( int i = 0; i < fuel && i < MAX_GUI_BARS; i++ ) 
			{
				text = text + "|";
			}//end for
			fuelRemainingText.text = text;

			Debug.Log ("fuel = " + fuel + ", numPipes = " + text.Length);
		}//end else
	}

	// we'll draw three pipes ('|||') for each hp left
	public void setShieldText(int shield)
	{
		if ( shield == 0 )
			shieldTextRed.text = "";
		else
		{
			string text = "";
			for ( int i = 0; i < shield - 1; i++ ) 
			{
				text = text + "||||";
			}//end for
			shieldTextBlue.text = text;
		}//end else
	}//end setShieldText
	
	public void setSpeedText(float percent)
	{		
		int numBars = (int)(percent * MAX_GUI_BARS);
		
		if ( numBars == MAX_GUI_BARS )
			maxSpeedBonusText.enabled = true;
		else
			maxSpeedBonusText.enabled = false;
		
		string text = "";
		for ( int i = 0; i < numBars; i++ ) 
		{
			text = text + "|";
		}
		speedText.text = text;
	}
	
	private void pauseGame()
	{
		// save current player params in case we switch controller type
		PlayerPrefs.SetFloat("x", player.transform.localPosition.x);
	
		levelMgr.pauseGame();
		enablePauseMenu();
		pauseMenuEnabled = true;
	}
	
	public void resumeGame()
	{
		disablePauseMenu();
		levelMgr.resumeGame();
		pauseMenuEnabled = false;
	}
	
	public void enablePauseMenu()
	{
//		pauseButton.enabled = false;
		resumeButton.enabled = true;
		noFaceDetectedText.enabled = false;
	}
	
	public void disablePauseMenu()
	{
//		pauseButton.enabled = true;
		resumeButton.enabled = false;
	}
	
	public void showGameOver()
	{
		noFaceDetectedText.enabled = false;

		gameOverText.text = "Game Over";		
		pauseButton.enabled = false;
		pauseMenuEnabled = false;
		gameOverText.enabled = true;
		gameOverTextSub.enabled = true;
		
		gameOverTime = Time.time;
	}

	public void showGameFinished ()
	{
		noFaceDetectedText.enabled = false;
		
		gameOverText.text = "You Did It!";
		gameOverTextSub.text = "You beat the game!\n\n" + gameOverTextSub.text;

		pauseButton.enabled = false;
		pauseMenuEnabled = false;
		gameOverText.enabled = true;
		gameOverTextSub.enabled = true;
		
		gameOverTime = Time.time;
	}
	
	public void showNewHighScore(int score)
	{
		noFaceDetectedText.enabled = false;
		pauseButton.enabled = false;
		pauseMenuEnabled = false;
		gameOverText.enabled = true;
		gameOverTextSub.enabled = true;
		
		gameOverText.text = "New High Score!";
		highScoreText.text = "High Score: " + score;
		
		gameOverTime = Time.time;
	}
	
	public void showGameOverTrial(bool trialCompleted, int numHits)
	{
		noFaceDetectedText.enabled = false;
		pauseButton.enabled = false;
		pauseMenuEnabled = false;
		gameOverText.enabled = true;
		gameOverTextSub.enabled = true;
		
		if ( !trialCompleted )
			gameOverText.text = "Game Over";
		else
			gameOverText.text = "Congratulations!";
		
		gameOverTextSub.text = "You hit " + numHits + " obstacles";
		
		gameOverTime = Time.time;
	}
	
	public void showLeaderBoardMessage()
	{
		gameOverTextSub.text = "You made the leaderboard!\n\n" + gameOverTextSub.text;
	}
	
	public void faceDetected(bool isDetected)
	{
		// going from detected to not detected
		if ( wasFaceDetected && !isDetected )
		{
			wasFaceDetected = false;
			noFaceDetectedText.enabled = true;
			levelMgr.pauseGame();
		}
		else if ( !wasFaceDetected && isDetected )
		{
			wasFaceDetected = false;
			noFaceDetectedText.enabled = true;
			levelMgr.pauseGame();
		}			
	}
		
	public void flashScreen(Color screenFlashColor, float screenFlashAlpha)
	{
		// setup screen flash vars
		if ( screenFlashAlpha > 0f )
		{
			currScreenFlashColor = screenFlashColor;
			currScreenFlashAlpha = screenFlashAlpha;
		}
	}
	
	private void updateGui()
	{
		if ( currScreenFlashAlpha > 0f ) 
		{
			screenFlashGuiTexture.color = new Color(currScreenFlashColor.r, currScreenFlashColor.g, currScreenFlashColor.b, currScreenFlashAlpha);
			currScreenFlashAlpha -= SCREEN_FLASH_RECOV;
		}
		else 
			screenFlashGuiTexture.color = new Color(1, 1, 1, 0);	

		updateScore ();
	}//end updateGUI

	public void updateScore()
	{
		if (displayedScore < currentScore)
		{
			displayedScore = (int)(Mathf.Lerp(displayedScore, currentScore, Time.deltaTime));
			scoreText.text = "Score: " + displayedScore;
		}
		if (displayedScore < currentScore) 
		{
			displayedScore++;
			scoreText.text = "Score: " + displayedScore;
		}
	}	
}
