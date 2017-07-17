using UnityEngine;
using System.Collections;

public class PlayerMoveEyeTrackerScript : IPlayerMove {
	
	public const int HUD_LINE_WIDTH = 2;
	public const float SENSITIVITY = 3f;
	
	public float lerp_speed = 2f;
	
	// This controller needs a HUD to let the user know where his/her face
	// is located, and how close their face is to the screen. They need to know
	// this since the eye tracker can not give a position update if his/her
	// face rect is even the slightest bit off screen
	private GameObject faceTopObj;
	private GameObject faceBottomObj;
	private GameObject faceLeftObj;
	private GameObject faceRightObj;
	
	// need a texture for each line =\
	private GUITexture faceTopTexture;
	private Texture2D faceTopTexture2D;
	private Rect faceTopLine;
	
	private GUITexture faceBottomTexture;
	private Texture2D faceBottomTexture2D;
	private Rect faceBottomLine;
	
	private GUITexture faceLeftTexture;
	private Texture2D faceLeftTexture2D;
	private Rect faceLeftLine;
	
	private GUITexture faceRightTexture;
	private Texture2D faceRightTexture2D;
	private Rect faceRightLine;
	
	// face origin
	private float faceOriginRatioX;
	private float faceOriginRatioY;
	private float faceWidthRatio;
	private float faceHeightRatio;
	private int faceOriginX;
	private int faceOriginY;
	private int faceWidth;
	private int faceHeight;
	
	private GuiScript guiScript;
	
	// Use this for initialization
	void Start () {
		PlayerPrefs.SetInt(PreferencesScript.TAG_CONTROLLER, (int)Controller.EyeTracker);
		
		// set look-at object
		if ( cameraLookAt == null )
		{
			cameraLookAt = GameObject.Find("CameraLookAt").transform;
		}
		give = 1f;
		faceTopObj = GameObject.Find("FaceTopLine");
		faceBottomObj = GameObject.Find("FaceBottomLine");
		faceLeftObj = GameObject.Find("FaceLeftLine");
		faceRightObj = GameObject.Find("FaceRightLine");
		
		GameObject guiObj = GameObject.Find("GUI");
		guiScript = guiObj.GetComponent<GuiScript>();

		enableFaceHUD();
	}//end Start
	
	// Update is called once per frame
	void LateUpdate () {
		if ( !isPaused )
		{
			// coordinates are passed from xcode since we're using the EyeTracker API
			// NOTE: these numbers are ratios (0 to 1)
			faceOriginRatioX = PlayerPrefs.GetFloat(PreferencesScript.TAG_X);
			faceOriginRatioY = PlayerPrefs.GetFloat(PreferencesScript.TAG_Y);
			faceWidthRatio = PlayerPrefs.GetFloat(PreferencesScript.TAG_FACE_WIDTH);
			faceHeightRatio = PlayerPrefs.GetFloat(PreferencesScript.TAG_FACE_HEIGHT);
			
			// set face rect
			if ( faceOriginRatioX >= 0 && faceOriginRatioX <= 1 && faceOriginRatioY >= 0 && faceOriginRatioY <= 1 )
			{
				guiScript.faceDetected(true);

				// convert ratios into actual screen coordinates
				faceWidth = (int)(faceWidthRatio * Screen.width);
				faceHeight = (int)(faceHeightRatio * Screen.height);
				faceOriginX = (int)(faceOriginRatioX * Screen.width);
				faceOriginY = (int)(faceOriginRatioY * Screen.height);
				
				// set the coordinates of the face rect on the HUD
				faceTopLine.width = faceWidth;
				faceTopLine.height = HUD_LINE_WIDTH;
				faceTopLine.x = faceOriginX - (faceWidth * 0.5f);
				faceTopLine.y = faceOriginY + (faceHeight * 0.5f);
				faceTopTexture.pixelInset = faceTopLine;
				
				faceBottomLine.width = faceWidth;
				faceBottomLine.height = HUD_LINE_WIDTH;
				faceBottomLine.x = faceOriginX - (faceWidth * 0.5f);
				faceBottomLine.y = faceOriginY - (faceHeight * 0.5f);
				faceBottomTexture.pixelInset = faceBottomLine;
				
				faceLeftLine.width = HUD_LINE_WIDTH;
				faceLeftLine.height = faceHeight;
				faceLeftLine.x = faceOriginX - (faceWidth * 0.5f);
				faceLeftLine.y = faceOriginY - (faceHeight * 0.5f);
				faceLeftTexture.pixelInset = faceLeftLine;
				
				faceRightLine.width = HUD_LINE_WIDTH;
				faceRightLine.height = faceHeight;
				faceRightLine.x = faceOriginX + (faceWidth * 0.5f);
				faceRightLine.y = faceOriginY - (faceHeight * 0.5f);
				faceRightTexture.pixelInset = faceRightLine;
				
				// convert ratios into scaled numbers
				xScaled = (faceOriginRatioX * 10) - 5;
				xScaled *= SENSITIVITY;
				if ( xScaled > maxX - give )
					xScaled = maxX - give;
				else if ( xScaled < minX + give)
					xScaled = minX + give;
				
				yScaled = (faceOriginRatioY * 10) - 5;
				yScaled *= SENSITIVITY;
				if ( yScaled > maxY - give)
					yScaled = maxY - give;
				else if ( yScaled < minY + give)
					yScaled = minY + give;
				
				// create a new vector of the desired position
				Vector3 newPositionVector;
				if ( currUpVector == PlayerUpVector.worldUp )
					newPositionVector = new Vector3(xScaled, yScaled, 0);
				else if ( currUpVector == PlayerUpVector.worldLeft)
					newPositionVector = new Vector3(-yScaled, xScaled, 0);
				else if ( currUpVector == PlayerUpVector.worldDown)
					newPositionVector = new Vector3(-xScaled, -yScaled, 0);
				else
					newPositionVector = new Vector3(yScaled, -xScaled, 0);
				
				// move to the new absolute position
				transform.localPosition = Vector3.Lerp(transform.localPosition, newPositionVector, Time.deltaTime * lerp_speed);
				
				// keep looking at an invisible point somwehere down the middle of the tunnel
				if ( cameraLookAt != null ) 
				{
					Transform cam = this.transform.FindChild("Near Camera");
					cam.transform.LookAt(cameraLookAt);
				}//end if
				
				// if we've hit a rotation powerup, then rotate to the new goal angle
				if ( (int)transform.eulerAngles.z != (int)goalAngle )
				{
					float newAngle = Mathf.LerpAngle(transform.eulerAngles.z, goalAngle, Time.deltaTime * lerp_speed);
					transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, newAngle);
				}//end if
				
				// keep looking at an invisible point somwehere down the middle of the tunnel
				if ( cameraLookAt != null ) 
				{
					Transform cam = this.transform.FindChild("Near Camera");
					cam.LookAt(cameraLookAt, this.transform.up);	
				}//end if
			}//end if
			// else pause the game b/c no face is detected
			else
			{
				guiScript.faceDetected(false);
			}
//			Debug.Log("absolute vals = " + xAbsolute + " , " + yAbsolute);
//			Debug.Log("scaled vals = " + faceOriginRatioX + " , " + faceOriginRatioY);
		}//end if
	}//end Update
	
	public override Controller getControllerType()
	{
		return Controller.EyeTracker;
	}
	
	void OnDisable() 
	{
 		disableFaceHUD();
    }

	
	private void enableFaceHUD()
	{	
		if ( faceTopObj.GetComponent<GUITexture>() == null )
		{
			faceTopTexture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			faceTopTexture2D.SetPixel(0, 0, Color.green);
			faceTopTexture2D.Apply();
			
		    // connect textures to material of GUI
//			faceTopObj = GameObject.Find("FaceTopLine");
			faceTopObj.transform.localScale = Vector3.zero;
			
			// create the gui Textures
			faceTopLine = new Rect();
			faceTopTexture = faceTopObj.AddComponent<GUITexture>();
			faceTopTexture.texture = faceTopTexture2D;
		}
		else
			faceTopTexture = faceTopObj.GetComponent<GUITexture>();
		
		if ( faceBottomObj.GetComponent<GUITexture>() == null )
		{
			faceBottomTexture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			faceBottomTexture2D.SetPixel(0, 0, Color.green);
			faceBottomTexture2D.Apply();
		
//			faceBottomObj = GameObject.Find("FaceBottomLine");
			faceBottomObj.transform.localScale = Vector3.zero;
			
			faceBottomLine = new Rect();
			faceBottomTexture = faceBottomObj.AddComponent<GUITexture>();
			faceBottomTexture.texture = faceBottomTexture2D;
		}
		else
			faceBottomTexture = faceBottomObj.GetComponent<GUITexture>();

		if ( faceLeftObj.GetComponent<GUITexture>() == null )
		{
			faceLeftTexture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			faceLeftTexture2D.SetPixel(0, 0, Color.green);
			faceLeftTexture2D.Apply();
			
//			faceLeftObj = GameObject.Find("FaceLeftLine");
			faceLeftObj.transform.localScale = Vector3.zero;
			
			faceLeftLine = new Rect();
			faceLeftTexture = faceLeftObj.AddComponent<GUITexture>();
			faceLeftTexture.texture = faceLeftTexture2D;
		}
		else
			faceLeftTexture = faceLeftObj.GetComponent<GUITexture>();
		
		if ( faceRightObj.GetComponent<GUITexture>() == null )
		{
			faceRightTexture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			faceRightTexture2D.SetPixel(0, 0, Color.green);
			faceRightTexture2D.Apply();
			
//			faceRightObj = GameObject.Find("FaceRightLine");
			faceRightObj.transform.localScale = Vector3.zero;
			
			faceRightLine = new Rect();
			faceRightTexture = faceRightObj.AddComponent<GUITexture>();
			faceRightTexture.texture = faceRightTexture2D;
		}
		else 
			faceRightTexture = faceRightObj.GetComponent<GUITexture>();
		
		faceTopTexture.enabled = true;
		faceBottomTexture.enabled = true;
		faceLeftTexture.enabled = true;
		faceRightTexture.enabled = true;
	}
	
	private void disableFaceHUD()
	{
		if ( faceTopTexture != null )
			faceTopTexture.enabled = false;
		if ( faceBottomTexture != null )
			faceBottomTexture.enabled = false;
		if ( faceLeftTexture != null )
			faceLeftTexture.enabled = false;
		if ( faceRightTexture != null )
			faceRightTexture.enabled = false;
	}
}
