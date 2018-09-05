using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DigitalRuby.LightningBolt;
 
// This script is attached to the death objects
// It handles tap, hold and flick gestures

public class DeathManager: MonoBehaviour 
{

	TowerManager TMScript;

	public float activationWait = 5.0f;
	public float nextActivation;
	public List<objClass> currentMedusas = new List<objClass>();
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

		FindAllMedusas ();

		currentMedusas.RemoveAll (w => w.nextActivation > Time.time);

		if (currentMedusas.Count == 0)
			return;

		currentMedusas.ForEach (w => w.nextActivation = Time.time + activationWait + UnityEngine.Random.Range(0,500)/1000);
		ActivateMedusas ();

//		if (Time.time >= nextActivation) 
//		{
//			FindAllMedusas ();
//
//			currentMedusas.RemoveAll (w => w.nextActivation > Time.time);
//
//
//			if (currentMedusas.Count > 0) 
//			{
//				nextActivation = Time.time + activationWait;
//				ActivateMedusas ();
//			}
//		}
	}

	private void ActivateMedusas()
	{
		currentMedusas = TMScript.RandomizeList (currentMedusas);

		foreach (var death in currentMedusas) 
		{
			if (Time.time - death.lastChangeTime < 2)
				continue;
			
			neighbors.Clear ();

			// Find neighbors that match rules
			neighbors = TMScript.FindMyCloseNeighbors ((int)Math.Round(death.myObject.transform.position.x), (int)Math.Round(death.myObject.transform.position.y), (int)Math.Round(death.myObject.transform.position.z));
			neighbors.RemoveAll (n => (Time.time - n.lastChangeTime) < 2);

			neighbors.RemoveAll (n => n.myType == "Coal");
			neighbors.RemoveAll (n => n.myType == "Medusa");
			neighbors.RemoveAll (n => n.myType == "Wizard");

			if (neighbors.Count == 0)
				continue;

			// Select a random neighbor
			objClass selection = neighbors[ UnityEngine.Random.Range (0, neighbors.Count)];

			// Replace and update neighbor object
			Transform oldParent = selection.myObject.transform.parent;
			Vector3 myPosition = selection.myObject.transform.position;
			string oldName = selection.myObject.transform.name;

			selection.myObject.GetComponent<Gem_Script> ().DestroySelf ();
			selection.myObject = (GameObject)Instantiate (TMScript.specials[0], myPosition, Quaternion.identity);
			selection.myObject.transform.parent = oldParent;
			selection.myObject.transform.name = oldName;
			selection.lastChangeTime = Time.time;
			selection.myType = "Coal";

			// Spawn a lightning bolt and set locations
			GameObject bolt =  GameObject.Instantiate (TMScript.deathBolt);

			bolt.transform.Find ("Sphere").transform.position = selection.myObject.transform.position;
			bolt.transform.Find ("Sphere").transform.parent = selection.myObject.transform;

			LightningBoltScript[] bolts = bolt.GetComponentsInChildren<LightningBoltScript> ();
			foreach (LightningBoltScript b in bolts) 
			{
				b.EndObject = selection.myObject;
				b.StartObject = death.myObject;
			}
		}

	}

	private void FindAllMedusas()
	{
		currentMedusas.Clear ();

		for (int x = 0; x< TMScript.matrixX; x++)
			for (int z = 0; z < TMScript.matrixZ; z++)
				currentMedusas.AddRange(TMScript.theTower.columnList [x,z].objectList.FindAll (item => item.myType.Equals("Medusa") && item.myObject.transform.position.y < 6));
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
