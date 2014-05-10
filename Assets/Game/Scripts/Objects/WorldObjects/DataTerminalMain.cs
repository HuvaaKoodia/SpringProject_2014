using UnityEngine;
using System.Collections;

public class DataTerminalMain : InteractableMain {

	public TileObjData.Obj Type{get;private set;}
	public GameController GC{get;set;}
	public DataTerminalGraphics graphics;

	public static int TypeToIndex(TileObjData.Obj type){
		return (int)type-TileObjData.TerminalStartIndex;
	}
	
	void Awake (){
		InteractCost = 0;
	}

	public override bool Interact(PlayerInteractSub interactSub)
	{
		GC.HUD.ActivateDataTerminalHUD(Type);
		return false;
	}

	public void SetType (TileObjData.Obj type)
	{
		Type=type;
		graphics.SetType(type);
	}
}
