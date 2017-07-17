using UnityEngine;
using System.Collections;

public class CrashFragmentScript : MonoBehaviour, IPauseHandle {
	
	public Vector3 relativeTo;
	public float speed;
	
	private bool isPaused = false;
	
	// Update is called once per frame
	void Update () 
	{	
		if ( !isPaused )
		{
//			Debug.Log("" + Time.deltaTime);
//			frag1pos.z += Time.deltaTime;
			
			this.transform.Translate(relativeTo.x, relativeTo.y, Time.deltaTime * speed);
//			frag1.transform.Rotate(Random.Range(0f,1.0f), Random.Range(0f,1.0f), Random.Range(0f,1.0f));
			this.transform.Rotate(1f, 1f, 1f);

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
