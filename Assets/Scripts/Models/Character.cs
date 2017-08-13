using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class Character : IXmlSerializable {
	public float X {
		get {
			return nextTile != null ? Mathf.Lerp(currTile.X, nextTile.X, movementPercentage) : currTile.X;
		}	
	}
	public float Y {
		get {
			return nextTile != null ? Mathf.Lerp(currTile.Y, nextTile.Y, movementPercentage) : currTile.Y;
		}	
	}

	public Tile currTile {
		get;
		protected set;
	} 
	Tile destTile; //If we aren't moving, then destTile = currTile
	Tile nextTile; //The next tile in the pathfinding sequence
	Path_AStar pathAStar;
	float movementPercentage; //goes from 0 to 1 as we move from currTile to destTile
	float speed = 2f; //Tiles per second
	Action<Character> cbCharacterChanged;

	Job myJob;

	public Character(Tile tile) {
		currTile = destTile = nextTile = tile;
	}

	[System.Obsolete("Should be used only for serialization")]
	public Character() {
		
	}

	public void Update(float deltaTime) {
		if (!Update_DoJob(deltaTime))
			return;

		Update_DoMovement(deltaTime);

		if (cbCharacterChanged != null) 
			cbCharacterChanged(this);
	}

	void Update_DoMovement(float deltaTime) {
		if (currTile == destTile) {
			pathAStar = null;
			return;
		}

		if (nextTile == null || nextTile == currTile) {
			//Get the next tile from the pathfinder
			if (pathAStar == null) {
				//Generate a path to our destination
				pathAStar = new Path_AStar(currTile.world, currTile, destTile);
				if (pathAStar.Length() == 0 && currTile != destTile) {
					Debug.LogError("Path_AStar returned no path to destination!");
					//FIXME: Job should maybe be re-enqued instead?
					AbandonJob();
					pathAStar = null;
					return;
				}
			}

			nextTile = pathAStar.Dequeue();

			if (nextTile == currTile) {
				Debug.LogError("Update_DoMovement - nextTile is currTile?");
			}
		}

		float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X - nextTile.X, 2) + Mathf.Pow(currTile.Y - nextTile.Y, 2));
		float distThisFrame = speed * deltaTime;
		float percThisFrame = distToTravel <= 0 ? 1 - movementPercentage : distThisFrame / distToTravel;

		movementPercentage += percThisFrame;

		if (movementPercentage >= 1) {
			//TODO: Get the next tile from the pathfinding system.
			//If there are no more tiles, then we have truly reached our destination.


			currTile = nextTile;
			movementPercentage = 0;
		}



		return;
	}

	public void AbandonJob() {
		nextTile = destTile = currTile;
		pathAStar = null;
		currTile.world.jobQueue.Enqueue(myJob);
		myJob = null;
	}

	bool Update_DoJob(float deltaTime) {
		if (myJob == null) {
			myJob = currTile.world.jobQueue.Dequeue();

			if(myJob != null) {
				//We have a job!
				destTile = myJob.tile;
				myJob.RegisterJobCancelCallback(OnJobEnded);
				myJob.RegisterJobCompleteCallback(OnJobEnded);
			}
		}

		if (currTile == destTile) {
			pathAStar = null;
			if (myJob != null) {
				myJob.DoWork(deltaTime);
			}	

			return false;
		}

		return true;
	}

	public void SetDestination(Tile tile) {
		if (currTile.IsNeighbour(tile, true) == false) {
			Debug.Log("Character::SetDestination -- Our destination tile isn't actually our neighbour");
		}

		destTile = tile;
	}

	public void RegisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged += cb;
	}

	public void UnregisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged -= cb;
	}

	void OnJobEnded(Job j) {
		//Job completed or was cancelled....

		if (j != myJob) {
			Debug.LogError("Character being told about job that isn't his. You forgot to unregister something.");
			return;
		}

		myJob = null;
	}

	//////////////////////////////////////////
	/// 		
	/// 			SAVING & LOADING
	/// 
	/////////////////////////////////////////

	#region Saving & Loading

	public XmlSchema GetSchema() {
		return null;
	}

	public void WriteXml(XmlWriter writer) {
		//Save info here
		writer.WriteAttributeString("X", currTile.X.ToString());
		writer.WriteAttributeString("Y", currTile.Y.ToString());

	}

	public void ReadXml(XmlReader reader) {
		//Load info here

	}

	#endregion
}
