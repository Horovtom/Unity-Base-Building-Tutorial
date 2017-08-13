using UnityEngine;
using System.Collections.Generic;

public class Path_ScoreDict {
	Dictionary<Path_Node<Tile>, float> dict;

	public Path_ScoreDict() {
		dict = new Dictionary<Path_Node<Tile>, float>();
	}

	public float this[Path_Node<Tile> t] {
		get {
			if (dict.ContainsKey(t)) {
				return dict[t];
			} else {
				dict[t] = float.MaxValue;
				return float.MaxValue;
			}
		}
		set {
			dict[t] = value;
		}
	}

	public int Count() {
		return dict.Count;
	}
}
