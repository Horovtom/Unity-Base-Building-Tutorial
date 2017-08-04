using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {

	public GameObject circleCursor;

	Vector3 lastFramePosition;
	Vector3 dragStartPosition;

	// Use this for initialization
	void Start () {
	
	}

	void UpdateCircleCursorPosition(Tile tileUnderMouse) {
		if (tileUnderMouse != null) {
			Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
			circleCursor.transform.position = cursorPosition;
			circleCursor.SetActive(true);
		} else {
			circleCursor.SetActive(false);
		}
	}

	void HandleLeftMouseClicks(Vector3 currFramePosition) {
		//Start drag
		if (Input.GetMouseButtonDown(0)) {
			dragStartPosition = currFramePosition;
		}
		//End drag
		if (Input.GetMouseButtonUp(0)) {
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

			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					Tile t = WorldController.Instance.World.GetTileAt(x, y);
					clickedOnTile(t);
				}
			}
		}
	}

	void clickedOnTile(Tile t) {
		if (t != null)
		t.Type = Tile.TileType.Floor;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		currFramePosition.z = 0;

		Tile tileUnderMouse = GetTileAtWorldCoord(currFramePosition);
		UpdateCircleCursorPosition(tileUnderMouse);

		HandleLeftMouseClicks(currFramePosition);

		//Handle screen dragging
		if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) { //Right or Middle mouse button
			Vector3 diff = lastFramePosition - currFramePosition;
			Camera.main.transform.Translate(diff);
		}

		lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		lastFramePosition.z = 0;
	}

	Tile GetTileAtWorldCoord(Vector3 coord) {
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.y);

		return WorldController.Instance.World.GetTileAt(x, y);
	}
}
