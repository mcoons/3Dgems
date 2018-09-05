using UnityEngine;
using System.Collections;

public class Slider : MonoBehaviour {

	public Transform knob;
	private Vector3 targetPos;


	void OnTouchStay(Vector3 point)
	{
		targetPos = new Vector3(point.x, targetPos.y,targetPos.z);
	}

	// Use this for initialization
	void Start () {
		targetPos = knob.position;
	}
	
	// Update is called once per frame
	void Update () {
		knob.position = Vector3.Lerp(knob.position, targetPos, Time.deltaTime * 7);
	}
}
