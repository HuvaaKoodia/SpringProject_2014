using UnityEngine;
using System.Collections;

public class DataTerminalMain : InteractableMain {

	public enum Type{Power=0,Armory=1,Command=2};
	public GameController GC;
	public GameObject graphics;
	public Type TerminalType;

	// Use this for initialization
	void Awake (){
		InteractCost = 0;
	}

	public override bool Interact(PlayerInteractSub interactSub)
	{
		GC.HUD.ActivateDataTerminalHUD(TerminalType);
		return false;
	}
}
