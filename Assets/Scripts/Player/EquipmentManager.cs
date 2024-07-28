using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
     private Player _player;
     
     [Header("Flashlight")]
     public GameObject flashlight;
     [SerializeField] private float batteryDrainRate;
     [SerializeField] private float rechargeWaitTime;
     private float flashlightBattery = 100.0f;
     private bool recharge = false;
     
     [Header("Flare")]
     [SerializeField] private GameObject flarePrefab;
     [SerializeField] private float throwForce;
     private int flareCount = 3;
     
     private void Start()
     {
          _player = GetComponent<Player>();
     }

     private void Update()
     {
          // Flashlight
          if (Input.GetKeyDown(KeyCode.F))
          {
               if (flashlightBattery <= 0)
               {
                    flashlight.SetActive(false);
               }
               else
               {
                    flashlight.SetActive(!flashlight.activeSelf);
                    if (!flashlight.activeSelf) StartCoroutine(RechargeBattery());     
               }
          }
          
          if (flashlight.activeSelf)
          {
               flashlightBattery -= batteryDrainRate * Time.deltaTime;
               if (flashlightBattery <= 0.0f)
               {
                    flashlight.SetActive(false);
                    StartCoroutine(RechargeBattery());
               }
          }
          else if (recharge)
          {
               flashlightBattery += batteryDrainRate * Time.deltaTime;
               if (flashlightBattery >= 100.0f)
               {
                    recharge = false;
                    flashlightBattery = 100.0f;
               }
          }
          
          
          // Flare
          if (Input.GetKeyDown(KeyCode.Mouse1) && flareCount != 0)
          {
               GameObject thrownFlare = Instantiate(flarePrefab, transform.position + _player.playerModel.transform.forward + new Vector3(0,1,0), new Quaternion());
               thrownFlare.GetComponent<Rigidbody>().AddForce(_player.playerModel.transform.forward * throwForce, ForceMode.Impulse);
               flareCount--;
          }
     }

     private IEnumerator RechargeBattery()
     {
          recharge = false;
          yield return new WaitForSeconds(rechargeWaitTime);
          recharge = !flashlight.activeSelf;
     }
}
