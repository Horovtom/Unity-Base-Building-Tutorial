﻿using UnityEngine;
using System.Collections;

public class Path_Edge<T> {
	public Path_Node<T> node;
	public float cost; //Cost to traverse this edge (i.e.: cost to ENTER the tile)
}
