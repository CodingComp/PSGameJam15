using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ItemUI : MonoBehaviour
{
    private PlayerInventory _playerInventory;
    public ItemData itemData;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text itemCountTxt;

    private void Start()
    {
        _playerInventory = GameObject.Find("Player").GetComponent<PlayerInventory>();
        icon.sprite = itemData.icon;
    }

    private void OnEnable()
    {
        if (_playerInventory is null) return;
        itemCountTxt.text = _playerInventory.items[itemData].ToString();
    }
}
