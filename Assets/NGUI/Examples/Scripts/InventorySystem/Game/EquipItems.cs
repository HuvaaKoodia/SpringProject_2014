using UnityEngine;

/// <summary>
/// Equip the specified items on the character when the script is started.
/// </summary>

public class EquipItems : MonoBehaviour
{
	public int[] itemIDs;

	void Start ()
	{
		if (itemIDs != null && itemIDs.Length > 0)
		{
			InvEquipmentStorage eq = GetComponent<InvEquipmentStorage>();
			if (eq == null) eq = gameObject.AddComponent<InvEquipmentStorage>();

			int qualityLevels = (int)InvGameItem.Quality._Amount;

			for (int i = 0, imax = itemIDs.Length; i < imax; ++i)
			{
				int index = itemIDs[i];
				InvBaseItem item = InvDatabase.FindByID(index);

				if (item != null)
				{
					InvGameItem gi = new InvGameItem(index, item);
					gi.quality = (InvGameItem.Quality)Random.Range(0, qualityLevels);
					gi.itemLevel = NGUITools.RandomRange(item.minItemLevel, item.maxItemLevel);
					eq.Equip(gi);
				}
				else
				{
					Debug.LogWarning("Can't resolve the item ID of " + index);
				}
			}
		}
		Destroy(this);
	}
}