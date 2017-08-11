using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

	public GameObject circleCursorPrefab;

	bool buildModeIsObjects = false;
	TileType buildModeTile = TileType.Floor;
	string buildModeObjectType;

	Vector3 lastFramePosition;
	Vector3 currFramePosition;
	Vector3 dragStartPosition;
	bool isDragging = false;

	List<GameObject> dragPreviewGameObjects;

	// Use this for initialization
	void Start () {
		dragPreviewGameObjects = new List<GameObject>();
	}

	/*void UpdateCursor() {
		Tile tileUnderMouse = WorldController.Instance.World.GetTileAt(
			Mathf.FloorToInt(currFramePosition.x), 
			Mathf.FloorToInt(currFramePosition.y));
		if (tileUnderMouse != null) {
			Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
			circleCursor.transform.position = cursorPosition;
			circleCursor.SetActive(true);
		} else {
			circleCursor.SetActive(false);
		}
	}*/

	void UpdateDragging() {
		

		//Start drag
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
			dragStartPosition = currFramePosition;
			isDragging = true;
		}

		//Return if not dragging
		if (!isDragging)
			return;

		#region Update start and end variables
		int startX = Mathf.FloorToInt(dragStartPosition.x);
		int startY = Mathf.FloorToInt(dragStartPosition.y);
		int endX = Mathf.FloorToInt(currFramePosition.x);
		int endY = Mathf.FloorToInt(currFramePosition.y);
		if (startX > endX) {
			int tmp = startX;
			startX = endX;
			endX = tmp;
		}
		if (startY > endY ) {
			int tmp = startY;
			startY = endY;
			endY = tmp;
		}
		#endregion

		//Clean up old drag previews
		while(dragPreviewGameObjects.Count > 0) {
			GameObject go = dragPreviewGameObjects[0];
			dragPreviewGameObjects.RemoveAt(0);
			SimplePool.Despawn(go);
		}

		if (Input.GetMouseButton(0)) {
			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					Tile t = WorldController.Instance.World.GetTileAt(x, y);
					if (t != null) {
						GameObject go = SimplePool.Spawn(circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
						go.transform.SetParent(this.transform, true);
						dragPreviewGameObjects.Add(go);
					}
				}
			}
		}

		//End drag
		if (Input.GetMouseButtonUp(0)) {
			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					Tile t = WorldController.Instance.World.GetTileAt(x, y);
					ClickedOnTile(t);
				}
			}
			isDragging = false;
		}
	}

	void ClickedOnTile(Tile t) {
		if (t != null) {
			if (buildModeIsObjects) {

				//This instantly builds the furniture:
				//WorldController.Instance.World.PlaceFurniture(buildModeObjectType, t);

				// Can we build the furniture in the selected tile?
				//Run the ValidPlacement function!

				if (	WorldController.Instance.World.IsFurniturePlacementValid(buildModeObjectType, t)
					&& 	t.pendingFurnitureJob == null) {
					//This tile is valid for this furniture
					//Create a job for it to be build

					string furnitureType = buildModeObjectType;
					Job j = new Job(t, (theJob) => {
						WorldController.Instance.World.PlaceFurniture(furnitureType, theJob.tile);
						t.pendingFurnitureJob = null;
					});

					//FIXME: I dont't like having to manually and explicitly set flags that prevent conflicts. It's too easy to forget to set/clear them!
					t.pendingFurnitureJob = j;

					j.RegisterJobancelCallback((theJob) => {
						theJob.tile.pendingFurnitureJob = null;
					});

					//Add a job to the queue
					WorldController.Instance.World.jobQueue.Enqueue(j);
					Debug.Log("Job Queue Size: " + WorldController.Instance.World.jobQueue.Count);
				}


			} else 
				t.Type = buildModeTile;
		}
	}

	void UpdateCameraMovement() {
		if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) { //Right or Middle mouse button
			Vector3 diff = lastFramePosition - currFramePosition;
			Camera.main.transform.Translate(diff);
		}

		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 20f);
	}


	
	// Update is called once per frame
	void Update () {
		currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		currFramePosition.z = 0;

		//UpdateCursor();
		UpdateDragging();
		UpdateCameraMovement();

		lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		lastFramePosition.z = 0;
	}

	public void SetMode_BuildFloor() {
		buildModeTile = TileType.Floor;
		buildModeIsObjects = false;
	}

	public void SetMode_Bulldoze() {
		buildModeTile = TileType.Empty;
		buildModeIsObjects = false;
	}

	public void SetMode_BuildFurniture(string objectType) {
		//Wall is not a Tile! Wall is a Furniture
		buildModeIsObjects = true;
		buildModeObjectType = objectType;
	}
}
