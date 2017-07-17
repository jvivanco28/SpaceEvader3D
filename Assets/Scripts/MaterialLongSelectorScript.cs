using UnityEngine;
using System.Collections;

public class MaterialLongSelectorScript : MonoBehaviour {
	
	public static string PIPE_MAT = "Materials/PipeMat";
	public static string HAZARD_MAT = "Materials/HazardLongMat";
	
	public enum PipeMaterials
	{
//		Pipe,
		Hazard
	}
	
	void Start () 
	{
//		int materialId = Random.Range(0,10000) % System.Enum.GetValues(typeof(PipeMaterials)).Length;
		
		Material newMat;
//		if ( materialId == (int)PipeMaterials.Pipe )
//			newMat = Resources.Load(PIPE_MAT, typeof(Material)) as Material;
//		else 
			newMat = Resources.Load(HAZARD_MAT, typeof(Material)) as Material;
		
		int numChildren = this.transform.childCount;
		if ( numChildren > 1 )
		{
			// going with the assumption that the children don't have children
			for ( int i = 0; i < numChildren; i++ ) 
			{
				this.transform.GetChild(i).GetComponent<Renderer>().material = newMat;
			}
		}
		else
			this.GetComponent<Renderer>().material = newMat;
	}
}
