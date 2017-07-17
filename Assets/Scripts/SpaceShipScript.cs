using UnityEngine;
using System.Collections;

public class SpaceShipScript : MonoBehaviour {

	public GameObject engineLight;
	public GameObject flame;
	
	public void Start()
	{
		setOnFire(false);
	}
	
	// plays a particle system of exploding white particles
	public void explode()
	{
		setOnFire(false);

		ParticleSystem explosionParticleSystem = this.transform.parent.GetComponent<ParticleSystem>();
		if ( explosionParticleSystem != null && !explosionParticleSystem.isPlaying ) 
			explosionParticleSystem.Play();
		this.gameObject.SetActive(false);
	}
	
	// plays a particle system showing a flame on the space ship
	public void setOnFire(bool onFire)
	{
		if ( flame != null ) 
		{
			Transform innerCore = flame.transform.FindChild("InnerCore");
			Transform outerCore = flame.transform.FindChild("OuterCore");
			Transform lightSource = flame.transform.FindChild("Lightsource");
			
			if ( !onFire ) 
			{
				if ( engineLight != null )
					engineLight.SetActive(true);

				innerCore.GetComponent<ParticleEmitter>().emit = false;
				outerCore.GetComponent<ParticleEmitter>().emit = false;
				lightSource.GetComponent<Light>().enabled = false;
			}
			else
			{
				if ( engineLight != null )
					engineLight.SetActive(false);

				innerCore.GetComponent<ParticleEmitter>().emit = true;
				outerCore.GetComponent<ParticleEmitter>().emit = true;
				lightSource.GetComponent<Light>().enabled = true;
			}
		}
	}
	
}
