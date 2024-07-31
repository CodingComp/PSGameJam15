using System.Collections;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("Flashlight")] 
    public GameObject flashlight;
    [SerializeField] private float batteryDrainRate;
    [SerializeField] private float rechargeWaitTime;

    [Header("Flare")] 
    [SerializeField] private GameObject flarePrefab;
    [SerializeField] private float throwForce;
    private int _flareCount = 3;
    private float _flashlightBattery = 100.0f;
    private Player _player;
    private bool _recharge;

    private void Start()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (_player.isBusy) return;

        // Flashlight
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_flashlightBattery <= 0)
            {
                flashlight.SetActive(false);
            }
            else
            {
                flashlight.SetActive(!flashlight.activeSelf);
                if (!flashlight.activeSelf) StartCoroutine(RechargeBattery());
            }
        }

        /*if (flashlight.activeSelf)
        {
            _flashlightBattery -= batteryDrainRate * Time.deltaTime;
            if (_flashlightBattery <= 0.0f)
            {
                flashlight.SetActive(false);
                StartCoroutine(RechargeBattery());
            }
        }
        else if (_recharge)
        {
            _flashlightBattery += batteryDrainRate * Time.deltaTime;
            if (_flashlightBattery >= 100.0f)
            {
                _recharge = false;
                _flashlightBattery = 100.0f;
            }
        }*/


        // Flare
        if (Input.GetKeyDown(KeyCode.Mouse1) && _flareCount != 0)
        {
            GameObject thrownFlare = Instantiate(flarePrefab, transform.position + _player.playerModel.transform.forward + new Vector3(0, 1, 0),
                new Quaternion());
            thrownFlare.GetComponent<Rigidbody>().AddForce(_player.playerModel.transform.forward * throwForce, ForceMode.Impulse);
            _flareCount--;
        }
    }

    private IEnumerator RechargeBattery()
    {
        _recharge = false;
        yield return new WaitForSeconds(rechargeWaitTime);
        _recharge = !flashlight.activeSelf;
    }
}
