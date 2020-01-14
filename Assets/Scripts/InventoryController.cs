using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
	#region Private Members
	private GameObject[] slotsList;
	private int score;
	private readonly bool isFreeSlots;
	#endregion

	#region Public Members
	public Canvas canvas;
	public GameObject itemPrefab;
	public Text text;
	public List<Sprite> woodSprites;
	public List<Sprite> ironSprites;
	public List<Sprite> stoneSprites;
	#endregion

	#region Private Methods
	private void Awake()
	{
		slotsList = GameObject.FindGameObjectsWithTag("Slot");
		text.text = "Score: 0";
	}

	private ItemType GetRandomItemType()
	{
		Array values = Enum.GetValues(typeof(ItemType));
		System.Random random = new System.Random();
		ItemType randomItemType = (ItemType)values.GetValue(random.Next(values.Length));
		return randomItemType;
	}
	#endregion

	#region Public Methods
	public void AddElement()
	{
		int freeslots = 0;
		foreach (GameObject slot in slotsList)
		{
			if (slot.transform.childCount == 0)
			{
				freeslots++;
			}
		}
		if (freeslots == 0)
		{
			Debug.Log("No empty Slots");
			return;
		}
		GameObject obj = Instantiate(itemPrefab) as GameObject;
		Item item = obj.GetComponent<Item>();
		item.level = 1;
		item.itemType = GetRandomItemType();
		item.image.sprite = ChangeSprite(item.itemType, item.level);

		bool created = false;
		while (!created)
		{
			int index = UnityEngine.Random.Range(0, slotsList.Length);
			if (slotsList[index].transform.childCount == 0)
			{
				obj.transform.SetParent(slotsList[index].transform);
				RectTransform rt = obj.GetComponent<RectTransform>();
				rt.anchoredPosition = Vector3.zero;
				item.Initialize();
				created = true;
				break;
			}
		}
	}

	public Sprite ChangeSprite(ItemType type, int level)
	{
		Sprite sp = null;
		switch (type)
		{
			case ItemType.Iron:
				sp = ironSprites[level - 1];
				break;
			case ItemType.Stone:
				sp = stoneSprites[level - 1];
				break;
			case ItemType.Wood:
				sp = woodSprites[level - 1];
				break;
		}
		return sp;
	}

	public List<RaycastResult> GetSlotInfo()
	{
		PointerEventData pointer = new PointerEventData(EventSystem.current);
		pointer.position = Input.mousePosition;
		List<RaycastResult> raycastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointer, raycastResults);
		return raycastResults;
	}

	public bool CheckSlotIsNotEmpty(List<RaycastResult> raycastResults)
	{
		if (raycastResults.Count > 0)
		{
			if (raycastResults[1].gameObject.tag == "Item")
			{
				return true;
			}
		}
		return false;
	}

	public Transform GetSlotTransform(List<RaycastResult> raycastResults, Transform startTransform)
	{
		if (raycastResults.Count > 0)
		{
			if (raycastResults[1].gameObject.tag == "Slot")
			{
				return raycastResults[1].gameObject.transform;
			}
		}
		return startTransform;
	}

	public void AddScore()
	{
		score += 100;
		text.text = "Score: " + score;
	}
	#endregion
}
