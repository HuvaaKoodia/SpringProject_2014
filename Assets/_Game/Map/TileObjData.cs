using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileObjData
{
	public enum Type {Floor,Wall};
	
	public Vector3 TilePosition;
	public Type TileType{get;private set;}
	
	public void SetType(Type type){
		TileType=type;
	}
	
	public TileObjData(){}
	
	
}
