﻿using UnityEngine;
using System.Collections;

public enum AlienAiState
{
	Flee = 1, Chase = 2, Rand = 3
}

public class AiLookupTable : MonoBehaviour {

	public AIBase owner;

	public bool AwareOfPlayer = false;
	public bool InformedOfPlayer = false;
	public uint TurnsWithoutPlayerAwareness = 0;

	public Point3D LastKnownPlayerPosition = null;

	public SearchNode Path = null;
	public Point3D Destination = null;

	public AlienAiState LatestPathType = 0;

	public bool Berserk = false;

	// Use this for initialization
	void Start () {
	
	}
}
