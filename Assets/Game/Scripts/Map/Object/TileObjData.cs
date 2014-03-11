using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileObjData
{
	public enum Type {Floor,Wall,Empty,Door,Corridor};
    public enum Obj {None,Player,Enemy,Loot,Obstacle,LootArea};
	
    Obj _obj;
    public int X,Y;
	public Type TileType{get;private set;}
    public Obj ObjType{get{return _obj;}}
    public ObjectXmlIndex ObjXml{get;private set;}
	
	public void SetType(Type type){
		TileType=type;
	}
	
    public void SetObj(Obj obj){
        _obj=obj;
    }

    public void SetObj(ObjectXmlIndex obj){
        ObjXml=obj;
        _obj=obj.type;
	}
}
