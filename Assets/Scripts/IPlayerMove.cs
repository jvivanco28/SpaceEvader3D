using UnityEngine;
using System.Collections;

public abstract class IPlayerMove : MonoBehaviour {
	
	// TODO: don't make this so ugly!!
	// all of these static vars are kind of a hack!! May need to figure out a different way
	protected static PlayerUpVector currUpVector = PlayerUpVector.worldUp;
	protected static float goalAngle = 0f;
	protected static bool isPaused = false; 
	protected static float xScaled = 0f;
	protected static float yScaled = 0f;	
	
	// TODO: don't make this so ugly!!
	// HACK!! This is super ugly code
	public static void init()
	{
		currUpVector = PlayerUpVector.worldUp;
		goalAngle = 0f;
		xScaled = 0f;
		yScaled = 0f;	
	}
	
	public float maxX = 5f;
	public float minX = -5f;
	public float maxY = 5f;
	public float minY = -5f;
	public float give = 1.25f;
	
	// gives us some bit of rotation when we translate
	public Transform cameraLookAt;

	public enum PlayerUpVector
	{
		worldUp,
		worldLeft,
		worldDown,
		worldRight
	}
	
	public enum Controller 
	{
		Delta = 0,
		Absolute = 1,
		EyeTracker = 2,
		Dead = 3,
		Start = 4,
		StartScreen = 5,
		Finished = 6
	}
	
	public enum DeltaControllerType
	{
		Normal = 0,
		Inverted = 1
	}
	
	public abstract Controller getControllerType();
		
	public virtual void rotateLeft()
	{
		goalAngle += 90 % 360;
		currUpVector = (PlayerUpVector)(((int)currUpVector + 1) % 4);
	}
	
	public virtual void rotateRight()
	{
		goalAngle += -90;
		currUpVector = (PlayerUpVector)(((int)currUpVector + 3 ) % 4); // same as -1, but we can't use negatives here
	}
	
	// for handling pause messages
	public void OnPauseGame ()
	{
		isPaused = true;
	}
	
	public void OnResumeGame ()
	{
		isPaused = false;
	}
}
