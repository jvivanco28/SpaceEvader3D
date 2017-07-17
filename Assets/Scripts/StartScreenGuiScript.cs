using UnityEngine;
using System.Collections;

public class StartScreenGuiScript : MonoBehaviour 
{
	public const float MENU_LERP_SPEED = 0.1f;
	public const float HIT_PLAY_DELAY = 2f;
	public const int NUM_HIGH_SCORES = 15;
	
//	private const string ALI_KHAN_ITUNES_LINK = "https://itunes.apple.com/ca/artist/ali-khan/id91684384";
	private const string ALI_KHAN_ITUNES_LINK = "http://dewtonerecordings.com";
	private const string MOBRO_WEBSITE = "http://mobrosoftware.com";
	private const string FACEBOOK_PAGE = "https://www.facebook.com/240507036105380";
	
	private const string ZONE_1_TITLE = "Apollo";
	private const string ZONE_2_TITLE = "Demeter";
	private const string ZONE_3_TITLE = "Hephaestus";
	private const string ZONE_4_TITLE = "Coeus";
	private const string ZONE_5_TITLE = "Ares";
	private const string ZONE_6_TITLE = "Dionysus";	
	
	public Transform mainButtonsTransform;
	public GUIText playButton;
	public GUIText instructionsButton;
	public GUIText profileButton;
	public GUIText highScoresButton;
	public GUIText optionsButton;
	public GUIText moreMusicButton;
	public GUIText creditsButton;
	public GUITexture mobroLogo;
	public GUITexture facebookLogo;
	
	public Transform zoneSelectionTransform;
	public GUIText zone1Button;
	public GUIText zone2Button;
	public GUIText zone3Button;
	public GUIText zone4Button;
	public GUIText zone5Button;
	public GUIText zone6Button;
	public GUITexture zonePic;
	public GUIText zoneSelectionBackButton;
	public GUIText zoneTitle;
	public GUIText requiredHighScore;
	public GUIText highscoreText;
	public GUIText zonePlayButton;
	public GUITexture selectionHighlighter;
	
	public Transform instructionsTransform;
	public Transform instructionsPart1;
	public Transform instructionsPart2;
	public Transform instructionsPart3;
	public Transform instructionsPart4;
//	public Transform instructionsPart5;
	public GUIText instructionsNextButton;
	public GUIText instructionsBackButton;
	private int instructionsIndex;

	public Transform playerNameTransform;
	public GUIText enterPlayerNameButton;
	
//	public Transform playOptionsButtonsTransform;
//	public GUIText playTouchButton;
//	public GUIText playWithHeadTrackerButton;
//	public GUIText playOptionsBackButton;
	public GUIText loadingText;
	
	public Transform optionsMenuTransform;
	public GUIText soundsButton;
	public GUIText musicButton;
	public GUIText optionsBackButton;
//	public GUIText setMaxSpeedButton;
	
	public Transform highScoresTransform;
	public GUIText highScoresBackButton;
	
	public Transform creditsTransform;
	public GUIText creditsBackButton;

	public GUISkin guiSkin;

	private AudioManager audioManager;
	
	private Vector2 leftOffScreenPos;
	private Vector2 rightOffScreenPos;
	private Vector2 onScreenPos;
	private Vector2 selectionHighlighterPosition;
	
//	private TouchScreenKeyboard keyboard;
//	private string textEntry;
	
//	private bool showControllerOptions;
	private bool showOptionsMenu;
	private bool showHighScores;
	private bool showZoneSelectionMenu;
	private bool showInstructionsMenu;
	private bool showCredits;
	private bool showPlayerNameTextBox;

	private float hitPlayTime;
	private LevelDescription.SkyBoxes selectedZone;
	private int highscore;
	
	private GameObject 	screenFlashObj;
	private GUITexture 	screenFlashGuiTexture;
	private Texture2D 	screenFlashTexture2D;
	private Color 		currScreenFlashColor; // we'll use this to store the rgb values
	private float 		currScreenFlashAlpha; // we'll use this to update the alpha channel separately
	private bool 		fadeScreenBlack;

	private string playerNameText = "Player";

//	// TODO: delete this
//	bool settingMaxSpeed;
//	int maxSpeed;
//	string maxSpeedText;
	
	void Awake ()
	{
		hitPlayTime = -1;
		
		leftOffScreenPos = new Vector2(-2,0);
		rightOffScreenPos = new Vector2(2,0);
		onScreenPos = new Vector2(0,0);
		
//		showControllerOptions = false;
		showOptionsMenu = false;
		showHighScores = false;
		showZoneSelectionMenu = false;
		showInstructionsMenu = false;
		showCredits = false;
		instructionsIndex = 0;
	}
	
	void Start()
	{	
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
				
		audioManager = GameObject.Find("Level").GetComponent<AudioManager>();
		
		// create our damage texture of size 1 and 1
		screenFlashTexture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		
		// set pixel values
		screenFlashTexture2D.SetPixel(0, 0, Color.white);

		// Apply all SetPixel calls
		screenFlashTexture2D.Apply();
		
	    // connect texture to material of GUI
		screenFlashObj = new GameObject("DamageGUI");
		screenFlashObj.transform.localScale = new Vector3(1, 1, 0);
		screenFlashObj.transform.localPosition = new Vector3(0, 0, 1);
		
		screenFlashGuiTexture = screenFlashObj.AddComponent<GUITexture>();
	    screenFlashGuiTexture.pixelInset = new Rect(0 , 0 , Screen.width , Screen.height );
		screenFlashGuiTexture.texture = screenFlashTexture2D;
		screenFlashGuiTexture.enabled = true;
		
		fadeScreenBlack = false;
		
		// zone selection menu
		selectedZone = LevelDescription.SkyBoxes.DeepSpaceBlue;
		zonePic.texture = (Texture)Resources.Load("Textures/Zone1");
		zoneTitle.text = ZONE_1_TITLE;
		requiredHighScore.text = "Required Score: 0";
		
		highscore = PlayerPrefs.GetInt(PreferencesScript.TAG_HIGHSCORE, 0);
		highscoreText.text = "High Score: " + highscore;
		
		selectionHighlighterPosition.x = 0.02f;
		selectionHighlighterPosition.y = zone1Button.transform.localPosition.y;
		
		if ( highscore < LevelDescription.ZONE_2_UNLOCK )
			zone2Button.material.color = Color.red;
		if ( highscore < LevelDescription.ZONE_3_UNLOCK )
			zone3Button.material.color = Color.red;
		if ( highscore < LevelDescription.ZONE_4_UNLOCK )
			zone4Button.material.color = Color.red;
		if ( highscore < LevelDescription.ZONE_5_UNLOCK )
			zone5Button.material.color = Color.red;
		if ( highscore < LevelDescription.ZONE_6_UNLOCK )
			zone6Button.material.color = Color.red;
		
		instructionsPart1.gameObject.SetActive(true);
		instructionsPart2.gameObject.SetActive(false);
		instructionsPart3.gameObject.SetActive(false);
		instructionsPart4.gameObject.SetActive(false);
//		instructionsPart5.gameObject.SetActive(false);

		playerNameText = PlayerPrefs.GetString(PreferencesScript.TAG_PLAYER_NAME);
		if (playerNameText == null || playerNameText.Length == 0) 
			playerNameText = "Player";
	}
	
	void OnGUI()
	{

		if (showPlayerNameTextBox) {
			GUI.skin = guiSkin;
//			playerNameText = GUI.TextField (new Rect ((Screen.width / 2) - 300, 400, 600, 50), playerNameText, 25);
			playerNameText = GUI.TextField (new Rect ((Screen.width / 2) - 300, (Screen.height / 2), 600, 100), playerNameText, 25);
		}

//		if ( keyboard != null && keyboard.active )
//		{
//			textEntry = PlayerPrefs.GetString(PreferencesScript.TAG_PLAYER_NAME);
//			// THIS LOOKS WRONG BTW!!!
//			if ( textEntry != null )
//				textEntry = "Enter your name";
//			
//			textEntry = keyboard.text;
//			PlayerPrefs.SetString(PreferencesScript.TAG_PLAYER_NAME, textEntry);
//		}	
	}
	
	void LateUpdate()
	{	
		// wait a second after pressing the play button to let the GUI sound play out
		if ( hitPlayTime > 0 && Time.time > hitPlayTime + HIT_PLAY_DELAY )
		{
			// make sure the player's name only contains letters and numbers before we start the game
			string playerName = PlayerPrefs.GetString(PreferencesScript.TAG_PLAYER_NAME);
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
			
			// make sure it's not an empty string
			if ( playerNameCleaned.Equals("") )
				playerNameCleaned = "Player";
			
			PlayerPrefs.SetString(PreferencesScript.TAG_PLAYER_NAME, playerNameCleaned);
			Application.LoadLevel(1);
		}
		
//		if ( showControllerOptions )
//		{
//			mainButtonsTransform.localPosition = Vector2.Lerp(mainButtonsTransform.localPosition, leftOffScreenPos, MENU_LERP_SPEED);
//			playOptionsButtonsTransform.localPosition = Vector2.Lerp(playOptionsButtonsTransform.localPosition, onScreenPos, MENU_LERP_SPEED);
//		}
		if ( showOptionsMenu )
		{
			mainButtonsTransform.localPosition = Vector2.Lerp(mainButtonsTransform.localPosition, leftOffScreenPos, MENU_LERP_SPEED);
			optionsMenuTransform.localPosition = Vector2.Lerp(optionsMenuTransform.localPosition, onScreenPos, MENU_LERP_SPEED);
		}
		else if ( showHighScores )
		{
			mainButtonsTransform.localPosition = Vector2.Lerp(mainButtonsTransform.localPosition, leftOffScreenPos, MENU_LERP_SPEED);
			highScoresTransform.localPosition = Vector2.Lerp(highScoresTransform.localPosition, onScreenPos, MENU_LERP_SPEED);
		}
		else if ( showZoneSelectionMenu ) 
		{
			mainButtonsTransform.localPosition = Vector2.Lerp(mainButtonsTransform.localPosition, leftOffScreenPos, MENU_LERP_SPEED);
			zoneSelectionTransform.localPosition = Vector2.Lerp(zoneSelectionTransform.localPosition, onScreenPos, MENU_LERP_SPEED);
			
			updateZoneHighlighterCoordinates();
		}
		else if ( showInstructionsMenu )
		{
			mainButtonsTransform.localPosition = Vector2.Lerp(mainButtonsTransform.localPosition, leftOffScreenPos, MENU_LERP_SPEED);
			instructionsTransform.localPosition = Vector2.Lerp(instructionsTransform.localPosition, onScreenPos, MENU_LERP_SPEED);
		}
		else if ( showCredits )
		{
			mainButtonsTransform.localPosition = Vector2.Lerp(mainButtonsTransform.localPosition, leftOffScreenPos, MENU_LERP_SPEED);
			creditsTransform.localPosition = Vector2.Lerp(creditsTransform.localPosition, onScreenPos, MENU_LERP_SPEED);
		}
		else if ( showPlayerNameTextBox ) 
		{
			mainButtonsTransform.localPosition = Vector2.Lerp(mainButtonsTransform.localPosition, leftOffScreenPos, MENU_LERP_SPEED);
			playerNameTransform.localPosition = Vector2.Lerp(playerNameTransform.localPosition, onScreenPos, MENU_LERP_SPEED);
		}
		else
		{
			mainButtonsTransform.localPosition = Vector2.Lerp(mainButtonsTransform.localPosition, onScreenPos, MENU_LERP_SPEED);
//			playOptionsButtonsTransform.localPosition = Vector2.Lerp(playOptionsButtonsTransform.localPosition, rightOffScreenPos, MENU_LERP_SPEED);
			optionsMenuTransform.localPosition = Vector2.Lerp(optionsMenuTransform.localPosition, rightOffScreenPos, MENU_LERP_SPEED);
			highScoresTransform.localPosition = Vector2.Lerp(highScoresTransform.localPosition, rightOffScreenPos, MENU_LERP_SPEED);
			zoneSelectionTransform.localPosition = Vector2.Lerp(zoneSelectionTransform.localPosition, rightOffScreenPos, MENU_LERP_SPEED);
			instructionsTransform.localPosition = Vector2.Lerp(instructionsTransform.localPosition, rightOffScreenPos, MENU_LERP_SPEED);
			creditsTransform.localPosition = Vector2.Lerp(creditsTransform.localPosition, rightOffScreenPos, MENU_LERP_SPEED);
			playerNameTransform.localPosition = Vector2.Lerp(playerNameTransform.localPosition, rightOffScreenPos, MENU_LERP_SPEED);
		}
		
		if ( Input.GetMouseButtonDown(0) )
		{
			// MAIN MENU
//			if ( hitPlayTime < 0 && playButton.HitTest(Input.mousePosition) )
			if ( playButton.HitTest(Input.mousePosition) )
			{	
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showZoneSelectionMenu = true;
			}
			else if ( instructionsButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showInstructionsMenu = true;
			}
			else if ( optionsButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showOptionsMenu = true;
			}
			else if ( highScoresButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showHighScores = true;
				clearHighScores();
				
				// Grab scores from server
				string url = PreferencesScript.GET_HIGH_SCORE_URL;
				WWW www = new WWW(url);
				StartCoroutine(requestHighScores(www));
			}
			else if ( profileButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showPlayerNameTextBox = true;
//				keyboard = TouchScreenKeyboard.Open(PlayerPrefs.GetString(PreferencesScript.TAG_PLAYER_NAME, "Enter your name (alphanumeric only)"),
//													TouchScreenKeyboardType.EmailAddress, false, false, false, false);				
			}
			else if ( enterPlayerNameButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showPlayerNameTextBox = false;

				PlayerPrefs.SetString(PreferencesScript.TAG_PLAYER_NAME, playerNameText);
			}
			else if ( creditsButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showCredits = true;
			}
			else if ( moreMusicButton.HitTest(Input.mousePosition) )
			{
				Application.OpenURL (ALI_KHAN_ITUNES_LINK);
			}
			else if ( mobroLogo.HitTest(Input.mousePosition) )
			{
				Application.OpenURL (MOBRO_WEBSITE);		
			}
			else if ( facebookLogo.HitTest(Input.mousePosition) )
			{
				Application.OpenURL (FACEBOOK_PAGE);
			}
			
//			// PLAY MENU
//			else if ( hitPlayTime < 0 && playTouchButton.HitTest(Input.mousePosition) )
//			{
//				// set the controller to the prefered touch controller (if no touch controller specified, then the 
//				// default touch controller will be to use deltas)
//				PlayerPrefs.SetInt(PreferencesScript.TAG_CONTROLLER, PlayerPrefs.GetInt(PreferencesScript.TAG_TOUCH_CONTROLLER, (int)IPlayerMove.Controller.Delta));
//				audioManager.stopAllMusic();
//				audioManager.playSound(AudioManager.Sound.guiSound2);
//				hitPlayTime = Time.time;
//				fadeScreenBlack = true;				
//			}
//			else if ( hitPlayTime < 0 && playWithHeadTrackerButton.HitTest(Input.mousePosition) )
//			{
//				PlayerPrefs.SetInt(PreferencesScript.TAG_CONTROLLER, (int)IPlayerMove.Controller.EyeTracker);
//				audioManager.stopAllMusic();
//				audioManager.playSound(AudioManager.Sound.guiSound2);
//				hitPlayTime = Time.time;
//				fadeScreenBlack = true;				
//			}
//			else if ( playOptionsBackButton.HitTest(Input.mousePosition) )
//			{
//				audioManager.playSound(AudioManager.Sound.guiSound1);
//				showControllerOptions = false;
//			}
			
			// OPTIONS MENU
			else if ( optionsBackButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showOptionsMenu = false;
			}
			else if ( zone1Button.HitTest(Input.mousePosition) ) 
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				selectedZone = LevelDescription.SkyBoxes.DeepSpaceBlue;
				zonePic.texture = (Texture)Resources.Load("Textures/Zone1");
				zoneTitle.text = ZONE_1_TITLE;
				requiredHighScore.text = "Required Score: 0";
				
				zonePlayButton.material.color = Color.white;
				zoneTitle.material.color = Color.white;
				highscoreText.material.color = Color.white;
				requiredHighScore.material.color = Color.white;
			}
			else if ( zone2Button.HitTest(Input.mousePosition) ) 
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				selectedZone = LevelDescription.SkyBoxes.DeepSpaceGreen;
				zonePic.texture = (Texture)Resources.Load("Textures/Zone2");
				zoneTitle.text = ZONE_2_TITLE;
				requiredHighScore.text = "Required Score: " + LevelDescription.ZONE_2_UNLOCK;
				checkScoreRequirements(LevelDescription.ZONE_2_UNLOCK);
			}
			else if ( zone3Button.HitTest(Input.mousePosition) ) 
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				selectedZone = LevelDescription.SkyBoxes.DeepSpaceGreenWithPlanet;
				zonePic.texture = (Texture)Resources.Load("Textures/Zone3");
				zoneTitle.text = ZONE_3_TITLE;
				requiredHighScore.text = "Required Score: " + LevelDescription.ZONE_3_UNLOCK;
				checkScoreRequirements(LevelDescription.ZONE_3_UNLOCK);
			}
			else if ( zone4Button.HitTest(Input.mousePosition) ) 
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				selectedZone = LevelDescription.SkyBoxes.DeepSpaceRed;
				zonePic.texture = (Texture)Resources.Load("Textures/Zone4");
				zoneTitle.text = ZONE_4_TITLE;
				requiredHighScore.text = "Required Score: " + LevelDescription.ZONE_4_UNLOCK;
				checkScoreRequirements(LevelDescription.ZONE_4_UNLOCK);
			}
			else if ( zone5Button.HitTest(Input.mousePosition) ) 
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				selectedZone = LevelDescription.SkyBoxes.DeepSpaceRedWithPlanet;
				zonePic.texture = (Texture)Resources.Load("Textures/Zone5");
				zoneTitle.text = ZONE_5_TITLE;
				requiredHighScore.text = "Required Score: " + LevelDescription.ZONE_5_UNLOCK;
				checkScoreRequirements(LevelDescription.ZONE_5_UNLOCK);
			}
			else if ( zone6Button.HitTest(Input.mousePosition) ) 
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				selectedZone = LevelDescription.SkyBoxes.Stars;
				zonePic.texture = (Texture)Resources.Load("Textures/Zone6");
				zoneTitle.text = ZONE_6_TITLE;
				requiredHighScore.text = "Required Score: " + LevelDescription.ZONE_6_UNLOCK;
				checkScoreRequirements(LevelDescription.ZONE_6_UNLOCK);
			}
			else if ( zonePlayButton.HitTest(Input.mousePosition) )
			{
				bool canPlayZone = false;
				
				if ( selectedZone == LevelDescription.SkyBoxes.DeepSpaceBlue )
					canPlayZone = true;
				else if ( selectedZone == LevelDescription.SkyBoxes.DeepSpaceGreen )
				{
					if ( highscore >= LevelDescription.ZONE_2_UNLOCK )
						canPlayZone = true;
				}
				else if ( selectedZone == LevelDescription.SkyBoxes.DeepSpaceGreenWithPlanet )
				{
					if ( highscore >= LevelDescription.ZONE_3_UNLOCK )
						canPlayZone = true;					
				}
				else if ( selectedZone == LevelDescription.SkyBoxes.DeepSpaceRed )
				{
					if ( highscore >= LevelDescription.ZONE_4_UNLOCK )
						canPlayZone = true;
				}
				else if ( selectedZone == LevelDescription.SkyBoxes.DeepSpaceRedWithPlanet )
				{
					if ( highscore >= LevelDescription.ZONE_5_UNLOCK )
						canPlayZone = true;
				}
				else if ( selectedZone == LevelDescription.SkyBoxes.Stars )
				{
					if ( highscore >= LevelDescription.ZONE_6_UNLOCK )
						canPlayZone = true;
				}
				
				if ( canPlayZone )
				{
					if ( hitPlayTime < 0 )
					{
						PlayerPrefs.SetInt(PreferencesScript.TAG_START_ZONE, (int)(selectedZone));
						
						// always use absolute control (the mouse position) for desktops and laptops
						if ( SystemInfo.deviceType == DeviceType.Desktop )
							PlayerPrefs.SetInt(PreferencesScript.TAG_CONTROLLER, (int)IPlayerMove.Controller.Absolute);
		
						// or use deltas for handheld devices
						else 
							PlayerPrefs.SetInt(PreferencesScript.TAG_CONTROLLER, (int)IPlayerMove.Controller.Delta);
						
						audioManager.stopAllMusic();
						audioManager.playSound(AudioManager.Sound.guiSound2);
						hitPlayTime = Time.time;
						fadeScreenBlack = true;
					}
				}
				else
				{
					audioManager.playSound(AudioManager.Sound.errorSound);
				}
			}
//			else if ( touchControlButton.HitTest(Input.mousePosition) )
//			{
//				audioManager.playSound(AudioManager.Sound.guiSound1);
//				
//				// if the last saved touch controller was absolute touch, then swap in delta touch
//				if ( PlayerPrefs.GetInt(PreferencesScript.TAG_TOUCH_CONTROLLER) == (int)IPlayerMove.Controller.Absolute ) 
//				{
//					PlayerPrefs.SetInt(PreferencesScript.TAG_TOUCH_CONTROLLER, (int)IPlayerMove.Controller.Delta);
//					touchControlButton.text = "Touch control: Swipe";
//				}
//				else
//				{
//					PlayerPrefs.SetInt(PreferencesScript.TAG_TOUCH_CONTROLLER, (int)IPlayerMove.Controller.Absolute);
//					touchControlButton.text = "Touch control: Absolute";
//				}
//			}
			// OPTOINS MENU
			else if ( soundsButton.HitTest(Input.mousePosition) )
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
			else if ( musicButton.HitTest(Input.mousePosition) )
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
					audioManager.playGameMusic();
				}
				audioManager.playSound(AudioManager.Sound.guiSound1);
			}
//			else if ( setMaxSpeedButton.HitTest(Input.mousePosition) )
//			{
//				settingMaxSpeed = true;
////				keyboard = TouchScreenKeyboard.Open(PlayerPrefs.
//				keyboard = TouchScreenKeyboard.Open("" + PlayerPrefs.GetInt(PreferencesScript.TAG_DEV_SPEED, 57), TouchScreenKeyboardType.NumberPad, false, false, false, false);	
//			}
			
			// HIGH SCORES
			else if ( highScoresBackButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showHighScores = false;
			}
			
			// ZONE SELECTION MENU
			else if ( zoneSelectionBackButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showZoneSelectionMenu = false;
			}
			
			// INSTRUCTIONS MENU
			else if ( instructionsNextButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				instructionsIndex = (instructionsIndex + 1) % 4;
				
				instructionsPart1.gameObject.SetActive(false);
				instructionsPart2.gameObject.SetActive(false);
				instructionsPart3.gameObject.SetActive(false);
				instructionsPart4.gameObject.SetActive(false);
//				instructionsPart5.gameObject.SetActive(false);
				
				if ( instructionsIndex == 0 )
					instructionsPart1.gameObject.SetActive(true);
				else if ( instructionsIndex == 1 )
					instructionsPart2.gameObject.SetActive(true);
				else if ( instructionsIndex == 2 )
					instructionsPart3.gameObject.SetActive(true);
				else if ( instructionsIndex == 3 )
					instructionsPart4.gameObject.SetActive(true);
//				else if ( instructionsIndex == 4 )
//					instructionsPart5.gameObject.SetActive(true);
			}
			else if ( instructionsBackButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showInstructionsMenu = false;
			}
			// CREDITS MENU
			else if ( creditsBackButton.HitTest(Input.mousePosition) )
			{
				audioManager.playSound(AudioManager.Sound.guiSound1);
				showCredits = false;
			}
			
		}//end if
		
		updateGui();
	}//end LateUpdate
	
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
		if ( currScreenFlashAlpha > 0.5f ) 
		{
			loadingText.enabled = true;
		}
		if ( fadeScreenBlack && currScreenFlashAlpha < 1f )
		{
			screenFlashGuiTexture.color = new Color(currScreenFlashColor.r, currScreenFlashColor.g, currScreenFlashColor.b, currScreenFlashAlpha);
			currScreenFlashAlpha += GuiScript.SCREEN_FLASH_RECOV;
		}
		else if ( currScreenFlashAlpha > 0f ) 
		{
			screenFlashGuiTexture.color = new Color(currScreenFlashColor.r, currScreenFlashColor.g, currScreenFlashColor.b, currScreenFlashAlpha);
			currScreenFlashAlpha -= GuiScript.SCREEN_FLASH_RECOV;
		}
		else 
			screenFlashGuiTexture.color = new Color(1, 1, 1, 0);	
	}//end updateGUI
	
	private void updateZoneHighlighterCoordinates()
	{
		if ( selectedZone == LevelDescription.SkyBoxes.DeepSpaceGreen )
		{
			selectionHighlighterPosition.y = zone2Button.transform.localPosition.y - 0.0075f;			
		}
		else if ( selectedZone == LevelDescription.SkyBoxes.DeepSpaceGreenWithPlanet )
		{
			selectionHighlighterPosition.y = zone3Button.transform.localPosition.y - 0.0075f;			
		}
		else if ( selectedZone == LevelDescription.SkyBoxes.DeepSpaceRed )
		{
			selectionHighlighterPosition.y = zone4Button.transform.localPosition.y - 0.0075f;			
		}
		else if ( selectedZone == LevelDescription.SkyBoxes.DeepSpaceRedWithPlanet )
		{
			selectionHighlighterPosition.y = zone5Button.transform.localPosition.y - 0.0075f;			
		}
		else if ( selectedZone == LevelDescription.SkyBoxes.Stars )
		{
			selectionHighlighterPosition.y = zone6Button.transform.localPosition.y - 0.0075f;			
		}
		else
		{
			selectionHighlighterPosition.y = zone1Button.transform.localPosition.y - 0.0075f;			
		}
		
		selectionHighlighter.transform.localPosition = Vector2.Lerp(selectionHighlighter.transform.localPosition, selectionHighlighterPosition, MENU_LERP_SPEED);
	}
	
	private void checkScoreRequirements(int requiredScore)
	{
		if ( highscore < requiredScore )
		{
			zonePlayButton.material.color = Color.red;
			zoneTitle.material.color = Color.red;
			highscoreText.material.color = Color.red;
			requiredHighScore.material.color = Color.red;
		}
		else
		{
			zonePlayButton.material.color = Color.white;
			zoneTitle.material.color = Color.white;
			highscoreText.material.color = Color.white;
			requiredHighScore.material.color = Color.white;					
		}	
	}
	
	private void clearHighScores()
	{
		for ( int i = 0; i < NUM_HIGH_SCORES; i++ )
		{
			GameObject go = GameObject.Find("HighScore" + (i + 1));
			if ( go != null ) 
			{
				GUIText guiText = go.GetComponent<GUIText>();
				if ( guiText != null )
				{
					guiText.text = "";
					Transform goScore = go.transform.FindChild("Score");
					if ( goScore != null )
					{
						GUIText guiScoreText = goScore.GetComponent<GUIText>();
						if ( guiScoreText != null)
						{
							guiScoreText.text = "";
						}//end if
					}//end if
				}//end if
			}//end if
		}//end for
	}//end clearHighScores
	
	private IEnumerator requestHighScores(WWW www)
	{
		yield return www;
		
		GameObject go = GameObject.Find("HighScore1");
		if ( go != null )
		{
			GUIText guiText = go.GetComponent<GUIText>();
			if ( guiText != null )
			{
				guiText.text = "Contacting Server...";
			}//end if
		}//end if
		
		// check for errors
		if ( www.error == null )
		{
//			Debug.Log(www.text);
			string[] scores = www.text.Split('\n');
			for ( int i = 0; i < scores.Length; i++ ) 
			{
//				Debug.Log(scores[i]);
				if ( scores[i].ToString().Length > 0 )
				{
					go = GameObject.Find("HighScore" + (i + 1));
					if ( go != null ) 
					{
						GUIText guiText = go.GetComponent<GUIText>();
						if ( guiText != null )
						{
							string[] nameValuePair = scores[i].ToString().Split(',');
							if ( nameValuePair != null && nameValuePair.Length == 2 )
							{
								guiText.text = (i + 1) + ". " + nameValuePair[0];
								Transform goScore = go.transform.FindChild("Score");
								if ( goScore != null )
								{
									GUIText guiScoreText = goScore.GetComponent<GUIText>();
									if ( guiScoreText != null)
									{
										guiScoreText.text = nameValuePair[1];
									}//end if
								}//end if
							}//end if							
						}//end if
					}//end if
				}//end if
			}//end for
		}//end if
		else
		{
			Debug.Log(www.error);
			go = GameObject.Find("HighScore1");
			if ( go != null ) 
			{
				GUIText guiText = go.GetComponent<GUIText>();
				if ( guiText )
				{
					guiText.text = "No response from server.";
				}
			}
		}
	}
}
