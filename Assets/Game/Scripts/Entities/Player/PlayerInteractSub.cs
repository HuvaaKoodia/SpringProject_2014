using UnityEngine;
using System.Collections;

public class PlayerInteractSub : MonoBehaviour {

	public PlayerMain player;

	InteractableMain interactableInfront;

	public bool WaitingInteractToFinish = false;

	public bool HasInteractable
	{ get { return interactableInfront != null; }}

	// Use this for initialization
	void Start () {
        CheckForInteractables();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CheckForInteractables()
	{
		interactableInfront = null;
		TileMain tileInfront = player.movement.GetTileInFront();

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

		player.GC.menuHandler.SetInteractVisibility(interactableInfront != null);
	}

	public void Interact(bool screenClick)
	{
		if (interactableInfront == null || (screenClick && !MouseHitInteractable()))
			return;

		if (interactableInfront != null)
		{
			
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

			if (player.ap == 0)
				player.EndPlayerPhase();
		}
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
