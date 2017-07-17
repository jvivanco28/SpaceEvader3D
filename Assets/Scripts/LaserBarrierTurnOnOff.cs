using UnityEngine;
using System.Collections;

public class LaserBarrierTurnOnOff : MonoBehaviour {
	
//	private const float ON_TIME = 1f;
	private float switchOnOffTime;
	private Transform fence;
	private float time;
	
	// Use this for initialization
	void Start () {
		fence = this.transform.FindChild("Fence");
		time = Time.time;
		switchOnOffTime = Random.Range(0.5f, 1.5f);
	}	
	
	// Update is called once per frame
	void Update () {
		if ( fence != null )
		{
			if ( Time.time > time + switchOnOffTime )
			{
				if ( fence.GetComponent<Renderer>().enabled ) 
				{
					fence.GetComponent<Renderer>().enabled = false;
					fence.GetComponent<Collider>().enabled = false;
				}
				else
				{
					fence.GetComponent<Renderer>().enabled = true;
					fence.GetComponent<Collider>().enabled = true;
				}
				time = Time.time;
			}//end if
		}//end if
	}
}
