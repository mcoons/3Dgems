using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MenuScreen
{
	public string name;
	public string[] choices = new string[6]; // [6]
}

[System.Serializable]
public class MenuClass
{
	public MenuScreen[,] menu = new MenuScreen[4,4];
}

public class MenuManager : MonoBehaviour 
{
	public MenuClass mainMenu = new MenuClass();
	StatsManager statsManager;
	TowerManager towerManager;
	public GameObject theTower;
	public GameObject menuPlanesParent;

	public List<GameObject> menuPlanes;

	public GameObject touchPlanesParent;

	public MenuScreen currentMenu;

	public int currentMain = -1;
	public int currentSub = -1;
	public int currentSelection = -1;

	public bool camUp = false;

	// Use this for initialization
	void Start () 
	{
		statsManager = GameObject.Find("StatsManager").GetComponent<StatsManager>(); 
		towerManager = GameObject.Find ("TowerManager").GetComponent<TowerManager> ();
		mainMenu.menu = new MenuScreen[4,4];

		MenuScreen tmpM;

		// Main Menu
		tmpM = new MenuScreen();
		tmpM.name = "Main";
		tmpM.choices = new string[] { "", "Options", "Game Mode", "Stats", "", "Play            Exit" };
		mainMenu.menu[0,0] = tmpM;

		// Options
		tmpM = new MenuScreen();
		tmpM.name = "Options";
		tmpM.choices = new string[] { "", "Brightness", "Volume", "Speed", "", "Play           Back" };
		mainMenu.menu[1,0] = tmpM;
		
		tmpM = new MenuScreen();
		tmpM.name = "Brightness";
		tmpM.choices = new string[] { "", "Brightness Slider", "", "Slider Value", "", "Play           Back" };
		mainMenu.menu[1,1] = tmpM;
		
		tmpM = new MenuScreen();
		tmpM.name = "Volume";
		tmpM.choices = new string[] { "", "Volume Slider", "", "Slider Value", "", "Play           Back" };
		mainMenu.menu[1,2] = tmpM;
		
		tmpM = new MenuScreen();
		tmpM.name = "Speeds";
		tmpM.choices = new string[] { "", "Speed Slider", "", "Slider Value", "", "Play           Back" };
		mainMenu.menu[1,3] = tmpM;

		// Game Modes
		tmpM = new MenuScreen();
		tmpM.name = "Mode";
		tmpM.choices = new string[] { "", "Timed",     "Limited",   "Simon Says", 		"", "Play           Back" };
		mainMenu.menu[2,0] = tmpM;
		
		tmpM = new MenuScreen();
		tmpM.name = "Timed";
		tmpM.choices = new string[] { "", "2 Minutes", "5 Minutes", "10 Minutes", 		"", "Play           Back" };
		mainMenu.menu[2,1] = tmpM;
		
		tmpM = new MenuScreen();
		tmpM.name = "Limited";
		tmpM.choices = new string[] { "", "20 Shots",  "50 Shots",  "100 Shots", 		"", "Play           Back" };
		mainMenu.menu[2,2] = tmpM;
		
		tmpM = new MenuScreen();
		tmpM.name = "Simon Says";
		tmpM.choices = new string[] { "", "Easy",      "Medium",    "Hard", 			"", "Play           Back" };
		mainMenu.menu[2,3] = tmpM;

		// Stats
		tmpM = new MenuScreen();
		tmpM.name = "Stats";
		tmpM.choices = new string[] { "", "Current Game", "All Time", "Clear Stats", "", "Play           Back" };
		mainMenu.menu[3,0] = tmpM;
		
		tmpM = new MenuScreen();
		tmpM.name = "Current";
		tmpM.choices = new string[] { "Current Stat List", "", "", "", "", "Play           Back" };
		mainMenu.menu[3,1] = tmpM;
		
		tmpM = new MenuScreen();
		tmpM.name = "All Time";
		tmpM.choices = new string[] { "All Time List", "", "", "", "", "Play           Back" };
		mainMenu.menu[3,2] = tmpM;
		
		tmpM = new MenuScreen();
		tmpM.name = "Clear Stats?";
		tmpM.choices = new string[] { "", "Yes", "", "No", "", "Play           Back" };
		mainMenu.menu[3,3] = tmpM;
	}

	void OnGUI()
	{

		int sW = Screen.width;
		int sH = Screen.height;



		GUI.TextArea (new Rect (sW - 110, sH - 45,110, 20), "GemTypes = "+ GameObject.Find ("TowerManager").GetComponent<TowerManager> ().maxGemTypes.ToString());

		if (currentMain < 0)
		if (GUI.Button (new Rect (sW - 55, sH - 20, 55, 20), "Gems+")) {
			if (towerManager.maxGemTypes<=10) {GameObject.Find ("TowerManager").GetComponent<TowerManager> ().maxGemTypes++;}

		}

		if (currentMain < 0)
		if (GUI.Button (new Rect (sW - 110, sH - 20, 55, 20), "Gems-")) {
			if (towerManager.maxGemTypes >=2) {GameObject.Find ("TowerManager").GetComponent<TowerManager> ().maxGemTypes--;}

		}



		if (currentMain < 0)
		if (GUI.Button (new Rect (0, sH - 120, 110, 20), "Summon Wizard")) // White
		{

		}

		if (currentMain < 0)
		if (GUI.Button (new Rect (0, sH - 145, 110, 20), "Summon Pixie")) // Pink
		{

		}


		if (currentMain < 0)
		if (GUI.Button (new Rect (0, sH - 95, 110, 20), "Buy Ether")) // Black
		{
			towerManager.etherAmount = 100;
		}

/*
		if (currentMain < 0)
		if (GUI.Button (new Rect (0, sH - 120, 110, 20), "Add Plague")) // Green
		{

		}


		if (currentMain < 0)
		if (GUI.Button (new Rect (0, sH - 170, 110, 20), "Add Dragon")) // Red
		{

		}
*/

		if (currentMain < 0)
		if (GUI.Button (new Rect (sW - 110, sH - 70,110, 20), "Add Dragon")) // Red
		{

		}

		if (currentMain < 0)
		if (GUI.Button (new Rect (sW - 110, sH - 95,110, 20), "Add Plague")) // Red
		{

		}

		if (currentMain < 0)
		if (GUI.Button (new Rect (sW - 110, sH - 120,110, 20), towerManager.showCenter ? "Hide Center": "Show Center")) {
			GameObject.Find ("TowerManager").GetComponent<TowerManager> ().showCenter = !GameObject.Find ("TowerManager").GetComponent<TowerManager> ().showCenter;

		}


		if (currentMain < 0)
		if (GUI.Button (new Rect (sW - 110, sH - 145,110, 20), "Add Medusa")) // Red
		{

		}

		if (currentMain < 0)
		if (GUI.Button (new Rect (sW - 110, sH - 170,110, 20), "Burn Tower")) // Red
		{
			towerManager.BurnTower ();

		}


			if (currentMain < 0)
		if (GUI.Button (new Rect (sW - 55, 0, 55, 20), "Peek")) 
			{
			//-1.45
			//-20

			if (camUp) {
//				GameObject.Find ("Main Camera").transform.rotation = Quaternion.Euler (-1.45f, 0f, 0f);
				GameObject.Find ("Main Camera").transform.rotation = Quaternion.Euler (5.402f, 0f, 0f);
				camUp = false;
			}
			else {
				GameObject.Find ("Main Camera").transform.rotation = Quaternion.Euler (-20f, 0f, 0f);
				camUp = true;
			}

		}


		if (currentMain < 0)
		if  (GUI.Button(new Rect(0, 0, 55, 20), "Menu"))
		{
			theTower.SetActive(false);
			touchPlanesParent.SetActive(false);
			menuPlanesParent.SetActive(true);
			currentMain = 0;
			currentSub = 0;
			currentSelection = -1;
		}
	}
	// Update is called once per frame
	void Update () 
	{
		

		// Display the text for the menuTextManager
		if (currentMain >= 0)
		{
			currentMenu = mainMenu.menu[currentMain,currentSub];
			
			GameObject.Find("TextManager").GetComponent<TextManager>().AddMessage(0,currentMenu.name,-1, Color.cyan);

			for (int i = 0; i < 6; i++) 
			{
				if (i < 5) 
				{
					if (currentMenu.choices[i] == "") 
						menuPlanes[i].SetActive(false);
					else
						menuPlanes[i].SetActive(true);
				}
				GameObject.Find("TextManager").GetComponent<TextManager>().AddMessage(i*2+2,currentMenu.choices[i],-1, Color.cyan);
			}
		}
		if (currentSelection >= 0)
		{
			if (currentSelection == 5) // Play was selected
			{
				theTower.SetActive(true);

				touchPlanesParent.SetActive(true);
	
				menuPlanesParent.SetActive(false);
				currentMain = -1;
				currentSub = -1;
				currentSelection = -1;
				clearMenuText();
				GameObject.Find("TextManager").GetComponent<TextManager>().AddMessage(0,statsManager.currentStats.score.ToString(),-1, Color.cyan);
			}
			else
			if (currentSelection == 6)  // Back or Exit was selected
			{
				if (currentMain == 0)
				{	
					Application.Quit();		
				}
				else
				if (currentSub == 0)
				{	
					currentMain=0;			
				}
				else
				{	
					currentSub=0;			
				}
				
				currentSelection = -1;
			}
			else
			if (currentMain == 0)  // Main Menu Selection
			{
				currentMain = currentSelection;
				currentSub = 0;
				currentSelection = -1;
			}
			else
			if (currentSub == 0)  // Main Sub Menu Selection
			{
				currentSub = currentSelection;

				if (currentMain == 3 && currentSelection == 1)
				{
					mainMenu.menu[3,1].choices = new string[] { statsManager.GetCurrentStatsString(0), "", "", "", "", "Play           Back" };
				}

				currentSelection = -1;
			}
			else
			// Final Menu Selection
			{
				string action = currentMain.ToString()+currentSub.ToString()+currentSelection.ToString();
				switch (action) 
				{	// Timed 2 Minutes
					case "211":						

						currentSelection = -1;
					break;

					// Timed 5 Minutes
					case "212":						

						currentSelection = -1;
					break;

					// Timed 10 Minutes
					case "213":						

						currentSelection = -1;
					break;

					// Limited 20 Shots
					case "221":						

						currentSelection = -1;
					break;

					// Limited 50 Shots
					case "222":						

						currentSelection = -1;
					break;

					// Limited 100 Shots
					case "223":						

						currentSelection = -1;
					break;

					// Simon Says Easy
					case "231":						

						currentSelection = -1;
					break;

					// Simon Says Medium
					case "232":						

						currentSelection = -1;
					break;

					// Simon Says Hard
					case "233":						

						currentSelection = -1;
					break;
					

					default:
					break;
				}
			}
		}
	}

	public void clearMenuText()
	{
		for (int i = 0; i < 6; i++) 
		{
			GameObject.Find("TextManager").GetComponent<TextManager>().AddMessage(i*2+2,"",-1, Color.cyan);
		}	

	}
}
