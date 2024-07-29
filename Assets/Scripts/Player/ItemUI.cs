using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ItemUI : MonoBehaviour
{
    public ItemData itemData;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text itemCountTxt;
    private PlayerInventory _playerInventory;

    private void Awake()
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
