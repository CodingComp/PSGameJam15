using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flare : MonoBehaviour
{
    private GameObject flareLight;

    [SerializeField] private float lightDuration = 10.0f;
    [SerializeField] private float lifeTime = 120.0f;
    
    private void Start()
    {
        flareLight = transform.GetChild(0).gameObject;
        flareLight.transform.parent = null;

        StartCoroutine(LightDurationTimer());
        StartCoroutine(LifeTimeDurationTimer());
    }

    private void Update()
    {
        // Keeps the light always above the flare, making sure it is always fully illuminating the area
        flareLight.transform.position = transform.position + (Vector3.up * 0.5f);
    }
    
    private IEnumerator LightDurationTimer()
    {
        yield return new WaitForSeconds(lightDuration);
        flareLight.SetActive(false);
    }

    private IEnumerator LifeTimeDurationTimer()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
