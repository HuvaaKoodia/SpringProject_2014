using UnityEngine;
using System.Collections;

public class DataTerminalMain : InteractableMain {

	public enum Type{Power,Armory,Command};
	public GameController GC;
	public GameObject graphics;
	public Type TerminalType;

	// Use this for initialization
	void Awake () {
		InteractCost = 0;
	}

	public override bool Interact(PlayerInteractSub interactSub)
	{
		GC.menuHandler.ActivateDataTerminalHUD(TerminalType);
		return false;
	}
}
