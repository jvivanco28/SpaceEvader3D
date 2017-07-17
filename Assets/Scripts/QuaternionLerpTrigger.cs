using UnityEngine;
using System.Collections;

public class QuaternionLerpTrigger : MonoBehaviour, IPauseHandle {

	public const float DEFAULT_SPEED = 2;
	public float lerpX;
	public float lerpY;
	public float lerpZ;
	public float speed;
		
	private bool isPaused = false;
	private bool startLerp = false;
	private Quaternion lerpRotation;
	
	void Start()
	{	
		startLerp = false;
		isPaused = false;
		
		if ( speed == 0 )
			speed = DEFAULT_SPEED;
		
		lerpRotation = Quaternion.Euler(new Vector3(lerpX, lerpY, lerpZ));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( !isPaused && startLerp)
		{
//			transform.localPosition = Vector3.Lerp(transform.localPosition, lerpPosition, Time.deltaTime * speed);
			transform.localRotation = Quaternion.Lerp(transform.localRotation, lerpRotation, Time.deltaTime * speed);
		}//end if
	}//end Update
	
	// the only thing flying into these cubes are the player
	void OnTriggerEnter(Collider player)
	{
		if ( !isPaused )
		{
			startLerp = true;
		}//end if
	}//end OnTriggerEnter
	
	public void OnPauseGame()
	{
		isPaused = true;
	}
	
	public void OnResumeGame()
	{
		isPaused = false;
	}
}
