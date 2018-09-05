using UnityEngine;
using System.Collections;
using System;


public class StatsManager : MonoBehaviour 
{
	[System.Serializable]
	public struct StatsStruct
	{
		public string name;

		public float timePlayed;
		public float timePaused;

		public int score;
		public int shots;
		public int objectsDestroyed;
		public int rowsDestroyed;
		public int levelsDestroyed;
		public int strikes;
		public int solidStrikes;
//		public int 
//		public int 
//		public int 
//		public int 
//		public int 
//

//
//		public float shotsPreMinute = 0;
//		public float pointsPerMinute = 0;
//		public float pointsPerShot = 0;
//		public float 
//		public float 
//		public float 

	}
//	GameManager GMScript;

	public StatsStruct currentStats;
	public StatsStruct allTimeStats;
	public string playerName = "Your Name";
	public float startTime;

	void ClearCurrentStats()
	{
		name = playerName;
		
		currentStats.timePlayed = 0;
		currentStats.timePaused = 0;
		
		currentStats.score = 0;
		currentStats.shots = 0;
		currentStats.objectsDestroyed = 0;
		currentStats.rowsDestroyed = 0;
		currentStats.levelsDestroyed = 0;
		currentStats.strikes = 0;
		currentStats.solidStrikes = 0;
	}
	
	void ClearAllTimeStats()
	{
		name = playerName;
		
		allTimeStats.timePlayed = 0;
		allTimeStats.timePaused = 0;
		
		allTimeStats.score = 0;
		allTimeStats.shots = 0;
		allTimeStats.objectsDestroyed = 0;
		allTimeStats.rowsDestroyed = 0;
		allTimeStats.levelsDestroyed = 0;
		allTimeStats.strikes = 0;
		allTimeStats.solidStrikes = 0;
	}

	public string GetCurrentStatsString(int type)
	{
		string statsString = "";

		if (type == 0)
		{
			//statsString += GMScript.playerName + "\n\n"; //currentStats.name + "\n\n";
			string timeString = TimeSpan.FromSeconds( Time.time - startTime ).ToString();

			statsString += timeString.Substring(0, timeString.LastIndexOf("."))  + " Played\n";
			//statsString += TimeSpan.FromSeconds( currentStats.timePlayed ).ToString() + " Played\n";
			statsString += TimeSpan.FromSeconds( currentStats.timePaused ).ToString() + " Paused\n\n";

			statsString += currentStats.score.ToString() + " Points\n\n";
			statsString += currentStats.shots.ToString() + " Shots \n";
			statsString += currentStats.objectsDestroyed.ToString() + " Objects\n";
			statsString += currentStats.rowsDestroyed.ToString() + " Rows\n";
			statsString += currentStats.levelsDestroyed.ToString() + " Levels\n";
			statsString += currentStats.strikes.ToString() + " Strikes\n";
			statsString += currentStats.solidStrikes.ToString() + " Solids\n";

			// statsString += ":" + currentStats..ToString() + "\n";

		}
		else
		{
			statsString = "";



		}

		return statsString;

	}

	void DisplayStatsToScreen()
	{

	}

	// Use this for initialization
	void Start () 
	{
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
