using UnityEngine;
using System.Collections;

public class RandomScale : MonoBehaviour {
	
	public float minScale;
	public float maxScale;

	// Use this for initialization
	void Start () 
	{
		float scale = Random.Range(minScale, maxScale);
		
		if ( scale < 0.1f )
			scale = 0.1f;
		
		this.transform.localScale = new Vector3(scale, scale, scale);
	}
}
