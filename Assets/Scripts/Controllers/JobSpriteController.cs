using UnityEngine;
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

		GameObject job_go = new GameObject();

		//FIXME: Does not consider multi-tile objects nor rotated objects

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
