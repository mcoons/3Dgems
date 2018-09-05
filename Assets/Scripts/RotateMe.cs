using UnityEngine;
using System.Collections;

/// <summary>
///  When this script is attached to an object it rotates it each frame.
/// </summary>

public class RotateMe : MonoBehaviour
{
	public Vector3 rotation = Vector3.zero; 
	 
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate(rotation * Time.deltaTime);
	}
}

 