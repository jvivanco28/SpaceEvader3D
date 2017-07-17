using UnityEngine;
using System.Collections;

public class OctTunnelRotateScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		int randomNum = Random.Range(0,8);
		transform.localEulerAngles = new Vector3(0, 0, randomNum * 45);
	}

}
