using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileObjData
{
	public enum Type {Floor,Wall,Empty,Door,Corridor,ElevatorTrigger,ElevatorDoor,Airlock};
	public enum Obj {None=0,Player,Enemy,Loot,Obstacle,LootArea,GatlingGun,GatlingGunArea,ArmoryTerminal=100,CargoTerminal,EngineTerminal,NavigationTerminal};

	public const int TerminalStartIndex=100;

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
