using UnityEngine;
using System.Collections;
using System;

public class Character {
	public float X {
		get {
			return Mathf.Lerp(currTile.X, destTile.X, movementPercentage);
		}	
	}
	public float Y {
		get {
			return Mathf.Lerp(currTile.Y, destTile.Y, movementPercentage);
		}	
	}

	public Tile currTile {
		get;
		protected set;
	} 
	Tile destTile; //If we aren't moving, then destTile = currTile
	float movementPercentage; //goes from 0 to 1 as we move from currTile to destTile
	float speed = 2f; //Tiles per second
	Action<Character> cbCharacterChanged;

	Job myJob;

	public Character(Tile tile) {
		currTile = destTile = tile;
	}

	public void Update(float deltaTime) {

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
			if (myJob != null) {
				myJob.DoWork(deltaTime);
			}	
		}
		
		float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X - destTile.X, 2) + Mathf.Pow(currTile.Y - destTile.Y, 2));
		float distThisFrame = speed * deltaTime;
		float percThisFrame = distToTravel <= 0 ? 1 - movementPercentage : distThisFrame / distToTravel;

		movementPercentage += percThisFrame;

		if (movementPercentage >= 1) {
			currTile = destTile;
			movementPercentage = 0;
		}

		if (cbCharacterChanged != null) 
			cbCharacterChanged(this);
		
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
}
