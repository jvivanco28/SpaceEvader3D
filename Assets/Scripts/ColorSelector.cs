using UnityEngine;
using System.Collections;

public class ColorSelector : MonoBehaviour {
	
	public float red;
	public float green;
	public float blue;
	public float alpha;
	
	// Use this for initialization
	void Start () {
	
//		if ( red <= 0 && green <= 0 && blue <= 0 )
//		{
//			red = Random.Range(0.0f, 1.0f);
//			green = Random.Range(0.0f, 1.0f);
//			blue = Random.Range(0.0f, 1.0f);
//		}
		gameObject.GetComponent<Renderer>().material.color = new Color(red, green, blue, alpha);
	}
}
