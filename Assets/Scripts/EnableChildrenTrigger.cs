using UnityEngine;
using System.Collections;

public class EnableChildrenTrigger : MonoBehaviour {
	
	private bool isPaused = false;
	private bool childrenEnabled = false;
	
	void Start()
	{	
		isPaused = false;
		childrenEnabled = false;
		
		// disable all children
		for ( int i = 0; i < transform.childCount; i++ )
		{
			Transform child = transform.GetChild(i);
			child.gameObject.SetActive(false);
		}
	}
	
	// the only thing flying into these cubes are the player
	void OnTriggerEnter(Collider player)
	{
		if ( !isPaused && !childrenEnabled)
		{
			childrenEnabled = true;
			
			for ( int i = 0; i < transform.childCount; i++ )
			{
				Transform child = transform.GetChild(i);
				child.gameObject.SetActive(true);
			}
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
