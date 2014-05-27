using UnityEngine;
using System.Collections;

public class ElevatorMain : InteractableMain {

    public GameController GC;

	void Start(){
		InteractCost = 1;
	}

	public override bool Interact(PlayerInteractSub interactSub)
	{
		GC.UseElevator();
		return false;
	}
}
