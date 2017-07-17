using UnityEngine;
using System.Collections;

public class WreckLaserScript : MonoBehaviour, IPauseHandle {
	
	private const int START_ANGLE = 180;
	private const int ANGLE_SPAN = 90;

	private Vector3 angle;
	private float speed;
	private bool isPaused;
	
	// Use this for initialization
	void Start () {
		
		isPaused = false;
		speed = Random.Range(-2f, 2f);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		if ( !isPaused )
		{
			// gives us a number between 0 and 90
			angle.z =  ((1 + Mathf.Sin(Time.time * speed)) / 2) * ANGLE_SPAN;
			
			// we want an angle between 180 and 270
			angle.z = START_ANGLE + angle.z;
			
			transform.localEulerAngles = angle;
		}
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
