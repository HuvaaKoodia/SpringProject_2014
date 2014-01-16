using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMain : MonoBehaviour 
{
	public TileObjData Data{get;private set;}
	public GameObject TileObject;	

	public void SetData(TileObjData data){
		Data=data;
	}

	
}
