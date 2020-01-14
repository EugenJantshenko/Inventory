using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	#region Private Members
	private Canvas canvas;
	private InventoryController inventoryController;
	private Transform startTransform;
	private Transform endTransform;
	#endregion

	#region Public Members
	public ItemType itemType;
	public int level;
	public Image image;
	#endregion

	#region Private Methods
	private void Awake()
	{
		inventoryController = FindObjectOfType<InventoryController>();
		image = GetComponent<Image>();
		canvas = inventoryController.canvas;
	}

	private bool TryToMergeItems(List<RaycastResult> raycastResults)
	{
		Item secondItem = raycastResults[1].gameObject.gameObject.GetComponent<Item>();
		if (secondItem.itemType.Equals(itemType))
		{
			if (secondItem.level == level)
			{
				Destroy(secondItem.gameObject.gameObject);
				MergeItems();
				return true;
			}
		}
		return false;
	}

	private void MergeItems()
	{
		level++;
		int i = (int)itemType;
		image.sprite = inventoryController.ChangeSprite(itemType, level);
		if (level == 3)
		{
			Destroy(gameObject, 1.5f);
			inventoryController.AddScore();
		}
	}
	#endregion

	#region Public Methods
	public void Initialize()
	{
		startTransform = gameObject.transform.parent;
		endTransform = startTransform;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		startTransform = gameObject.transform.parent;
		endTransform = startTransform;
		transform.SetParent(canvas.transform, false);
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (inventoryController.CheckSlotIsNotEmpty(inventoryController.GetSlotInfo()))
		{
			if (!TryToMergeItems(inventoryController.GetSlotInfo()))
			{
				endTransform = startTransform;
				transform.position = endTransform.position;
				transform.SetParent(endTransform, false);
				RectTransform rt = transform.GetComponent<RectTransform>();
				rt.anchoredPosition = Vector3.zero;
				return;
			}
		}
		endTransform = inventoryController.GetSlotTransform(inventoryController.GetSlotInfo(), startTransform);
		transform.SetParent(endTransform, false);
		transform.position = endTransform.position;
		startTransform = transform.parent;
	}
	#endregion
}
