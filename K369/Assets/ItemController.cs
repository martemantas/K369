using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    [SerializeField] private Color color;
    [SerializeField] private Color originalColor;
    [SerializeField] private Material material;
    [SerializeField] private TMP_Text button;
    [SerializeField] private GameObject itemInfo;
    [SerializeField] private int price;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private GameObject ItemList;
    [SerializeField] private Button buyButton;
    [SerializeField] private List<ItemController> ItemsOfSameType;
    [SerializeField] private TMP_Text buyButtonText;
    [SerializeField] private GameObject checkMark;
    public bool isEquipped = false;
    public bool isBought = false;
    public int index;
    public void Start()
    {
        if (UserManager.Instance.CurrentUser.boughtItems.Contains(index))
        {
            isBought = true;
        }

        if (UserManager.Instance.CurrentUser.equipped.Contains(index))
        {
            isEquipped = true;
        }
        
        if (isBought)
        {
            button.text = "Equip";
            checkMark.SetActive(true);
        }
        else
        {
            button.text = price.ToString();
            checkMark.SetActive(false);            
        }
        if (isEquipped)
        {
            EquipItem();
        }
    }

    public void ClickAction()
    {
        if (isBought)
        {
            if (isEquipped)
            {
                UnequipItem();
            }
            else
            {
                EquipItem();
            }
        }
        else
        {
            DisplayInfo();
        }
    }

    public void DisplayInfo()
    {
        buyButton.onClick.RemoveAllListeners();
        itemInfo.SetActive(true);
        buyButton.onClick.AddListener(BuyItem);
        ItemList.SetActive(false);
        buyButtonText.text = price.ToString();
    }
    
    public void BuyItem()
    {
        if(UserManager.Instance.CurrentUser.Points < price)
        {
            buyButtonText.text = "Not enough points";
            return;
        }
        Debug.Log("Item bought!");
        button.text = "Equip";
        UserManager.Instance.CurrentUser.Points -= price;
        pointsText.text = UserManager.Instance.CurrentUser.Points.ToString();
        isBought = true;
        checkMark.SetActive(true);
        itemInfo.SetActive(false);
        ItemList.SetActive(true);
        UserManager.Instance.CurrentUser.boughtItems.Add(index);
        DatabaseManager.Instance.BuyItemForUser(UserManager.Instance.CurrentUser.Id,index);
    }

    //Kazkada gal turesim actual itema, kolkas pakeičiam spalvą
    public void EquipItem()
    {
        foreach (ItemController itemController in ItemsOfSameType)
        {
            if (itemController.isEquipped)
            {
                itemController.UnequipItem();
            }
        }
        material.color = color;
        button.text = "Unequip";
        isEquipped = true;
        UserManager.Instance.CurrentUser.equipped.Add(index);
        DatabaseManager.Instance.EquipItemForUser(UserManager.Instance.CurrentUser.Id,index);
    }
    
    public void UnequipItem()
    {
        material.color = originalColor;
        button.text = "Equip";
        isEquipped = false;
        UserManager.Instance.CurrentUser.equipped.Remove(index);
        DatabaseManager.Instance.UnequipItemForUser(UserManager.Instance.CurrentUser.Id, index);
    }

    public void OnDestroy()
    {
        material.color = originalColor;
    }
}
