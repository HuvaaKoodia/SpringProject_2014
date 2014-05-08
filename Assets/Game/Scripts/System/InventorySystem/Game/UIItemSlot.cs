using UnityEngine;
using System.Collections.Generic;

public delegate bool GameItemDelegate(InvGameItem item);

public abstract class UIItemSlot : MonoBehaviour
{
    public bool VendorSlot=false;

    public GameItemDelegate OnDragStart,OnItemDroppedToVendorSlot,OnItemReplaceEvent;
	public UISprite icon;
	public UIWidget background;
	public UILabel label;

	public AudioClip grabSound;
	public AudioClip placeSound;
	public AudioClip errorSound;

	InvGameItem mItem;
    public InvGameItem Item{get;private set;}
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

		foreach (var slot in UIAmmoSlot.EquipmentSlots){
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
        string t = GetItemBaseToolTip(item);

        for (int i = 0, imax = item.Stats.Count; i < imax; ++i)
        {
            InvStat stat = item.Stats[i];
            if (stat._amount == 0) continue;
            //DEV.HAX
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

    string GetItemBaseToolTip(InvGameItem item)
    {
        var t="[" + NGUIText.EncodeColor(item.color) + "]" + item.name + "[-]\n";
        
        if (item.baseItem.type!=InvBaseItem.Type.QuestItem)
            t += "[AFAFAF]MK." + item.itemLevel + " ";
        
        t+=item.baseItem.type;

        if (item.baseItem.AmmoData!=null&&item.baseItem.AmmoData.ShowInGame){
            t += "\n[AFAFAF]AmmoType: " + item.baseItem.AmmoData.Name + "[-]";
        }

        return t;
    }

    string GetTooltipText(InvGameItem item,InvGameItem compare){
        string t = GetItemBaseToolTip(item);

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
            if (stat.type==InvStat.Type.Range||stat.type==InvStat.Type.Value) continue;

            bool reverse_comparison=false;

            //DEV.HAX
            if (stat.type==InvStat.Type.Heat) reverse_comparison=true;

            string color="00FF00";//green
            string sign=":<better>";
            if (c_stat!=null&&((!reverse_comparison&&stat._amount<c_stat._amount)||(reverse_comparison&&stat._amount>c_stat._amount))){
                color="FF0000";//red
                sign=":<worse>";
            }
            if (c_stat!=null&&stat._amount==c_stat._amount){
                color="00C6FF";//blue
                sign=":<equal>";
            }

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
        if (stat==null) return "";
		return "[FFCC00]Price: " + stat._amount+" "+XmlDatabase.MoneyUnit+"[-]";
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

	public void Update ()
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
					icon.transform.localScale=Vector3.one*0.9f;
				}
			}
		}
	}

    
    /// <summary>
    /// Checks the vendor slot item interactions.
    /// Returns true if vendor slot and not vendor item.
    /// </summary>
    protected bool CheckVendorSlotItemInteractions(InvGameItem item)
    {
        if (item==null) return false;
        if (VendorSlot&&!item.VendorItem) return true;
        if (!VendorSlot&&item.VendorItem){
            if (OnItemDroppedToVendorSlot!=null){
                OnItemDroppedToVendorSlot(item);
            }
        }
        return false;
    }
}
