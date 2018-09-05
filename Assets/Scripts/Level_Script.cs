﻿using UnityEngine;
using TouchScript.Gestures;
using System.Collections;
using System;

// This script is attached to the level touch plane objects.
// A flick gesture will rotate the whole level left or right

public class Level_Script : MonoBehaviour 
{
	TowerManager TMScript;

	public int myLevel;
	public Vector2 flickVec;

	private void Start() 
	{	
		TMScript = GameObject.Find("TowerManager").GetComponent<TowerManager>(); 
	}

	private void OnEnable()	
	{ 
		GetComponent<FlickGesture>().Flicked += flickHandler;
		GetComponent<TapGesture>().Tapped += tappedHandler; 
	}
	private void OnDisable()
	{ 
		GetComponent<TapGesture>().Tapped -= tappedHandler; 
		GetComponent<FlickGesture>().Flicked -= flickHandler;	
	}

	private void tappedHandler(object sender, EventArgs eventArgs)
	{
		TMScript.ClearMarkers();
	}

	private void flickHandler(object sender, EventArgs eventArgs)
	{
		var flick = sender as FlickGesture;
		
		flickVec = flick.ScreenFlickVector;
		
		if (flickVec.x < 0) 
		{  // Rotate the objects in that level clockwise/right
			TMScript.RotateLevelRight(myLevel);
		}
		else
		{ // Rotate the objects in that level counterclockwise/left
			TMScript.RotateLevelLeft(myLevel);
		}
	}
}
