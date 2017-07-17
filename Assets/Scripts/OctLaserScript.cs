using UnityEngine;
using System.Collections;

public class OctLaserScript : MonoBehaviour {
	
	private int angle;
	private float laserStartTime;
	private float switchLaserTime;
	
	// Use this for initialization
	void Start () {
	
		int random = Random.Range(0,8);
		angle = 45 * random;
		transform.localEulerAngles = new Vector3(0, 0, angle);
		laserStartTime = Time.time;
		switchLaserTime = Random.Range(0.5f, 1.5f);
	}
	
	// Wayyy too hard if we include this
	void LateUpdate () {
		
		if ( Time.time > laserStartTime + switchLaserTime ) 
		{
			angle = (angle + 45) % 360;
			transform.localEulerAngles = new Vector3(0, 0, angle);
			laserStartTime = Time.time;
		}
	}
}
