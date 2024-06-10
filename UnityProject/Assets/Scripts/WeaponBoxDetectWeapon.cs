using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBoxDetectWeapon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Arma")
        {
            GameManager.Instance.WeaponEffectTouchBox.Play();
            Destroy(other.gameObject);
        }
    }
}
