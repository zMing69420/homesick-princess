using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
	public GameObject droppedItemPrefab;
	public List<Loot> lootList = new List<Loot>();
	public float bounceForce = 10f;

	List<Loot> GetDroppedItems()
	{
		int randomNumber = Random.Range(1, 101);
		List<Loot> possibleItems = new List<Loot>();

		foreach (Loot item in lootList)
		{
			if (randomNumber <= item.dropChance)
			{
				possibleItems.Add(item);
			}
		}

		if (possibleItems.Count > 0)
		{
			int randomIndex = Random.Range(0, possibleItems.Count);
			List<Loot> selectedItem = new List<Loot> { possibleItems[randomIndex] };
			return selectedItem;
		}

		Debug.Log("No loot dropped");
		return new List<Loot>();
	}

	public void InstantiateLoot(Vector3 spawnPosition)
	{
		List<Loot> droppedItems = GetDroppedItems();
		if (droppedItems.Count > 0)
		{
			foreach (Loot droppedItem in droppedItems)
			{
				GameObject lootGameObject = Instantiate(droppedItemPrefab, spawnPosition, Quaternion.identity);
				lootGameObject.GetComponent<SpriteRenderer>().sprite = droppedItem.lootSprite;
				lootGameObject.name = droppedItem.lootName;

				Rigidbody2D rb = lootGameObject.GetComponent<Rigidbody2D>();
			}
		}
	}
}
