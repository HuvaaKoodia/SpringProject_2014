using UnityEngine;
using System.Collections;

public class PlayerInteractSub : MonoBehaviour {

	public PlayerMain player;

	InteractableMain interactableInfront;

	public bool WaitingInteractToFinish = false;

	public bool HasInteractable
	{ get { return interactableInfront != null; }}

	public void CheckForInteractables()
	{
		interactableInfront = null;
		TileMain tileInfront = player.movement.GetTileInFront();

		if (tileInfront == null || tileInfront.entityOnTile != null)
		{
			player.GC.HUD.SetInteractVisibility(false);
			return;
		}

		GameObject objInfront = tileInfront.TileObject;
		InteractableMain infront = null;

		if (objInfront != null)
		{
			infront = objInfront.GetComponent<InteractableMain>();
		}

		if (infront != null)
		{
			interactableInfront = infront;
		}

		player.GC.HUD.SetInteractVisibility(interactableInfront != null);
	}

	public void Interact(bool screenClick)
	{
		if (interactableInfront == null || (screenClick && !MouseHitInteractable()))
			return;

		if (player.ap < interactableInfront.InteractCost)
			return;

		WaitingInteractToFinish = true;

		if (interactableInfront.Interact(this))
		{
			player.ap -= interactableInfront.InteractCost;
		}
		else
		{
			WaitingInteractToFinish = false;
		}
		
		player.CullWorld(false);

		if (player.ap == 0)
			player.EndPlayerPhase();
	}

	public bool MouseHitInteractable()
	{
		Component hit;
		Subs.GetObjectMousePos(out hit, MapGenerator.TileSize.x+2, "Interactable");

		if (hit == null)
			return false;

		if (hit.transform == interactableInfront.transform)
			return true;

		return false;
	}

	public void InteractFinished()
	{
		WaitingInteractToFinish = false;

		if (player.Finished)
			player.EndPlayerPhase();
	}
}
