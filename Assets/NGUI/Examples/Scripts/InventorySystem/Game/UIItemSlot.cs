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

    //DEV. puukkoa!

    void Start(){
        //OnDragStart+=UpdateSlotColors;
    }

    public void UpdateSlotColors(InvGameItem item){
        foreach (var slot in UIEquipmentSlot.EquipmentSlots){
            if (slot.slot!=UIEquipmentSlot.Slot.RecycleBin)
                slot.SetSlotColor(item);
        }
    }

    //Dev.end

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
            var t1=GetTooltipText(item);
            if (mDraggedItem!=null){
                t1=GetTooltipText(item,mDraggedItem);
                var t2=GetTooltipText(mDraggedItem,item);
                UITooltip.ShowText(t1,t2);
            }
            else{
                UITooltip.ShowText(t1);
            }
			
			return;
			
		}
        UITooltip.ClearTexts();
	}

    string GetTooltipText(InvGameItem item){
        string t = "[" + NGUIText.EncodeColor(item.color) + "]" + item.name + "[-]\n";
        
        t += "[AFAFAF]MK." + item.itemLevel + " " + item.baseItem.type;

        for (int i = 0, imax = item.Stats.Count; i < imax; ++i)
        {
            InvStat stat = item.Stats[i];
            if (stat._amount == 0) continue;
            //DEV.HAX
            if (stat.type==InvStat.Type.MaxAmmo&&stat._amount==-1) continue;
            if (stat.type==InvStat.Type.Range||stat.type==InvStat.Type.Value) continue;

            if (stat._amount < 0)
            {
                t += "\n[FF0000]" + stat._amount;
            }
            else
            {
                t += "\n[00FF00]" + stat._amount;
            }
            
            //if (stat.modifier == InvStat.Modifier.Percent) t += "%";
            t += " " + stat.type;
            t += "[-]";
        }

        t+="\n"+GetSellPriceText(item);
        
        t += "\n"+GetItemDescription(item);
        return t;
    }

    string GetTooltipText(InvGameItem item,InvGameItem compare){
        string t = "[" + NGUIText.EncodeColor(item.color) + "]" + item.name + "[-]\n";
        
        t += "[AFAFAF]MK." + item.itemLevel + " " + item.baseItem.type;
        
        List<InvStat> stats = item.Stats;
        
        for (int i = 0, imax = stats.Count; i < imax; ++i)
        {
            string tt="";
            InvStat stat = stats[i];
            InvStat c_stat=null;
            foreach (var s in compare.Stats){
                if (s.type==stat.type){
                    c_stat=s;
                }
            }
            if (stat._amount == 0) continue;
            //DEV.HAX
            if (stat.type==InvStat.Type.MaxAmmo&&stat._amount==-1) continue;
            if (stat.type==InvStat.Type.Range||stat.type==InvStat.Type.Value) continue;

            string color="00FF00";//green
            string sign="+";
            if (c_stat!=null&&stat._amount<c_stat._amount){
                color="FF0000";//red
                sign="-";
            }
            if (c_stat!=null&&stat._amount==c_stat._amount) color="00C6FF";//blue

            string type=stat.type.ToString();

            tt +=stat._amount+" " + type;
           // tt =tt.PadRight(20,' ');
            //tt+=sign;
            t+="\n["+color+"] "+sign+" "+tt+"[-]";
        }

        t+="\n"+GetSellPriceText(item);
        t += "\n"+GetItemDescription(item);
        return t;
    }

    string GetItemDescription(InvGameItem item){
        if (!string.IsNullOrEmpty(item.baseItem.description)) return "[FF0066]" + item.baseItem.description;
        return "";
    }

    string GetSellPriceText(InvGameItem item)
    {
        var stat=item.Stats.Find(s=>s.type==InvStat.Type.Value);
        return "[FFCC00]Price: " + stat._amount+" BC[-]";
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
        UpdateSlotColors(item);
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
				string itemName = (i != null) ? (i.name + "\nLvl."+i.itemLevel): null;
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
