using UnityEngine;
using System.Collections;

public class TextureAnimator : MonoBehaviour, IPauseHandle
{
	public float scrollSpeed = 0.25f;
	
	private bool isPaused;
	private Vector2 offsetVector;
	
	// Use this for initialization
	void Start () 
	{
		isPaused = false;
		offsetVector = new Vector2(0,0);
	}
	
	void FixedUpdate() 
	{
		if ( !isPaused )
		{
			// animates the 2D texture to make it actually look like a "laser wall"
		    float offset = Time.time * scrollSpeed;
			offsetVector.x = offset;
			offsetVector.y = offset;
			GetComponent<Renderer>().material.mainTextureOffset = offsetVector;
		}//end if
	}
	
	public void OnPauseGame()
	{
		isPaused = true;
	}
	
	public void OnResumeGame()
	{
		isPaused = false;
	}
}
