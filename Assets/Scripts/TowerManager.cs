using UnityEngine;
using TouchScript.Gestures;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class objClass
{
	public GameObject myObject;

	public string myType = "NOT SET";
	public float lastChangeTime;
	public float nextActivation;

	public bool alreadyChecked = false;
	public bool explodeMe = false;

	public bool dropping = false;
	public float dropStartTime;
	public Vector3 startLocation;
	public Vector3 desiredLocation;

	public bool infected = false;
	public float exposed = 0;
}
 
[System.Serializable]
public class columnClass
{
	public string columnName = "ColumnClass";
	public List<objClass> objectList = new List<objClass>();
}

[System.Serializable]
public class towerClass
{
	public string towerName = "TowerClass";
	public List<columnClass> columnListCopy = new List<columnClass>();

	public columnClass[,] columnList;
}

public enum SoundQueueIndex { Rotate, BadSelection, Poof }

public class TowerManager : MonoBehaviour {

	#region Object pointers
	
	public GameObject towerObject;								// Parent tower for levels for group rotation
	public GameObject[] towerLevelObjects = new GameObject[12];			// Levels to rotate and to parent objects to
	public GameObject centerColumn;
	
	public GameObject greenMarker;
	public GameObject redMarker;
	
	public GameObject markerX;
	public GameObject markerY;
	public GameObject markerZ;
	public GameObject markerLayer;
	
	public GameObject poof;
	public GameObject greenBolt;
	public GameObject deathBolt;
	public GameObject infection;
	
	public GameObject[] prefabs;		// Objects to instantiate
	public GameObject blankGem;
	public int gemStartOffset = 7;
	int totalBonus;
	public GameObject[] specials; 
	public enum specialsIndex	{Coal, Wildcard, Medusa, Wizard, Present, Pixie, Plague, Dragon }
	#endregion


	public GrabBag rndBag;
	//public string rndBagString;

	#region Tower variables
	
	public  int matrixX = 4;
	public  int matrixY = 12;
	public  int matrixZ = 4;		
	
	//	public objClass[,,] array3D = new objClass[3,6,3];	
	int i, j, k;		//  Array Iterators
	
	objClass tmpTowerObject;
	columnClass tmpTowerColumn;
	public towerClass theTower;

	public GameObject L5CHider;
	public GameObject L4CHider;
	public GameObject L3CHider;
	public GameObject L2CHider;
	public GameObject L1CHider;
	public GameObject L0CHider;

	public GameObject D5CHider;
	public GameObject D4CHider;
	public GameObject D3CHider;
	public GameObject D2CHider;
	public GameObject D1CHider;
	public GameObject D0CHider;

	public int maxGemTypes;
	public bool showCenter = true;
	//public int[,] coordToColumn = new int[3,3];
	//  |2|5|8|
	//Z |1|4|7|
	//  |0|3|6|
	//     X
	public float[] levelYPos = new float[12];

	#endregion

	#region Level Rotation Variables
	
	bool[] levelInRotation = new bool[12];
	float[] levelRotationStartTime = new float[12];
	Quaternion[] levelStartRotation = new Quaternion[12];
	Quaternion[] levelDesiredRotation = new Quaternion[12];
	
	#endregion					
	
	#region Tower Rotation Variables
	
	bool towerInRotation = false;
	float towerRotationStartTime = -1;
	Quaternion towerStartRotation;
	Quaternion towerDesiredRotation;
	
	#endregion
	
	#region Object Drop Variables
	
	public int dropCount = 0;				// Current count of objects dropping
	bool groupExplosion = false;
	public bool objectsDestroyed;
	
	#endregion

	TextManager textManagerScript;
	StatsManager statsManagerScript;

	//public List<GameObject> neighborList = new List<GameObject>();
	public List<objClass> neighborList = new List<objClass>();
	public List<GameObject> markerList = new List<GameObject>();
	public int[,] columnNeighborsFound;
	public int[] layerNeighborsFound;
	int[,] xRows;
	int[,] zRows;

	public float explosionTimer;

	public AudioSource[] AudioFile;	

	public float etherAmount = 0; // 0-100

	int currentType;


	#region *** Property Access Methods ***

	public bool     TowerInRotation() 	{ return towerInRotation;	}
	public bool 	LevelsInRotation()	{ return (levelInRotation[0] | levelInRotation[1] | levelInRotation[2] | levelInRotation[3] | levelInRotation[4] | levelInRotation[5] ); }
	public Vector3  towerSize() 	  	{ return new Vector3(matrixX,matrixY,matrixZ); }
	public bool 	HasSelection()		{ return !(markerList.Count == 0); }
	public bool		CenterShown()		{ return showCenter; }
	public bool 	IsDropping()		{ return (dropCount != 0); }

	#endregion


	// Use this for initialization
	void Start () 
	{
		AudioFile = transform.GetComponents<AudioSource>();
		textManagerScript = GameObject.Find ("TextManager").GetComponent<TextManager> ();
		statsManagerScript = GameObject.Find ("StatsManager").GetComponent<StatsManager> ();

		maxGemTypes = UnityEngine.Random.Range(3,3);
		gemStartOffset = UnityEngine.Random.Range(0,1) * 7;
		//towerLevelObjects = new GameObject[matrixY];
		rndBag = new GrabBag();
		//rndBagString = "";

//		int columnNumber = -1;
//		for (i = 0; i < matrixX; i++)
//			for (k = 0; k < matrixZ; k++)
//			{
//				columnNumber++;
//				i,k = columnNumber;
//			}
		theTower.columnListCopy = new List<columnClass>();


		levelYPos [0] = 0;
		levelYPos [1] = 1;
		levelYPos [2] = 2;
		levelYPos [3] = 3;
		levelYPos [4] = 4;
		levelYPos [5] = 5;
		levelYPos [6] = 7.5f;
		levelYPos [7] = 8.5f;
		levelYPos [8] = 9.5f;
		levelYPos [9] = 10.5f;
		levelYPos [10] = 11.5f;
		levelYPos [11] = 12.5f;

		rndBag.setMin (0);
		rndBag.setMax(500);
		rndBag.setDups (1);
		rndBag.addRange ();

		BuildRandomTower();


//		rndBag.fillBag (0,484,1);
//		BuildRandomTower();
//		rndBag.fillBag (0,484,1);
//		rndBag.setMax (490);

	}

//	theTower.columnList = new columnClass[matrixX,matrixZ];

	public void UpdateArray()
	{
		theTower.columnListCopy.Clear();

		for (int x = 0; x < matrixX; x++) 
			for (int z = 0; z < matrixZ; z++) 
				theTower.columnListCopy.Add(theTower.columnList[x,z]);
	}



	// Update is called once per frame
	void Update () 
	{

		if (CenterShown ()) 
		{
			centerColumn.SetActive (true);

			L5CHider.SetActive (false);
			L4CHider.SetActive (false);
			L3CHider.SetActive (false);
			L2CHider.SetActive (false);
			L1CHider.SetActive (false);
			L0CHider.SetActive (false);

			D5CHider.SetActive (false);
			D4CHider.SetActive (false);
			D3CHider.SetActive (false);
			D2CHider.SetActive (false);
			D1CHider.SetActive (false);
			D0CHider.SetActive (false);
		} 
		else 
		{
			centerColumn.SetActive (false);

			L5CHider.SetActive (true);
			L4CHider.SetActive (true);
			L3CHider.SetActive (true);
			L2CHider.SetActive (true);
			L1CHider.SetActive (true);
			L0CHider.SetActive (true);

			D5CHider.SetActive (true);
			D4CHider.SetActive (true);
			D3CHider.SetActive (true);
			D2CHider.SetActive (true);
			D1CHider.SetActive (true);
			D0CHider.SetActive (true);
		}


		if (groupExplosion && !objectsDestroyed)
		{
			DestroyNeighborObjects();		// uses neighborlist

			neighborList.Clear();

			DropObjectsInMatrix();
		}
		else
		if (groupExplosion && Time.time - explosionTimer > 1) 
		{
			textManagerScript.AddMessage((int)TextQueueIndex.Score,statsManagerScript.currentStats.score.ToString(),-1, Color.cyan);

			statsManagerScript.currentStats.shots+=1;
			statsManagerScript.currentStats.objectsDestroyed += neighborList.Count;
			
			groupExplosion = false;
			ResetNames();
		}
		else
		if (!groupExplosion)
		{
			// Lerp objects
			for (j = 0; j < matrixY; j++) 
				if (levelInRotation[j]) 
					levelObjectLerpRotation(j);
			
			if (towerInRotation) 
				towerObjectLerpRotation();
			
			for (i = 0; i < matrixX; i++)
				for (k = 0; k < matrixZ; k++)
					for (j = 0; j < matrixY; j++)		
						if (theTower.columnList[i,k].objectList[j].dropping)  
							towerObjectLerpDrop(i,j,k);
			
			MirrorCenterColumn ();
			UpdateArray();

		}


		//etherAmount -= .001f;
		if (etherAmount < 0)
			etherAmount = 0;

		//rndBagString = rndBag.showBag ();
	}

	public void BurnTower()
	{
		ClearMarkers ();

		for (i = 0; i < matrixX; i++)
			for (k = 0; k < matrixZ; k++)
				for (j = 0; j < 6; j++) 
				{
					theTower.columnList[i,k].objectList[j].explodeMe = true;
//					neighborList.Add(theTower.columnList[i,k].objectList[j].myObject);
					neighborList.Add(theTower.columnList[i,k].objectList[j]);
				}

		groupExplosion = true;
		explosionTimer = Time.time;
		objectsDestroyed = false;
	}

	public void TapObject(Vector3 position)
	{

		if (dropCount > 0 || towerInRotation ) return;

		if (LevelsInRotation()) return;

		position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
		//int myColumn = coordToColumn[(int)Mathf.Round(position.x),(int)Mathf.Round(position.z)];

		objClass obj = theTower.columnList [(int)position.x, (int)position.z].objectList [(int)position.y];

		
		if (position.z < .5f)	// ?????????  Is this the correct value ??????????
		{
			// Clear any previously selected/marked neighbors 
			neighborList.Clear();	// In tap object

			foreach (GameObject marker in markerList) 
			{
				Destroy(marker);
			}
			markerList.Clear();		// In tap object

			// Recursively find all touching neighbors
			//MarkMyNeighbors( (int)position.x, (int)position.y, (int)position.z, obj.myType ); 
			MarkMyNeighbors( obj, obj.myType ); 


			// If a Wildcard or Coal or only one neighbor is found, itself. change it from blue to red
			if (neighborList.Count == 1      || 
				obj.myType.Equals("Coal")    || 
				obj.myType.Equals("Wildcard")||

				obj.myType.Equals("Medusa")  || 
				obj.myType.Equals("Wizard")  ||

				obj.myType.Equals("Pixie")   || 
				obj.myType.Equals("Plague")  ||
				obj.infected
			
				)
			{
				textManagerScript.AddMessage((int)TextQueueIndex.Bottom,"NO MATCH",-1, Color.red);

				Vector3 tp = markerList[0].transform.position;
				foreach (GameObject marker in markerList) 
				{
					Destroy(marker);
				}
				markerList.Clear();		// In tap object

				// Give it a red marker since there are no neighbors
				GameObject tmpObject = (GameObject) Instantiate(redMarker, tp, Quaternion.identity);
				tmpObject.transform.parent = towerObject.transform;
				markerList.Add(tmpObject);
				AudioFile[(int)SoundQueueIndex.BadSelection].Play();
			}
			else
			{
				textManagerScript.AddMessage((int)TextQueueIndex.Bottom,neighborList.Count.ToString() + " selected",-1, Color.cyan);

				// Scan neighbor list for rows, columns and layers
				ScanForBonus();
			}
		}
	}

	public void PressObject(Vector3 position)
	{
		position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));

		if (neighborList.Count == 1)  // Should this be zero or one ???????
			return;

		//		//
		//		// Something needs to be selected here to go on
		//		//

//		foreach (objClass tmpObject in neighborList) 
//		{
//			tmpObject.explodeMe = true;
//		}

		neighborList.ForEach(n => n.explodeMe = true);

		groupExplosion = true;
		explosionTimer = Time.time;
		objectsDestroyed = false;
		
//		foreach (GameObject marker in markerList) 
//		{
//			Destroy(marker); 
//		}

		markerList.ForEach (m => Destroy (m));

		markerList.Clear();		// In press object

		AudioFile[(int)SoundQueueIndex.Poof].Play();

		ApplyPoints();
		ApplyBonusPoints();  // uses neighborlist
	}

	public void ClearMarkers()
	{
//		foreach (GameObject marker in markerList) 
//		{
//			Destroy(marker);
//		}

		markerList.ForEach (m => Destroy (m));

		markerList.Clear();	
		neighborList.Clear ();

		textManagerScript.AddMessage((int)TextQueueIndex.Bottom,"",-1, Color.red);

	}

	public void BuildRandomTower()
	{

		//     DESTROY ANY OLD TOWER FIRST     ***************************************


		theTower = new towerClass();
		//theTower.columnList = new List<columnClass>();
		theTower.columnList = new columnClass[matrixX,matrixZ];
		// Fill the tower
		// Populate the matrixes and instantiate the objects
		for (i = 0; i < matrixX; i++)
		for (k = 0; k < matrixZ; k++)
		{
				tmpTowerColumn = new columnClass();
				tmpTowerColumn.columnName = i.ToString()+",0,"+k.ToString();
				tmpTowerColumn.objectList = new List<objClass>();

			// fill up the columns
			for (j = 0; j < matrixY; j++)
			{
				// Assign and Instantiate random gem object to the array element
				int myTowerNum = UnityEngine.Random.Range(0,maxGemTypes);	// used to generate random regular gem type

				int rndNumber = rndBag.getRndNumber ();

				// Tower gets set here
				tmpTowerObject = new objClass();


				Vector3 tmpPosition = new Vector3(i,levelYPos[j],k);

				// Change to switch statement
				if (rndNumber < 480) 
				{
					tmpTowerObject.myObject = (GameObject)Instantiate (prefabs [gemStartOffset + myTowerNum], tmpPosition, Quaternion.identity);
				} 
				else if (rndNumber < 485) 
				{
					tmpTowerObject.myObject = (GameObject)Instantiate (specials[(int)specialsIndex.Wildcard], tmpPosition, Quaternion.identity);
				} 
				else if (rndNumber < 495) 
				{
					tmpTowerObject.myObject = (GameObject)Instantiate (specials[(int)specialsIndex.Pixie], tmpPosition, Quaternion.identity);
				}
				else
				{
					tmpTowerObject.myObject = (GameObject)Instantiate (specials[(int)specialsIndex.Plague], tmpPosition, Quaternion.identity);
				}

				tmpTowerObject.myType = tmpTowerObject.myObject.transform.name.Split('(')[0];
				tmpTowerObject.myObject.transform.position = tmpPosition;

				tmpTowerObject.myObject.transform.name = tmpPosition.ToString();
				tmpTowerObject.myObject.transform.parent = towerLevelObjects[j].transform;

				tmpTowerColumn.objectList.Add(tmpTowerObject);
			}

			theTower.columnList[i,k] = tmpTowerColumn;
		}

		
		for (j = 0; j < matrixY; j++) 
		{	
			levelInRotation[j] = false; 
			levelRotationStartTime[j] = 0;
		}
	}

	void MirrorCenterColumn()
	{


		foreach (Transform childTransform in centerColumn.transform) 
		{

			if (!childTransform.name.Equals("markerBlue(Clone)"))
				childTransform.gameObject.GetComponentInChildren<Gem_Script> ().DestroySelf ();
		}


		for (int j = 0; j < matrixY; j++) 
		{

			if (theTower.columnList [1,1].objectList [j].myObject) 
			{
				GameObject tmpObject = (GameObject)Instantiate (theTower.columnList [1,1].objectList [j].myObject, new Vector3 (4, theTower.columnList [1,1].objectList [j].myObject.transform.position.y, 1), theTower.columnList [1,1].objectList [j].myObject.transform.rotation);
				tmpObject.transform.parent = centerColumn.transform;
				// remove unnecessary components
			}
		}


}

	public void ScanForBonus()
	{
		int nValue;

		layerNeighborsFound = new int[6] {0,0,0,0,0,0};
		
		xRows = new int[matrixX,6];
		zRows = new int[matrixZ,6];

		columnNeighborsFound = new int[matrixX, matrixZ];
		
		// Parse neighbor list adding up row and column members
		foreach (objClass item in neighborList) 
		{
			layerNeighborsFound[(int)Mathf.Round( item.myObject.transform.position.y)]++;
			
			xRows[(int)Mathf.Round(item.myObject.transform.position.z),(int)Mathf.Round(item.myObject.transform.position.y)]++;
			zRows[(int)Mathf.Round(item.myObject.transform.position.x),(int)Mathf.Round(item.myObject.transform.position.y)]++;
			
			columnNeighborsFound[(int)Mathf.Round( item.myObject.transform.position.x),(int)Mathf.Round( item.myObject.transform.position.z)]++;
		}

		if (CenterShown ())
			nValue = matrixX * matrixZ;
		else
			nValue = (matrixX * matrixZ)-1;

		// Check for full layers and pop a layer marker if needed
		for (j = 0; j < 6; j++)
			if (layerNeighborsFound[j] == nValue)  
			{
				GameObject tmpObject = (GameObject) Instantiate(markerLayer, new Vector3(1,j,1) , Quaternion.identity);
				tmpObject.transform.parent = towerObject.transform;
				markerList.Add(tmpObject);
			}
		
		// Check for full rows and pop a row marker if needed
		for (k = 0; k < matrixZ; k++)
		for (j = 0; j < 6; j++)
		{
			if (xRows[k,j] == matrixX && (layerNeighborsFound[j] != nValue))
			{
				GameObject tmpObject = (GameObject) Instantiate(markerX, new Vector3(1,j,k) , Quaternion.Euler(0, 0, 90));
				tmpObject.transform.parent = towerObject.transform;
				markerList.Add(tmpObject);
			}
			
			if (zRows[k,j] == matrixZ && (layerNeighborsFound[j] != nValue))
			{
				GameObject tmpObject = (GameObject) Instantiate(markerZ, new Vector3(k,j,1) , Quaternion.Euler(90, 0, 0));
				tmpObject.transform.parent = towerObject.transform;
				markerList.Add(tmpObject);
			}
		}
		
		// Check for full columns and pop a column marker if needed
		for (i = 0; i < matrixX; i++)
			for (k = 0; k < matrixZ; k++)
				if (columnNeighborsFound[i,k] == 6)
				{
					GameObject tmpObject = (GameObject) Instantiate(markerY, new Vector3(i,2.5f,k) , Quaternion.identity);
					tmpObject.transform.parent = towerObject.transform;
					markerList.Add(tmpObject);
				}
	}
	
	public void ApplyPoints()
	{	
		statsManagerScript.currentStats.score += neighborList.Count*neighborList.Count;
		etherAmount += neighborList.Count;
	}

	// REWRITE to parse the marker list
	public void ApplyBonusPoints()
	{
		int i,j,k;
		int[] rowTotal = new int[6];
		int layerTotal = 0;
		int columnTotal = 0;
		int totalRows = 0;
		
		int totalBonus;
		int etherGained;
		int nValue;

		if (CenterShown ())
			nValue = 9;
		else
			nValue = 8;

		// Calculate layer bonus
		for (j = 0; j < 6; j++)
			if (layerNeighborsFound[j] == nValue)
			{
				layerTotal++;
			}
		
		// Calculate row bonus
		for (k = 0; k < matrixZ; k++)
		for (j = 0; j < 6; j++)
		{
				if (xRows[k,j] == matrixX && (layerNeighborsFound[j] != nValue))
			{
				rowTotal[j]++;
				totalRows++;
			}
			
				if (zRows[k,j] == matrixZ && (layerNeighborsFound[j] != nValue))
			{
				rowTotal[j]++;
				totalRows++;
			}
		}

		for (i = 0; i < matrixX; i++)
		for (k = 0; k < matrixZ; k++) 
		{
			if (columnNeighborsFound [i, k] == 6) 
			{
				columnTotal++;
			}
		}
		
		etherGained = totalRows * 5 + columnTotal * 25 + layerTotal * 25;

		etherAmount += etherGained;
		if (etherAmount > 100)
			etherAmount = 100;

		textManagerScript.AddMessage((int)TextQueueIndex.Bottom,( neighborList.Count*neighborList.Count).ToString()+ " Points!!",5.25f, Color.cyan, true, 50);
	}

	public void DestroyItems(List<objClass> tmpObjects)
	{
		GameObject tmp;

		foreach (objClass tmpObject in tmpObjects) 
		{
			if ((int)Mathf.Round (tmpObject.myObject.transform.position.x) == 1 && (int)Mathf.Round (tmpObject.myObject.transform.position.z) == 1 && showCenter)
			{
				tmp = (GameObject) Instantiate(poof, new Vector3(4, tmpObject.myObject.transform.position.y , 1), Quaternion.identity);
				// destroy center mirror object here
			}

			tmp = (GameObject) Instantiate(poof, tmpObject.myObject.transform.position, Quaternion.identity);
			tmpObject.myObject.transform.GetComponent<Gem_Script>().DestroySelf(); 

			theTower.columnList [(int)Mathf.Round (tmpObject.myObject.transform.position.x), (int)Mathf.Round (tmpObject.myObject.transform.position.z)].objectList [(int)tmpObject.myObject.transform.position.y].explodeMe = true;
		}

		groupExplosion = true;
		explosionTimer = Time.time;
		objectsDestroyed = false;
	}

	#region *** Neighbor Selection  Routines ***

	void MarkMyNeighbors(int myX, int myY, int myZ, string checkType)
	{
		neighborList.Clear();

		for (i = 0; i < matrixX; i++)
			for (k = 0; k < matrixZ; k++)
				for (j = 0; j <6; j++)
					theTower.columnList[i,k].objectList[j].alreadyChecked = false;

		FindMyMatchingNeighbors(myX, myY, myZ, checkType);

		for (i = 0; i < matrixX; i++)
			for (k = 0; k < matrixZ; k++)
				for (j = 0; j < 6; j++)
					theTower.columnList[i,k].objectList[j].alreadyChecked = false;
	}

	void MarkMyNeighbors(objClass obj, string checkType)
	{
		neighborList.Clear();

		for (i = 0; i < matrixX; i++)
			for (k = 0; k < matrixZ; k++)
				for (j = 0; j <6; j++)
					theTower.columnList[i,k].objectList[j].alreadyChecked = false;

		FindMyMatchingNeighbors(obj, checkType);

		for (i = 0; i < matrixX; i++)
			for (k = 0; k < matrixZ; k++)
				for (j = 0; j < 6; j++)
					theTower.columnList[i,k].objectList[j].alreadyChecked = false;
	}

	public List<objClass> FindMyCloseNeighbors(int myX, int myY, int myZ)
	{
		List<objClass> retVal = new List<objClass>();

		for (int i = -1; i <= 1; i++) 
			for (int j = -1; j <= 1; j++) 
				for (int k = -1; k <= 1; k++) 
				{
					int tx = myX + i;
					int ty = myY + j;
					int tz = myZ + k;

					if ((tx >= 0 && tx < matrixX) && (ty >= 0 && ty <= 5) && (tz >= 0 && tz < matrixZ) )
					if (!(i == 0 && j == 0 && k == 0))
						retVal.Add(theTower.columnList[tx,tz].objectList[ty]);
				}

		return retVal;
	}

	public List<objClass> FindMyCloseNeighbors(objClass obj)
	{
		List<objClass> retVal = new List<objClass>();

		for (int i = -1; i <= 1; i++) 
			for (int j = -1; j <= 1; j++) 
				for (int k = -1; k <= 1; k++) 
				{
					int tx = (int)Math.Round(obj.myObject.transform.position.x) + i;
					int ty = (int)Math.Round(obj.myObject.transform.position.y) + j;
					int tz = (int)Math.Round(obj.myObject.transform.position.z) + k;

					if ((tx >= 0 && tx < matrixX) && (ty >= 0 && ty <= 5) && (tz >= 0 && tz < matrixZ) )
					if (!(i == 0 && j == 0 && k == 0))
						retVal.Add(theTower.columnList[tx,tz].objectList[ty]);
				}

		return retVal;
	}

	// Recursive routine to find the group of matching neighbors
	void FindMyMatchingNeighbors(int myX, int myY, int myZ, string checkType)
	{	
		// Check for out of range
		if (myX < 0 || myY < 0 || myZ < 0 || myX > matrixX - 1 || myY > 6 - 1 || myZ > matrixZ - 1)
			return;

		// Dont waste time if it was already checked during a previous recursion
		if (theTower.columnList[myX,myZ].objectList[myY].alreadyChecked)
			return;

		// It has now been checked
		theTower.columnList[myX,myZ].objectList[myY].alreadyChecked = true;

		if (neighborList.Count > 0 && theTower.columnList [myX, myZ].objectList [myY].infected)
			return;


		// Check for center column usage
		if (myX == 1 && myZ == 1 && !CenterShown())  // This eliminates the center column from being included  ******************************************
			return;

		// if I dont match the original gem type then return unless I am a Wildcard
		if (theTower.columnList[myX,myZ].objectList[myY].myType != checkType  &&
			!theTower.columnList[myX,myZ].objectList[myY].myType.Equals("Wildcard"))
			return;

		// I must be a new match if I get to here
		neighborList.Add(theTower.columnList[myX,myZ].objectList[myY]);

		// pop a marker
		GameObject tmpObject = (GameObject) Instantiate(greenMarker, theTower.columnList[myX,myZ].objectList[myY].myObject.transform.position, Quaternion.identity);
		tmpObject.transform.parent = towerObject.transform;
		markerList.Add(tmpObject);

		if (myX == 1 && myZ == 1 )
		{	
			tmpObject = (GameObject) Instantiate(greenMarker, new Vector3(4, myY , 1), Quaternion.identity);
			tmpObject.transform.parent = centerColumn.transform;
			markerList.Add(tmpObject);
		}

		if (theTower.columnList [myX, myZ].objectList [myY].myType.Equals ("Medusa") ||
			theTower.columnList [myX, myZ].objectList [myY].myType.Equals ("Wizard") ||
			theTower.columnList [myX, myZ].objectList [myY].myType.Equals ("Pixie") ||
			theTower.columnList [myX, myZ].objectList [myY].myType.Equals ("Plague") ||
			theTower.columnList [myX, myZ].objectList [myY].infected 
		)
			return;

		// See if I have any matching neighbors
		FindMyMatchingNeighbors(myX+1, myY, myZ, checkType);
		FindMyMatchingNeighbors(myX-1, myY, myZ, checkType);
		FindMyMatchingNeighbors(myX, myY+1, myZ, checkType);
		FindMyMatchingNeighbors(myX, myY-1, myZ, checkType);
		FindMyMatchingNeighbors(myX, myY, myZ+1, checkType);
		FindMyMatchingNeighbors(myX, myY, myZ-1, checkType);

		return;
	}

	// Recursive routine to find the group of matching neighbors
	void FindMyMatchingNeighbors(objClass obj, string checkType)
	{	
		
		int myX = (int)Math.Round(obj.myObject.transform.position.x);
		int myY = (int)Math.Round(obj.myObject.transform.position.y);
		int myZ = (int)Math.Round(obj.myObject.transform.position.z);

		// Check for out of range
		if (myX < 0 || myY < 0 || myZ < 0 || myX > matrixX - 1 || myY > 6 - 1 || myZ > matrixZ - 1)
			return;

		// Dont waste time if it was already checked during a previous recursion
		if (obj.alreadyChecked)
			return;

		// It has now been checked
		obj.alreadyChecked = true;

		// If I am infected I will not match anything so return
		if (neighborList.Count > 0 && obj.infected)
			return;

		// Check for center column usage
		if (myX == 1 && myZ == 1 && !CenterShown())  // This eliminates the center column from being included  ******************************************
			return;

		// if I dont match the original gem type then return unless I am a Wildcard
		if (obj.myType != checkType  &&	!obj.myType.Equals("Wildcard"))
			return;

		// I must be a new match if I get to here
		neighborList.Add(obj);

		// pop a marker
		GameObject tmpObject = (GameObject) Instantiate(greenMarker, obj.myObject.transform.position, Quaternion.identity);
		tmpObject.transform.parent = towerObject.transform;
		markerList.Add(tmpObject);

		if (myX == 1 && myZ == 1 )
		{	
			tmpObject = (GameObject) Instantiate(greenMarker, new Vector3(4, myY , 1), Quaternion.identity);
			tmpObject.transform.parent = centerColumn.transform;
			markerList.Add(tmpObject);
		}

		// Cannot match on special items so return
		if (obj.myType.Equals ("Medusa") ||
			obj.myType.Equals ("Wizard") ||
			obj.myType.Equals ("Pixie")  ||
			obj.myType.Equals ("Plague") ||
			obj.infected 
		)
			return;

		// See if my neighbors match me
		if (myX + 1 < matrixX)
			FindMyMatchingNeighbors(theTower.columnList [myX+1, myZ].objectList [myY], checkType);
		if (myX - 1 >= 0 )
			FindMyMatchingNeighbors(theTower.columnList [myX-1, myZ].objectList [myY], checkType);
		if (myZ + 1 < matrixZ)
			FindMyMatchingNeighbors(theTower.columnList [myX, myZ+1].objectList [myY], checkType);
		if (myZ - 1 >= 0)
			FindMyMatchingNeighbors(theTower.columnList [myX, myZ-1].objectList [myY], checkType);
		if (myY + 1 < matrixY)
			FindMyMatchingNeighbors(theTower.columnList [myX, myZ].objectList [myY+1], checkType);
		if (myY - 1 >= 0 )
			FindMyMatchingNeighbors(theTower.columnList [myX, myZ].objectList [myY-1], checkType);

		return;
	}

	public void DestroyNeighborObjects()
	{
		GameObject tmpObject;

		foreach (objClass item in neighborList) 
		{
			if (Mathf.Round(item.myObject.transform.position.x) == 1 && Mathf.Round(item.myObject.transform.position.z) == 1) 
			{
				tmpObject = (GameObject) Instantiate(poof, new Vector3(4, item.myObject.transform.position.y , 1), Quaternion.identity);

			}

			tmpObject = (GameObject) Instantiate(poof, item.myObject.transform.position, Quaternion.identity);
			item.myObject.gameObject.GetComponent<Gem_Script>().DestroySelf();  //?????
		}		

		objectsDestroyed = true;
	}

	#endregion

	#region *** Object Lerping Routines ***

	// Prepare Dropper for rotation
	public void RotateDropperLeft()
	{
		if (towerInRotation || dropCount > 0) return;

		if (LevelsInRotation()) return;

		// Rotate the whole dropper counter clockwise/left
		RotateLevelLeft(6);
		RotateLevelLeft(7);
		RotateLevelLeft(8);
		RotateLevelLeft(9);
		RotateLevelLeft(10);
		RotateLevelLeft(11);

		AudioFile[(int)SoundQueueIndex.Rotate].Play();

	}

	// Prepare Dropper for rotation
	public void RotateDropperRight()
	{
		if (towerInRotation || dropCount > 0) return;

		if (LevelsInRotation()) return;

		RotateLevelRight (6);
		RotateLevelRight (7);
		RotateLevelRight (8);
		RotateLevelRight (9);
		RotateLevelRight (10);
		RotateLevelRight (11);

		AudioFile[(int)SoundQueueIndex.Rotate].Play();

	}

	// Prepare Tower for rotation
	public void RotateTowerLeft()
	{
		if (towerInRotation || dropCount > 0) return;

		if (LevelsInRotation()) return;

		// Rotate the whole tower counter clockwise/left
		towerStartRotation = new Quaternion(towerObject.transform.rotation.x, towerObject.transform.rotation.y, towerObject.transform.rotation.z, towerObject.transform.rotation.w);
		towerRotationStartTime = Time.time;
		towerInRotation = true;

		// Temporarily rotate and set the desired position
		towerObject.transform.Rotate(0,-90,0,Space.World);				
		towerDesiredRotation = new Quaternion(towerObject.transform.rotation.x, towerObject.transform.rotation.y, towerObject.transform.rotation.z, towerObject.transform.rotation.w);
		towerObject.transform.Rotate(0,90,0,Space.World);

		// Rotate the matrix values of each level to match
		RotateTowerMatrixLeft();
		AudioFile[(int)SoundQueueIndex.Rotate].Play();

	}

	// Prepare Tower for rotation
	public void RotateTowerRight()
	{
		if (towerInRotation || dropCount > 0) return;

		if (LevelsInRotation()) return;

		// Rotate the whole tower clockwise/right
		towerStartRotation = new Quaternion(towerObject.transform.rotation.x, towerObject.transform.rotation.y, towerObject.transform.rotation.z, towerObject.transform.rotation.w);
		towerRotationStartTime = Time.time;
		towerInRotation = true;

		// Temporarily rotate and set the desired position
		towerObject.transform.Rotate(0,90,0,Space.World);				
		towerDesiredRotation = new Quaternion(towerObject.transform.rotation.x, towerObject.transform.rotation.y, towerObject.transform.rotation.z, towerObject.transform.rotation.w);
		towerObject.transform.Rotate(0,-90,0,Space.World);

		// Rotate the matrix values of each level to match
		RotateTowerMatrixRight();
		AudioFile[(int)SoundQueueIndex.Rotate].Play();

	}
		
	// Rotate the tower object over time
	void towerObjectLerpRotation()
	{
		float rotationTime = 4.0f;  // 4.0f equates to .25 seconds
		
		// Rotate the groupBlock over a period of rotationTime using Lerp
		towerObject.transform.rotation = Quaternion.Lerp(towerStartRotation, towerDesiredRotation, (Time.time - towerRotationStartTime) * rotationTime );
		
		// Once rotationTime has elapsed and the Lerp is done set inRotation to false
		if (Time.time - towerRotationStartTime > 1/rotationTime) towerInRotation = false;
	}

	// Prepare Level for rotation
	public void RotateLevelLeft(int myLevel)
	{
		if (levelInRotation[myLevel] || towerInRotation || dropCount > 0) return;

		neighborList.Clear();	// In rotate level left

//		foreach (GameObject marker in markerList) 
//		{
//			Destroy(marker);
//		}
		markerList.ForEach (m => Destroy (m));
		markerList.Clear();  	// In rotate level left

		// Rotate the objects in that level counter clockwise/left
		levelStartRotation[myLevel] = new Quaternion(towerLevelObjects[myLevel].transform.rotation.x, towerLevelObjects[myLevel].transform.rotation.y, towerLevelObjects[myLevel].transform.rotation.z, towerLevelObjects[myLevel].transform.rotation.w);
		levelRotationStartTime[myLevel] = Time.time;
		levelInRotation[myLevel] = true;

		// Temporarily rotate to the desired position
		towerLevelObjects[myLevel].transform.Rotate(0,-90,0,Space.World);				

		// Set the desired rotation for lerping
		levelDesiredRotation[myLevel] =  new Quaternion(towerLevelObjects[myLevel].transform.rotation.x, towerLevelObjects[myLevel].transform.rotation.y, towerLevelObjects[myLevel].transform.rotation.z, towerLevelObjects[myLevel].transform.rotation.w);

		// Undo the previous rotation
		towerLevelObjects[myLevel].transform.Rotate(0,90,0,Space.World);

		// Rotate the matrix values of the level to match
		RotateLevelMatrixLeft(myLevel);
		AudioFile[(int)SoundQueueIndex.Rotate].Play();

	}

	//Prepare Level for rotation
	public void RotateLevelRight(int myLevel)
	{
		if (levelInRotation[myLevel] || towerInRotation || dropCount > 0) return;

		neighborList.Clear(); 	// In rotate level right

//		foreach (GameObject marker in markerList) 
//		{
//			Destroy(marker);
//		}
		markerList.ForEach (m => Destroy (m));
		markerList.Clear();		// In rotate level right

		// Rotate the objects in that level clockwise/right
		levelStartRotation[myLevel] = new Quaternion(towerLevelObjects[myLevel].transform.rotation.x, towerLevelObjects[myLevel].transform.rotation.y, towerLevelObjects[myLevel].transform.rotation.z, towerLevelObjects[myLevel].transform.rotation.w);
		levelRotationStartTime[myLevel] = Time.time;
		levelInRotation[myLevel] = true;

		// Temporarily rotate to the desired position
		towerLevelObjects[myLevel].transform.Rotate(0,90,0,Space.World);				

		// Set the desired rotation for lerping
		levelDesiredRotation[myLevel] =  new Quaternion(towerLevelObjects[myLevel].transform.rotation.x, towerLevelObjects[myLevel].transform.rotation.y, towerLevelObjects[myLevel].transform.rotation.z, towerLevelObjects[myLevel].transform.rotation.w);

		// Undo the previous rotation
		towerLevelObjects[myLevel].transform.Rotate(0,-90,0,Space.World);

		// Rotate the matrix values of the level to match
		RotateLevelMatrixRight(myLevel);
		AudioFile[(int)SoundQueueIndex.Rotate].Play();

	}

	// Rotate the level object over time
	void levelObjectLerpRotation(int lvl)
	{
		float rotationTime = 4.0f;  // 4.0f; // This equates to .25 seconds
		
		// Rotate the groupBlock over a period of rotationTime using Lerp
		towerLevelObjects[lvl].transform.rotation = Quaternion.Lerp(levelStartRotation[lvl], levelDesiredRotation[lvl], (Time.time - levelRotationStartTime[lvl]) * rotationTime );
		
		// Once rotationTime has elapsed and the Lerp is done set inRotation to false
		if (Time.time - levelRotationStartTime[lvl] > .25f) levelInRotation[lvl] = false;
	}
		
	// Drop the objects in Tower over time
	void towerObjectLerpDrop(int i, int j, int k)
	{
		objClass tmpobj = theTower.columnList[i,k].objectList[j];

		float dropTime = 4.0f;  // 4.0f; // This equates to .25 seconds

		// Drop the object over a period of dropTime using Lerp
		tmpobj.myObject.transform.position = Vector3.Lerp(tmpobj.startLocation,tmpobj.desiredLocation, (Time.time - tmpobj.dropStartTime) * dropTime);

		// Once drop has elapsed and the Lerp is done set dropping to false
		if (Time.time - tmpobj.dropStartTime > .25f) 
		{
			tmpobj.dropping = false;
			tmpobj.lastChangeTime = Time.time;
			dropCount--;
		}
	}

	#endregion

	#region *** Matrix Routines ***

	//  |2|5|8|
	//Z |1|4|7|
	//  |0|3|6|
	//     X


	// 0 = 0,0
	// 1 = 0,1
	// 2 = 0,2
	// 3 = 1,0
	// 4 = 1,1
	// 5 = 1,2
	// 6 = 2,0
	// 7 = 2,1
	// 8 = 2,2

	// Rotate the tower and dropper in the 3D matrix clockwise
	void RotateTowerMatrixRight()
	{
		columnClass tmp;
		
		tmp = theTower.columnList[0,2];
		theTower.columnList[0,2] = theTower.columnList[0,0];
		theTower.columnList[0,0] = theTower.columnList[2,0];
		theTower.columnList[2,0] = theTower.columnList[2,2];
		theTower.columnList[2,2] = tmp;

		tmp = theTower.columnList[0,1];
		theTower.columnList[0,1] = theTower.columnList[1,0];
		theTower.columnList[1,0] = theTower.columnList[2,1];
		theTower.columnList[2,1] = theTower.columnList[1,2];
		theTower.columnList[1,2] = tmp;

		ResetNames();
	}

	// Rotate the tower and dropper in the 3D matrix counter clockwise
	void RotateTowerMatrixLeft()
	{
		columnClass tmp;
		
		tmp = theTower.columnList[2,0];
		theTower.columnList[2,0] = theTower.columnList[0,0];
		theTower.columnList[0,0] = theTower.columnList[0,2];
		theTower.columnList[0,2] = theTower.columnList[2,2];
		theTower.columnList[2,2] = tmp;

		tmp = theTower.columnList[2,1];
		theTower.columnList[2,1] = theTower.columnList[1,0];
		theTower.columnList[1,0] = theTower.columnList[0,1];
		theTower.columnList[0,1] = theTower.columnList[1,2];
		theTower.columnList[1,2] = tmp;

		ResetNames();
	}

	// Rotate the level in the 3D matrix clockwise
	void RotateLevelMatrixRight(int lvl)
	{
		objClass tmp;
		
		tmp = theTower.columnList[0,2].objectList[lvl];
		theTower.columnList[0,2].objectList[lvl] = theTower.columnList[0,0].objectList[lvl];
		theTower.columnList[0,0].objectList[lvl] = theTower.columnList[2,0].objectList[lvl];
		theTower.columnList[2,0].objectList[lvl] = theTower.columnList[2,2].objectList[lvl];
		theTower.columnList[2,2].objectList[lvl] = tmp;
		
		tmp = theTower.columnList[0,1].objectList[lvl];
		theTower.columnList[0,1].objectList[lvl] = theTower.columnList[1,0].objectList[lvl];
		theTower.columnList[1,0].objectList[lvl] = theTower.columnList[2,1].objectList[lvl];
		theTower.columnList[2,1].objectList[lvl] = theTower.columnList[1,2].objectList[lvl];
		theTower.columnList[1,2].objectList[lvl] = tmp;
		
		ResetNames();
	}

	// Rotate the level in the 3D matrix counter clockwise
	void RotateLevelMatrixLeft(int lvl)
	{
		objClass tmp;
		
		tmp = theTower.columnList[2,0].objectList[lvl];
		theTower.columnList[2,0].objectList[lvl] = theTower.columnList[0,0].objectList[lvl];
		theTower.columnList[0,0].objectList[lvl] = theTower.columnList[0,2].objectList[lvl];
		theTower.columnList[0,2].objectList[lvl] = theTower.columnList[2,2].objectList[lvl];
		theTower.columnList[2,2].objectList[lvl] = tmp;
		
		tmp = theTower.columnList[2,1].objectList[lvl];
		theTower.columnList[2,1].objectList[lvl] = theTower.columnList[1,0].objectList[lvl];
		theTower.columnList[1,0].objectList[lvl] = theTower.columnList[0,1].objectList[lvl];
		theTower.columnList[0,1].objectList[lvl] = theTower.columnList[1,2].objectList[lvl];
		theTower.columnList[1,2].objectList[lvl] = tmp;
		
		ResetNames();
	}

	void DropObjectsInMatrix()
	{
		

		// Update theTower Matrix
		for (i = 0; i < matrixX; i++)
		for (k = 0; k < matrixZ; k++)
		{  
			// Now remove objects from the column list that are exploding
			theTower.columnList[i,k].objectList.RemoveAll(ii => ii.explodeMe);    //?????
		}

		dropCount = 0;

		// Update all tower columns
		for (i = 0; i < matrixX; i++)
		for (k = 0; k < matrixZ; k++)
		{
			int listLength = theTower.columnList[i,k].objectList.Count;
			
			// If elements have been removed from the column...
			if (listLength < matrixY)
			{
				// Prepare any elements in this column that need moved to be dropped
				for (int j = 0; j < listLength; j++) 
				{
						if (theTower.columnList[i,k].objectList[j].myObject.transform.position.y != levelYPos[j])
						{

						theTower.columnList[i,k].objectList[j].dropping = true;
						theTower.columnList[i,k].objectList[j].dropStartTime = Time.time+1;  // add a second for the poof to finish
						theTower.columnList[i,k].objectList[j].startLocation = 
								new Vector3( theTower.columnList[i,k].objectList[j].myObject.transform.position.x,
											 theTower.columnList[i,k].objectList[j].myObject.transform.position.y,
											 theTower.columnList[i,k].objectList[j].myObject.transform.position.z);

						theTower.columnList[i,k].objectList[j].desiredLocation = new Vector3(i,levelYPos[j],k);
						
						// re-parent the object to its new level
						theTower.columnList[i,k].objectList[j].myObject.transform.parent = towerLevelObjects[j].transform;

						theTower.columnList[i,k].objectList[j].lastChangeTime = Time.time;

						dropCount++;
				
					}
				}
			}
		}

		// Add all the new objects to fill the holes  
		for (i = 0; i < matrixX; i++)
		for (k = 0; k < matrixZ; k++)
		{
			int listLength = theTower.columnList[i,k].objectList.Count;
			
			int above = 10;
			for (int j = listLength; j < matrixY; j++) 
			{
				dropCount++;
				above++;
				
				float yPos;

				yPos = levelYPos[j];

				// Assign and Instantiate random gem object to the array element
				int myNum = UnityEngine.Random.Range(0,maxGemTypes);

				tmpTowerObject = new objClass();
				tmpTowerObject.startLocation = new Vector3(i,12+above,k);
				tmpTowerObject.desiredLocation = new Vector3(i,yPos,k);

				tmpTowerObject.dropping = true;


				int rndNumber = rndBag.getRndNumber ();


				if (rndNumber < 480) 
				{
					tmpTowerObject.myObject = (GameObject)Instantiate (prefabs [gemStartOffset + myNum], tmpTowerObject.startLocation, Quaternion.identity);
				} 
				else if (rndNumber < 485) 
				{
					tmpTowerObject.myObject = (GameObject)Instantiate (specials[(int)specialsIndex.Wildcard], tmpTowerObject.startLocation, Quaternion.identity);
				} 
				else if (rndNumber < 495) 
				{
						tmpTowerObject.myObject = (GameObject)Instantiate (specials[(int)specialsIndex.Pixie], tmpTowerObject.startLocation, Quaternion.identity);
				}
				else
				{
						tmpTowerObject.myObject = (GameObject)Instantiate (specials[(int)specialsIndex.Plague], tmpTowerObject.startLocation, Quaternion.identity);
				}

					tmpTowerObject.myType = tmpTowerObject.myObject.transform.name.Split('(')[0];

				tmpTowerObject.myObject.transform.name = new Vector3(i,levelYPos[j],k).ToString();
				tmpTowerObject.myObject.transform.parent = towerLevelObjects[j].transform;
				tmpTowerObject.dropStartTime = Time.time+1;
				
				theTower.columnList[i,k].objectList.Add(tmpTowerObject);
			}
		}
	}

	void ResetNames()
	{
		objClass thisObject;
		
		for (i = 0; i < matrixX; i++)
		for (k = 0; k < matrixZ; k++)
		{
			theTower.columnList[i,k].columnName = i.ToString()+",0,"+k.ToString();

			for (j = 0; j < matrixY; j++)		
			{
				thisObject = theTower.columnList[i,k].objectList[j];
				thisObject.myObject.transform.name = i.ToString()+","+levelYPos[j].ToString() +","+k.ToString();
			}
		}
	}

	#endregion



	public List<T> RandomizeList<T>(List<T> list)
	{
		List<T> randomizedList = new List<T> ();
		while (list.Count > 0) 
		{

			int index = UnityEngine.Random.Range (0, list.Count);
			randomizedList.Add (list [index]);
			list.RemoveAt (index);

		}
		return randomizedList;
	}


}
