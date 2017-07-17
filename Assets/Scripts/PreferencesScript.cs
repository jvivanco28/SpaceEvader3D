using UnityEngine;
using System.Collections;

public class PreferencesScript : MonoBehaviour {
	
	public const string TAG_CONTROLLER = "Controller"; // totally got lost between these two... f*ck it
	public const string TAG_TOUCH_CONTROLLER = "TouchController";
	public const string TAG_SOUNDS = "Sounds";
	public const string TAG_MUSIC = "Music";
	public const string TAG_HIGHSCORE = "HighScore";
	public const string TAG_X = "x";
	public const string TAG_Y = "y";
	public const string TAG_FACE_WIDTH = "faceWidth";
	public const string TAG_FACE_HEIGHT = "faceHeight";
	public const string TAG_PLAYER_NAME = "PlayerName";
	public const string TAG_DEV_SPEED = "MAX_SPEED";
	public const string TAG_START_ZONE = "Zone";
	public const string KEY_POST_NAME = "Name";
	public const string KEY_POST_SCORE = "Score";
	public const string GET_HIGH_SCORE_URL = "http://130.179.130.142/spaceevader/getHighScoresWeb.php";
	public const string POST_HIGH_SCORE_URL = "http://130.179.130.142/spaceevader/sendHighScoreWeb.php";

	private GameObject player; // so we can change controllers
//	private LevelManagerScript levelMgrScript; // to change game speed (based on controllers)
	
	public enum Sounds
	{
		On = 0,
		Off = 1
	}
	
	public enum Music
	{
		On = 0,
		Off = 1
	}
	
	void Start ()
	{
		player = GameObject.Find("Player"); 
		
		// load the saved controller
		destroyCurrentControllerScript();
	}
	
	public IPlayerMove getCurrentControllerScript()
	{
		IPlayerMove controllerScript = null;
		int controller = PlayerPrefs.GetInt(TAG_CONTROLLER);

		if ( controller == (int)IPlayerMove.Controller.Absolute )
			controllerScript = player.GetComponent<PlayerMoveAbsScript>();
		else if ( controller == (int)IPlayerMove.Controller.Delta )
			controllerScript = player.GetComponent<PlayerMoveDeltaScript>();
		else if ( controller == (int)IPlayerMove.Controller.EyeTracker )
			controllerScript = player.GetComponent<PlayerMoveEyeTrackerScript>();
		else if ( controller == (int)IPlayerMove.Controller.Start )
			controllerScript = player.GetComponent<PlayerStartCamScript>();
		else if ( controller == (int)IPlayerMove.Controller.StartScreen )
			controllerScript = player.GetComponent<PlayerStartScreenOrbitScript>();
		else if ( controller == (int)IPlayerMove.Controller.Dead )
			controllerScript = player.GetComponent<PlayerDeadOrbitScript>();
		return controllerScript;
	}
	
	// set up controller with the last used controller
	public void setupController()
	{
		int controllerType = PlayerPrefs.GetInt(TAG_CONTROLLER);
		
		// the default controller MUST be one that can actually control the spaceship (this should never happen, but just in case)
		if ( (IPlayerMove.Controller)controllerType == IPlayerMove.Controller.Dead ||
			(IPlayerMove.Controller)controllerType == IPlayerMove.Controller.Start ||
		    (IPlayerMove.Controller)controllerType == IPlayerMove.Controller.Finished)
		{
			controllerType = (int)PlayerPrefs.GetInt(TAG_TOUCH_CONTROLLER);
			Debug.Log("WTF!");
		}
		
		setupController((IPlayerMove.Controller)controllerType);
	}
	
	public void setupController(IPlayerMove.Controller controller)
	{
		Debug.Log("Setting up " + controller);
		
		// if the user changes the controller during the opening scene,
		// make sure we put the camera in the POV of the space ship
		IPlayerMove controllerScript = player.GetComponent<IPlayerMove>();
		if ( controllerScript != null && controllerScript.getControllerType() == IPlayerMove.Controller.Start ) 
		{
			Transform cam = player.transform.FindChild("Near Camera");
			cam.localPosition = new Vector3(0,0,0);
			player.transform.FindChild("SpaceShip").GetComponent<Renderer>().enabled = false;
		}

		// when we switch a controller, we'll destroy the current script
		// from the player game object, then attach the newly selected 
		// controller script
		destroyCurrentControllerScript();
		if ( controller == IPlayerMove.Controller.Absolute )
		{
			Debug.Log("Using Absolute Touch Control");
			player.AddComponent<PlayerMoveAbsScript>();
		}
		else if ( controller == IPlayerMove.Controller.Delta )
		{
			Debug.Log("Using Touch Delta Control");
			player.AddComponent<PlayerMoveDeltaScript>();
		}
		else if ( controller == IPlayerMove.Controller.EyeTracker )
		{
			Debug.Log("Using Eye Tracker Control");
			player.AddComponent<PlayerMoveEyeTrackerScript>();
		}
		else if ( controller == IPlayerMove.Controller.Dead )
		{
			Debug.Log("Using Dead Orbit Camera");
			player.AddComponent<PlayerDeadOrbitScript>();
		}
		else if ( controller == IPlayerMove.Controller.Start )
		{
			Debug.Log("Using Start Camera");
			player.AddComponent<PlayerStartCamScript>();
		}
		else if ( controller == IPlayerMove.Controller.Finished )
		{
			Debug.Log("Using Finished Camera");
			player.AddComponent<PlayerFinishedOrbitScript>();
		}
	}
	
	public void destroyCurrentControllerScript()
	{
		Component[] controllerScripts = player.GetComponents<IPlayerMove>();

		if ( controllerScripts != null && controllerScripts.Length > 0)
		{
			for (int i = 0; i < controllerScripts.Length; i++ ) 
				Object.Destroy(controllerScripts[i]);
		}
	}

}//end class
