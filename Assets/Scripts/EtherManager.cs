using UnityEngine;
using System.Collections;

public class EtherManager : MonoBehaviour 
{
	TowerManager TMScript;

	float newValue;

	float oldMin;
	float oldMax;
	float newMin;
	float newMax;
	float oldRange;
	float newRange;

	// Use this for initialization
	private void Start() 
	{ 
		TMScript = GameObject.Find("TowerManager").GetComponent<TowerManager>(); 
	

		oldMin = 0;
		oldMax = 100f;
		newMin = -3.1f;
		newMax = .85f;
		oldRange = (oldMax - oldMin);
		newRange = (newMax - newMin);
	}

	// 0 - 100
	// -13.4 - 12.5


	// Update is called once per frame
	void Update () 
	{
		newValue = (((TMScript.etherAmount - oldMin) * newRange / oldRange) + newMin);
		gameObject.transform.position = new Vector3 (-3, newValue, 1);
	}
}
