using UnityEngine;
using System.Collections;

public class NewLevelDoorScript : MonoBehaviour, IPauseHandle {
	
	private bool isDoorOpening;
	private bool isPaused;
	
	private Transform topDoor;
	private Transform bottomDoor;
	private Transform leftDoor;
	private Transform rightDoor;
	
	private Vector3 topDoorOpenPos;
	private Vector3 bottomDoorOpenPos;
	private Vector3 leftDoorOpenPos;
	private Vector3 rightDoorOpenPos;
	
	void Start()
	{	
		isDoorOpening = false;
		isPaused = false;
		
		Transform child = this.transform.FindChild("FullDoor");
		
		topDoor = child.transform.Find("TopDoor");
		bottomDoor = child.transform.Find("BottomDoor");
		leftDoor = child.transform.Find("LeftDoor");
		rightDoor = child.transform.Find("RightDoor");
		
		topDoorOpenPos = new Vector3(topDoor.localPosition.x, 11, topDoor.localPosition.z);
		bottomDoorOpenPos = new Vector3(bottomDoor.localPosition.x, -11, bottomDoor.localPosition.z);
		leftDoorOpenPos = new Vector3(-11, leftDoor.localPosition.y, leftDoor.localPosition.z);
		rightDoorOpenPos = new Vector3(11, rightDoor.localPosition.y, rightDoor.localPosition.z);
	}
	
	void Update()
	{
		if ( isDoorOpening && !isPaused)
		{			
			topDoor.localPosition = Vector3.Lerp(topDoor.localPosition, topDoorOpenPos, Time.deltaTime * 5);
			bottomDoor.localPosition = Vector3.Lerp(bottomDoor.localPosition, bottomDoorOpenPos, Time.deltaTime * 5);
			leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, leftDoorOpenPos, Time.deltaTime * 5);
			rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, rightDoorOpenPos, Time.deltaTime * 5);
		}
	}
	
	// the only thing flying into these cubes are the player
	void OnTriggerEnter(Collider player)
	{
		if ( !isPaused && !isDoorOpening)
		{
			isDoorOpening = true;

		}//end if
	}//end OnTriggerEnter
	
	public void OnPauseGame()
	{
		isPaused = true;
	}
	
	public void OnResumeGame()
	{
		isPaused = false;
	}
}
