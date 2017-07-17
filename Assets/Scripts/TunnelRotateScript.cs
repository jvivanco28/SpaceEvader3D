using UnityEngine;
using System.Collections;

public class TunnelRotateScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// determine angle of tunnel: either 0, 90, 180, 270 degrees rotated
		int randomNum = Random.Range(0,10000) % 4;
		if ( randomNum == 1 )
			transform.Rotate(0,0,90f);
		else if ( randomNum == 2 )
			transform.Rotate(0,0,180);
		else if ( randomNum == 3 )
			transform.Rotate(0,0,270);
	}//end if
}
