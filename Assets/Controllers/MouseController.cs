using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

	public GameObject circleCursorPrefab;

	Vector3 lastFramePosition;
	Vector3 currFramePosition;
	Vector3 dragStartPosition;
	bool isDragging = false;

	List<GameObject> dragPreviewGameObjects;

	BuildModeController bmc;

	// Use this for initialization
	void Start () {
		dragPreviewGameObjects = new List<GameObject>();
		bmc = GameObject.FindObjectOfType<BuildModeController>();
	}

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
					Tile t = WorldController.Instance.world.GetTileAt(x, y);
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
					Tile t = WorldController.Instance.world.GetTileAt(x, y);
					ClickedOnTile(t);
				}
			}
			isDragging = false;
		}
	}

	void ClickedOnTile(Tile t) {
		if (t != null) {
			bmc.DoBuild(t);
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
}
