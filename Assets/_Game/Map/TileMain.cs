using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMain : MonoBehaviour 
{
	public Rect size;
	public Vector3 position;
	public GameObject tileObject;
	
	public TileData Data{get;private set;}
	public void SetData(TileData data){
		Data=data;
	}

	public bool Blocked ()
	{
		return tileObject!=null;
	}

	
}
