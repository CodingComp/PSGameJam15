using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Enemy Class.
/// </summary>
public class Enemy : MonoBehaviour
{
    public EnemyManager em;
    private Material mat;
    
    public float health = 100.0f;
    public bool inLight = false;
    
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }
    
    void Update()
    {
        if (inLight)
        {
            mat.color = Color.green;
            
            // Damage Testing
            health -= 100.0f * Time.deltaTime;
            if (health <= 0.0f) em.EnemyDied(this);
        }
        else mat.color = Color.red;
    }
    
    private void OnBecameVisible()
    {
        em.EnemyVisible(this);
    }
    
    private void OnBecameInvisible()
    {
        em.EnemyLeftVisibility(this);
    }
}
