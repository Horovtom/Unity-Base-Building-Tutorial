using UnityEngine;
using System.Collections;

public class DebuggingMenu : MonoBehaviour {

	public BuildModeController buildModeController; 

	// Use this for initialization
	void Start () {
		//buildModeController = (BuildModeController)(GameObject.Find("BuildModeController"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CreatePathfindingExample() {
		buildModeController.DoPathfindingTest();
	}

	public void ShowPathfindingDebuggingInfo() {
		buildModeController.DisplayPathfindingDebug();
	}
}
