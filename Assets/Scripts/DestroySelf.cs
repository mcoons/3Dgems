using UnityEngine;
using System.Collections;

// Wait for the timer to count up to the 
// expire time and then destroy myself

public class DestroySelf : MonoBehaviour 
{
	public float expireTime = 3;
	
	void Start () 
	{ 
		StartCoroutine("DestroyMyselfAfterSomeTime"); 
	}
	
	IEnumerator DestroyMyselfAfterSomeTime()
	{
		yield return new WaitForSeconds(expireTime);
		Destroy(gameObject);
	}
}
