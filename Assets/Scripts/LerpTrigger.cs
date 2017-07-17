using UnityEngine;
using System.Collections;

public class LerpTrigger : MonoBehaviour, IPauseHandle {
	
	public const float DEFAULT_SPEED = 2;
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;
	public float speed;
	
	private float lerpX, lerpY;
	
	private bool isPaused = false;
	private bool startLerp = false;
	private Vector3 lerpPosition;
	
	void Start()
	{	
		startLerp = false;
		isPaused = false;
		
		if ( speed == 0 )
			speed = DEFAULT_SPEED;
		
		lerpX = Random.Range(minX, maxX);
		lerpY = Random.Range(minY, maxY);
		
		lerpPosition = new Vector3(lerpX, lerpY, transform.localPosition.z);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( !isPaused && startLerp)
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, lerpPosition, Time.deltaTime * speed);
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
