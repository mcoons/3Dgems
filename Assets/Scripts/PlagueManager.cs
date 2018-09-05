using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DigitalRuby.LightningBolt;
 
// This script is attached to the death objects
// It handles tap, hold and flick gestures

public class PlagueManager: MonoBehaviour 
{

	TowerManager TMScript;

	public float activationWait = 5.0f;
	public float nextActivation;
	public List<objClass> currentPlagues = new List<objClass>();
	public List<objClass> neighbors = new List<objClass> ();

	private void Start() 
	{ 
		TMScript = GameObject.Find("TowerManager").GetComponent<TowerManager>(); 
		nextActivation = Time.time;
	}


	private void Update()
	{

		if (TMScript.TowerInRotation () || TMScript.LevelsInRotation () || TMScript.HasSelection () || TMScript.IsDropping())
			return;

		FindAllPlagues ();

		currentPlagues.RemoveAll (w => w.nextActivation > Time.time);

		if (currentPlagues.Count == 0)
			return;

		currentPlagues.ForEach (w => w.nextActivation = Time.time + activationWait + UnityEngine.Random.Range(0,500)/1000);
		ActivatePlagues ();

//		if (Time.time >= nextActivation) 
//		{
//			FindAllPlagues ();
//
//			currentPlagues.RemoveAll (w => w.nextActivation > Time.time);
//
//
//			if (currentPlagues.Count > 0) 
//			{
//				nextActivation = Time.time + activationWait;
//				ActivatePlagues ();
//			}
//		}
	}

	private void ActivatePlagues()
	{
		//currentPlagues = TMScript.RandomizeList (currentPlagues);

		for (int i = 0; i < TMScript.matrixX; i++)
			for (int k = 0; k < TMScript.matrixZ; k++)
				for (int j = 0; j < 6; j++)
					TMScript.theTower.columnList[i,k].objectList[j].alreadyChecked = false;
		
		foreach (var p in currentPlagues) 
		{
			if (Time.time - p.lastChangeTime < 2)
				continue;
			
			neighbors.Clear ();

			// Find neighbors that match rules
			neighbors = TMScript.FindMyCloseNeighbors ((int)Math.Round(p.myObject.transform.position.x), (int)Math.Round(p.myObject.transform.position.y), (int)Math.Round(p.myObject.transform.position.z));
			neighbors.RemoveAll (n => (Time.time - n.lastChangeTime) < 2);

			neighbors.RemoveAll (n => n.myType == "Coal");
			neighbors.RemoveAll (n => n.myType == "Plague");
			neighbors.RemoveAll (n => n.myType == "Pixie");
			neighbors.RemoveAll (n => n.myType == "Wizard");
			neighbors.RemoveAll (n => n.infected);
			neighbors.RemoveAll (n => n.alreadyChecked);

			if (neighbors.Count == 0)
				continue;

			foreach (objClass n in neighbors) 
			{

				n.alreadyChecked = true;
				n.exposed += (float)((float)UnityEngine.Random.Range(1,10)/10);

				if (n.exposed > 5) 
				{
					n.infected = true;
					n.lastChangeTime = Time.time;

					GameObject i =  GameObject.Instantiate (TMScript.infection);
					i.transform.position = n.myObject.transform.position;
					i.transform.parent = n.myObject.transform;

					GameObject bolt =  GameObject.Instantiate (TMScript.deathBolt);

					bolt.transform.Find ("Sphere").transform.position = n.myObject.transform.position;
					bolt.transform.Find ("Sphere").transform.parent = n.myObject.transform;

					LightningBoltScript[] bolts = bolt.GetComponentsInChildren<LightningBoltScript> ();
					foreach (LightningBoltScript b in bolts) 
					{
						b.EndObject = n.myObject;
						b.StartObject = p.myObject;
					}
				}
			}

		}

	}

	private void FindAllPlagues()
	{
		currentPlagues.Clear ();

		for (int x = 0; x < TMScript.matrixX; x++)
			for (int z = 0; z < TMScript.matrixZ; z++) 
			{
				currentPlagues.AddRange (TMScript.theTower.columnList [x, z].objectList.FindAll (item => item.myType.Equals ("Plague") && item.myObject.transform.position.y < 6));
				currentPlagues.AddRange (TMScript.theTower.columnList [x, z].objectList.FindAll (item => item.infected && item.myObject.transform.position.y < 6));
			}
	}


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
