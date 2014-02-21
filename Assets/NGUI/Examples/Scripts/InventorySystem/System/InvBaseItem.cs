using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InvBaseItem
{
	public enum Type
	{
		None,
		LightWeapon,
		HeavyWeapon,
		Utility,
        QuestItem,
		_Amount
	}

    public string name,description,ammotype;
    public AmmoXmlData AmmoData;
	public Type type = Type.None;

	public int minItemLevel = 1;
	public int maxItemLevel = 10;

	/// <summary>
	/// And and all base stats this item may have at a maximum level.
	/// Actual object's stats are calculated based on item's level and quality.
	/// </summary>

	public List<InvStat> stats = new List<InvStat>();

	/// <summary>
	/// Game Object that will be created and attached to the specified slot on the body.
	/// This should typically be a prefab with a renderer component, such as a sword,
	/// a bracer, shield, etc.
	/// </summary>

	public GameObject attachment;

	public Color color = Color.white;
	public UIAtlas iconAtlas;
	public string iconName = "";
}