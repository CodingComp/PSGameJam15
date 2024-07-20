using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightTest : MonoBehaviour
{
    private Player playerRef;
    
    public GameObject[] lightDetectors;

    public float health = 100.0f;

    public bool inLight = false;

    private Material mat;
    
    // Start is called before the first frame update
    void Start()
    {
        playerRef = GameObject.Find("Player").GetComponent<Player>();
        mat = GetComponent<Renderer>().material;
    }

    private void FixedUpdate()
    {
        if (inLight) mat.color = Color.green;
        else mat.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0.0f)
        {
            OnBecameInvisible();
            Destroy(gameObject);
        }
    }
    
    // Use These Functions To Detect Instead.
    private void OnBecameVisible()
    {
        print("Visible");
        playerRef.EnemyVisible(gameObject, this);
    }
    
    private void OnBecameInvisible()
    {
        print("Invisible");
        playerRef.EnemyLeftVisibility(gameObject);
    }

}
