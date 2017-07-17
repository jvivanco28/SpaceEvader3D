using UnityEngine;
using System.Collections;

public class RandomAsteroidPicker : MonoBehaviour {
	
	public const int 	NUMBER_OF_ASTEROIDS_TO_PICK_FROM = 4;
	public const string	ASTEROID1_PATH = "Obstacles/Asteroid1";
	public const string	ASTEROID2_PATH = "Obstacles/Asteroid2";
//	public const string	ASTEROID3_PATH = "Obstacles/Asteroid3";
	public const string	ASTEROID4_PATH = "Obstacles/Asteroid4";
	public const string	ASTEROID5_PATH = "Obstacles/Asteroid5";

	public bool hasTwoAsteroids;
	
	// Use this for initialization
	void Start () {
		
		GameObject newAsteroidObject = createRandomAsteroid();
		
		if ( newAsteroidObject != null )
		{
			newAsteroidObject.transform.parent = this.transform;
			newAsteroidObject.transform.localPosition = new Vector3(newAsteroidObject.transform.localPosition.x,
																	newAsteroidObject.transform.localPosition.y,
																	3);
		}//end if
		
		if ( hasTwoAsteroids )
		{
			newAsteroidObject = createRandomAsteroid();
			if ( newAsteroidObject != null ) 
			{
				newAsteroidObject.transform.parent = this.transform;
				newAsteroidObject.transform.localPosition = new Vector3(newAsteroidObject.transform.localPosition.x,
																		newAsteroidObject.transform.localPosition.y,
																		-3);
			}//end if
		}//end if
	}
	
	
	private GameObject createRandomAsteroid()
	{
		GameObject newAsteroidObject = null;
		
		int random = Random.Range(0,1000) % NUMBER_OF_ASTEROIDS_TO_PICK_FROM;
		
		if ( random % NUMBER_OF_ASTEROIDS_TO_PICK_FROM == 0 )
			newAsteroidObject = (GameObject)Instantiate(Resources.Load(ASTEROID1_PATH), this.transform.position, this.transform.rotation);
		else if ( random % NUMBER_OF_ASTEROIDS_TO_PICK_FROM == 1 )
			newAsteroidObject = (GameObject)Instantiate(Resources.Load(ASTEROID2_PATH));
//		else if ( random % NUMBER_OF_ASTEROIDS_TO_PICK_FROM == 2 )
//			newAsteroidObject = (GameObject)Instantiate(Resources.Load(ASTEROID3_PATH));
		else if ( random % NUMBER_OF_ASTEROIDS_TO_PICK_FROM == 2 )
			newAsteroidObject = (GameObject)Instantiate(Resources.Load(ASTEROID4_PATH));
		else if ( random % NUMBER_OF_ASTEROIDS_TO_PICK_FROM == 3 )
			newAsteroidObject = (GameObject)Instantiate(Resources.Load(ASTEROID5_PATH));		

		return newAsteroidObject;
	}
}
