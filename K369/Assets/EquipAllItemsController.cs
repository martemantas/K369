using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipAllItemsController : MonoBehaviour
{
    [SerializeField] private List<ItemController> allItems;

    public void EquipAllItems()
    {
        foreach (var item in allItems)
        {
            if (UserManager.Instance.CurrentUser.boughtItems.Contains(item.index))
            {
                item.EquipItem();
            }
        }
    }
}
