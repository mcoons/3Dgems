using UnityEngine;
using TouchScript.Gestures;
using System.Collections;
using System;

// This script is attached to the menu touch plane objects

public class MenuItem : MonoBehaviour 
{
	public int myNumber;
	MenuManager menuManager;

	// Use this for initialization
//	void Start () 
//	{
//		menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>(); 
//	}

	private void OnEnable() 
	{ 
		GetComponent<TapGesture>().Tapped += tappedHandler; 
	}

	private void OnDisable()
	{ 
		GetComponent<TapGesture>().Tapped -= tappedHandler; 
	}
	
	private void tappedHandler(object sender, EventArgs eventArgs)
	{
		GameObject.Find("MenuManager").GetComponent<MenuManager>().currentSelection = myNumber;
	}

	// Update is called once per frame
//	void Update () 
//	{
//	
//	}
}
