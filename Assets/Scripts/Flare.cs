using System.Collections;
using UnityEngine;

public class Flare : MonoBehaviour
{
    [SerializeField] private float lightDuration = 10.0f;
    [SerializeField] private float lifeTime = 120.0f;
    private GameObject _flareLight;

    private void Start()
    {
        _flareLight = transform.GetChild(0).gameObject;
        _flareLight.transform.parent = null;

        StartCoroutine(LightDurationTimer());
        StartCoroutine(LifeTimeDurationTimer());
    }

    private void Update()
    {
        // Keeps the light always above the flare, making sure it is always fully illuminating the area
        _flareLight.transform.position = transform.position + Vector3.up * 0.5f;
    }

    private IEnumerator LightDurationTimer()
    {
        yield return new WaitForSeconds(lightDuration);
        _flareLight.SetActive(false);
    }

    private IEnumerator LifeTimeDurationTimer()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
