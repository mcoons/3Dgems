using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DigitalRuby.LightningBolt;
 
// This script is attached to the gem objects
// It handles tap, hold and flick gestures

public class WizardManager: MonoBehaviour 
{

	TowerManager TMScript;
	public float activationWait = 5.0f;
	//public float nextActivation;
	public List<objClass> currentWizards = new List<objClass>();
	public List<objClass> neighbors = new List<objClass> ();
	public List<objClass> enemies = new List<objClass> ();

	private void Start() 
	{ 
		TMScript = GameObject.Find("TowerManager").GetComponent<TowerManager>(); 
		//nextActivation = Time.time;
	}


	private void Update()
	{
		if (TMScript.TowerInRotation () || TMScript.LevelsInRotation () || TMScript.HasSelection () || TMScript.IsDropping())
			return;

		FindAllWizards ();

		currentWizards.RemoveAll (w => w.nextActivation > Time.time);

		if (currentWizards.Count == 0)
			return;

		currentWizards.ForEach (w => w.nextActivation = Time.time + activationWait + UnityEngine.Random.Range(0,500)/1000);
		ActivateWizards ();
	}

	private void ActivateWizards()
	{

		currentWizards = TMScript.RandomizeList (currentWizards);
			
		foreach (objClass wizard in currentWizards) 
		{
			if (Time.time - wizard.lastChangeTime < 2)
				continue;

			if (TMScript.etherAmount < 10)
				continue;

			neighbors.Clear ();

			neighbors = TMScript.FindMyCloseNeighbors ((int)Math.Round(wizard.myObject.transform.position.x), (int)Math.Round(wizard.myObject.transform.position.y), (int)Math.Round(wizard.myObject.transform.position.z));
			neighbors.RemoveAll (n => (Time.time - n.lastChangeTime) < 2);

			if (neighbors.Count == 0)
				continue;

			enemies = neighbors.FindAll (n => (n.myType == "Medusa")	);

			if (enemies.Count == 0)
				enemies = neighbors.FindAll (n => (n.myType == "Coal") );

			if (enemies.Count == 0)
				enemies = neighbors.FindAll (n => (n.myType == "Pixie"));

			if (enemies.Count == 0)
				enemies = neighbors.FindAll (n => (n.myType == "Wizard"));

			if (enemies.Count == 0)
				continue;

			objClass selection = enemies[ UnityEngine.Random.Range (0, enemies.Count)];

			int type = TMScript.gemStartOffset + UnityEngine.Random.Range (0, TMScript.maxGemTypes);

			Transform oldParent = selection.myObject.transform.parent;
			string oldName = selection.myObject.transform.name;
			Vector3 myPosition = selection.myObject.transform.position;

			selection.myObject.GetComponent<Gem_Script> ().DestroySelf ();
			selection.myObject = (GameObject)Instantiate (TMScript.prefabs [type], myPosition, Quaternion.identity);
			selection.myObject.transform.parent = oldParent;
			selection.myObject.transform.name = oldName;
			selection.myType =  TMScript.prefabs[type].name;
			selection.lastChangeTime = Time.time;
			TMScript.etherAmount -= 10;

			GameObject bolt =  GameObject.Instantiate (TMScript.greenBolt);

			bolt.transform.Find ("Sphere").transform.position = selection.myObject.transform.position;
			bolt.transform.Find ("Sphere").transform.parent = selection.myObject.transform;

			LightningBoltScript[] bolts = bolt.GetComponentsInChildren<LightningBoltScript> ();
			foreach (LightningBoltScript b in bolts) 
			{
				b.EndObject = selection.myObject;
				b.StartObject = wizard.myObject;
			}
		}

	}

	private void FindAllWizards()
	{
		currentWizards.Clear ();

		for (int x = 0; x< TMScript.matrixX; x++)
			for (int z = 0; z < TMScript.matrixZ; z++)
				currentWizards.AddRange(TMScript.theTower.columnList [x,z].objectList.FindAll (item => item.myType.Equals ("Wizard") && item.myObject.transform.position.y < 6));


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
