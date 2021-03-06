﻿using UnityEngine;
using System.Collections.Generic;

public class JobSpriteController : MonoBehaviour {

	//THis bare-bones controller is mostly just going to piggyback 
	//on FurnitureSpriteController because we don't yet fully know 
	//what our job system is going to look like in the end.

	FurnitureSpriteController fsc;
	Dictionary<Job, GameObject> jobGameObjectMap;

	// Use this for initialization
	void Start () {
		fsc = GameObject.FindObjectOfType<FurnitureSpriteController>();
		jobGameObjectMap = new Dictionary<Job, GameObject>();
		//FIXME: No such thing as a job queue yet!
		WorldController.Instance.world.jobQueue.RegisterJobCreationCallback(OnJobCreated);
	}

	void OnJobCreated(Job j) {
		//FIXME: We can only do furniture-building jobs.

		//TODO: Sprite


		//FIXME: Does not consider multi-tile objects nor rotated objects

		if (jobGameObjectMap.ContainsKey(j)) {
			Debug.LogError("OnJobCreated for a jobGO that already exists -- most likely a job being RE-QUEUED, as opposed to created");
			return;
		}
		GameObject job_go = new GameObject();

		jobGameObjectMap.Add(j, job_go);

		job_go.name = "JOB_" + j.jobObjectType + "(" + j.tile.X + ", " + j.tile.Y + ")";
		job_go.transform.position = new Vector3(j.tile.X, j.tile.Y, 0);
		job_go.transform.SetParent(this.transform, true);

		//FIXME: We assume that the object must be a wall, so use the hardcoded reference to the wall sprite.
		SpriteRenderer sr = job_go.AddComponent< SpriteRenderer>();
			sr.sprite = fsc.GetSpriteForFurniture(j.jobObjectType);
		//Set transparency
		sr.color = new Color(0.7f, 1f, 0.7f, 0.3f);
		sr.sortingLayerName = "Jobs";

		//FIXME: This hardconding is not ideal!
		if (j.jobObjectType == "Door") {
			//By default, the door graphic is meant for walls to the east & west
			//Check to see if we actually have a wall noirth/south, and if so, then 
			//rotate this GO by 90 degrees
			Tile northTile = j.tile.world.GetTileAt(j.tile.X, j.tile.Y + 1);
			Tile southTile = j.tile.world.GetTileAt(j.tile.X, j.tile.Y - 1);
			if (northTile != null && southTile != null && northTile.furniture != null && southTile.furniture != null &&
				northTile.furniture.ObjectType == "Wall" && southTile.furniture.ObjectType == "Wall") {
				job_go.transform.rotation = Quaternion.Euler(0, 0, 90);
				job_go.transform.Translate(1f, 0,0, Space.World); //Ugly hack to compensate for bottom_left anchor point
			}
		}

		j.RegisterJobCompleteCallback(OnJobEnded);
		j.RegisterJobCancelCallback(OnJobEnded);
	}

	void OnJobEnded(Job job) {
		//This executes whether a job was COMPLETED or CANCELLED

		//TODO: Delete Sprite
		if (jobGameObjectMap.ContainsKey(job) == false) {
			Debug.LogError("OnJobEnded - No such job!");
			return;
		}
		GameObject job_go = jobGameObjectMap[job];
		job.UnregisterJobCancelCallback(OnJobEnded);
		job.UnregisterJobCompleteCallback(OnJobEnded);

		Destroy(job_go);
	}
}
