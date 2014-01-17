using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileObjData
{
	public enum Type {Floor,Wall,Empty};
	public enum Obj {None,Player,Enemy};
	
	public Vector3 TilePosition;
	public Type TileType{get;private set;}
	public Obj ObjType{get;private set;}
	
	public void SetType(Type type){
		TileType=type;
	}
	
	public void SetObj(Obj obj){
		ObjType=obj;
	}
	
	public TileObjData(){
	}
	
	
}
