using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health = 100.0f;
    [SerializeField] private float maxHealth = 100.0f;
    private Player _player;

    private Vector3 _respawnPosition;

    private void Start()
    {
        _player = GetComponent<Player>();
        _respawnPosition = transform.position;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) Death();
    }

    public void Heal(float healAmount)
    {
        health += healAmount;
        if (health > maxHealth) health = maxHealth;
    }

    private void Death()
    {
        GameObject droppedBackpack = Instantiate(_player.inventory.droppedBackpackPrefab);
        droppedBackpack.transform.position = transform.position;
        droppedBackpack.GetComponent<DroppedBackpack>().droppedItems = new Dictionary<ItemData, int>(_player.inventory.items);

        _player.inventory.DropAllItems();

        transform.position = _respawnPosition;
        health = maxHealth;
    }
}
