using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Creates item storage slots in Hud based on a InvItemStorage.
/// </summary>
public class UIItemStorage : MonoBehaviour
{

	public InvItemStorage ItemStorage;

    public void SetItemStorage(InvItemStorage store){
        ItemStorage=store;
        UpdateSlots();
    }

	public GameObject SlotPrefab;

	/// <summary>
	/// Background widget to scale after the item slots have been created.
	/// </summary>

	public UIWidget background;

	/// <summary>
	/// Spacing between slots.
	/// </summary>

	public int spacing = 128;

	/// <summary>
	/// Padding around the border.
	/// </summary>

	public int padding = 10;

	/// <summary>
	/// Initialize the container and create an appropriate number of UI slots.
	/// </summary>

	public List<UIStorageSlot> Slots;

	void Start ()
	{
		//UpdateSlots();

	}

	public void ChangeItemStorage (InvItemStorage items)
	{
		ItemStorage=items;

		UpdateSlots();

	}

	void UpdateSlots ()
	{
		if (SlotPrefab == null||ItemStorage==null) return;

		if (Slots==null) Slots=new List<UIStorageSlot>();

		if (Slots.Count>0){
			foreach (var s in Slots)
				NGUITools.Destroy(s.gameObject);
		}
		Slots.Clear();

		int count = 0;
		Bounds b = new Bounds();
		
		for (int y = 0; y < ItemStorage.maxRows; ++y)
		{
			for (int x = 0; x < ItemStorage.maxColumns; ++x)
			{
				GameObject go = NGUITools.AddChild(gameObject, SlotPrefab);
				Transform t = go.transform;
				t.localPosition = new Vector3(padding + (x + 0.5f) * spacing, -padding - (y + 0.5f) * spacing, 0f);
				
				UIStorageSlot slot = go.GetComponent<UIStorageSlot>();
				Slots.Add(slot);
				if (slot != null)
				{
					slot.storage = ItemStorage;
					slot.slot = count;
				}
				++count;
				b.Encapsulate(new Vector3(padding * 2f + (x + 1) * spacing, -padding * 2f - (y + 1) * spacing, 0f));
			}
		}
		if (background != null) background.transform.localScale = b.size;
	}
}