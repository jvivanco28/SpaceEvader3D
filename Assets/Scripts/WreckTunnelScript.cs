using UnityEngine;
using System.Collections;

public class WreckTunnelScript : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		Transform wallAxis1 = transform.FindChild("WallAxis1");
		Transform wallAxis2 = transform.FindChild("WallAxis2");
		Transform wallAxis3 = transform.FindChild("WallAxis3");
		Transform wallAxis4 = transform.FindChild("WallAxis4");
		
		int angle = 45 + (180 * (Random.Range(0,2)));
		wallAxis1.localEulerAngles = new Vector3(0, 0, angle);
		
		angle = 90 + (180 * Random.Range(0,2));
		wallAxis2.localEulerAngles = new Vector3(0, 0, angle);
		
		angle = (180 * Random.Range(0,2));
		wallAxis3.localEulerAngles = new Vector3(0, 0, angle);
		
		angle = 135 + (180 * Random.Range(6,8));
		wallAxis4.localEulerAngles = new Vector3(0, 0, angle);
	}
}
