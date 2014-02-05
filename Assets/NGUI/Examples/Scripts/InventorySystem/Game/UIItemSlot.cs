using UnityEngine;
using System.Collections.Generic;

public delegate void OnDragStartEvent(InvGameItem item);

public abstract class UIItemSlot : MonoBehaviour
{

    protected OnDragStartEvent OnDragStart;
	public UISprite icon;
	public UIWidget background;
	public UILabel label;

	public AudioClip grabSound;
	public AudioClip placeSound;
	public AudioClip errorSound;

	InvGameItem mItem;
	string mText = "";

	static InvGameItem mDraggedItem;

	/// <summary>
	/// This function should return the item observed by this UI class.
	/// </summary>

	abstract protected InvGameItem observedItem { get; }

	/// <summary>
	/// Replace the observed item with the specified value. Should return the item that was replaced.
	/// </summary>

	abstract protected InvGameItem Replace (InvGameItem item);
	
	void OnTooltip (bool show)
	{
		InvGameItem item = show ? mItem : null;

		if (item != null)
		{
			InvBaseItem bi = item.baseItem;

			if (bi != null)
			{
				string t = "[" + NGUIText.EncodeColor(item.color) + "]" + item.name_long + "[-]\n";

				t += "[AFAFAF]MK." + item.itemLevel + " " + bi.type;

				List<InvStat> stats = item.Stats;

				for (int i = 0, imax = stats.Count; i < imax; ++i)
				{
					InvStat stat = stats[i];
					if (stat._amount == 0) continue;

					if (stat._amount < 0)
					{
						t += "\n[FF0000]" + stat._amount;
					}
					else
					{
						t += "\n[00FF00]+" + stat._amount;
					}

					if (stat.modifier == InvStat.Modifier.Percent) t += "%";
					t += " " + stat.type;
					t += "[-]";
				}

				if (!string.IsNullOrEmpty(bi.description)) t += "\n[FF9900]" + bi.description;
				UITooltip.ShowText(t);
				return;
			}
		}
		UITooltip.ShowText(null);
	}

	/// <summary>
	/// Allow to move objects around via drag & drop.
	/// </summary>

	void OnClick ()
	{
		if (mDraggedItem != null)
		{
			OnDrop(null);
		}
		else if (mItem != null)
		{
			StartDraggingItem(Replace(null));
			if (mDraggedItem != null) NGUITools.PlaySound(grabSound);
		}
	}

	/// <summary>
	/// Start dragging the item.
	/// </summary>

	void OnDrag (Vector2 delta)
	{
		if (mDraggedItem == null && mItem != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			StartDraggingItem(Replace(null));
			if (mDraggedItem != null) NGUITools.PlaySound(grabSound);
		}
	}

	/// <summary>
	/// Stop dragging the item.
	/// </summary>

	void OnDrop (GameObject go)
	{
		InvGameItem item = Replace(mDraggedItem);

		UICamera.current.ResetTooltipDelay();

		if (mDraggedItem == item){
			//wrong type for the slot
			NGUITools.PlaySound(errorSound);
		}
		else if (item != null) NGUITools.PlaySound(grabSound);
		else NGUITools.PlaySound(placeSound);

		StartDraggingItem(item);
	}

	void StartDraggingItem(InvGameItem item){
		mDraggedItem = item;
		UpdateCursor();
        if (OnDragStart!=null)
            OnDragStart(item);
	}

	/// <summary>
	/// Set the cursor to the icon of the item being dragged.
	/// </summary>

	void UpdateCursor ()
	{
		if (mDraggedItem != null && mDraggedItem.baseItem != null)
		{
			UICursor.Set(mDraggedItem.baseItem.iconAtlas, mDraggedItem.baseItem.iconName);
		}
		else
		{
			UICursor.Clear();
		}
	}

	/// <summary>
	/// Keep an eye on the item and update the icon when it changes.
	/// </summary>

	void Update ()
	{
		InvGameItem i = observedItem;

		if (mItem != i)
		{
			mItem = i;

			InvBaseItem baseItem = (i != null) ? i.baseItem : null;

			if (label != null)
			{
				string itemName = (i != null) ? i.name_linefeed : null;
				if (string.IsNullOrEmpty(mText)) mText = label.text;
				label.text = (itemName != null) ? itemName : mText;
			}
			
			if (icon != null)
			{
				if (baseItem == null || baseItem.iconAtlas == null)
				{
					icon.enabled = false;
				}
				else
				{
					icon.atlas = baseItem.iconAtlas;
					icon.spriteName = baseItem.iconName;
					icon.enabled = true;
					icon.MakePixelPerfect();
				}
			}

			/*
			if (background != null)
			{
				background.color = (i != null) ? i.color : Color.white;
			}*/
		}
	}
}
