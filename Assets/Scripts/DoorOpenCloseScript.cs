using UnityEngine;
using System.Collections;

public class DoorOpenCloseScript : MonoBehaviour, IPauseHandle
{
	public const float DEFAULT_DOOR_SPAN = 3f;
	
	public const float DEFAULT_DOOR_OFFSET = 4f;
	
	public float doorSpan;
	public float doorOffset;
	
	private float speed;
	private bool isPaused;

	// Use this for initialization
	void Start () 
	{
		isPaused = false;

		// how fast the door will be moving
		speed = Random.Range(1,3);
		
		if ( doorSpan <= 0.01f )
		{
			doorSpan = DEFAULT_DOOR_SPAN;
		}
		
		if ( doorOffset <= 0.01f )
		{
			doorOffset = DEFAULT_DOOR_OFFSET;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( !isPaused )
		{
			float offset = (((1 + Mathf.Sin(Time.time * speed)) * doorSpan ) + doorOffset);
			transform.localPosition = new Vector3(offset, transform.localPosition.y, transform.localPosition.z);			
		}//end if
	}//end Update
	
	public void OnPauseGame()
	{
		isPaused = true;
	}
	
	public void OnResumeGame()
	{
		isPaused = false;
	}
}
