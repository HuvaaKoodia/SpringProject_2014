using UnityEngine;
using System.Collections;

public class InteractableMain : MonoBehaviour {

	public PlayerInteractSub interactor;
	public int InteractCost;

	public virtual bool Interact(PlayerInteractSub interactSub)
	{
		interactSub.InteractFinished();
		return false;
	}
}
