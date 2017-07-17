using UnityEngine;
using System.Collections;

public class ZRotationScript : MonoBehaviour, IPauseHandle {
	
	public float rotationSpeed;
	
	private bool isPaused = false;
	
	void Start()
	{
		// if not set, then set to some random number
		if ( rotationSpeed <= 0.01 )
			rotationSpeed = Random.Range(-1f, 1f);
	}

	
	// Update is called once per frame
	void Update () {
		if ( !isPaused )
		{
			// simple rotation around the z axis
			transform.Rotate(0f, 0f, rotationSpeed);
		}
	}//end update
	
	public void OnPauseGame()
	{
		isPaused = true;
	}
	
	public void OnResumeGame()
	{
		isPaused = false;
	}
}
