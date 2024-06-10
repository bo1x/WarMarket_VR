using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWeaponOnTouchingGround : MonoBehaviour
{
    [SerializeField] ParticleSystem PS;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Arma")
        {
            var go = Instantiate(PS, other.gameObject.transform.position,Quaternion.identity);
            go.Emit(30);
            
            //A lo mejor deberiamos poner que si se destruye cualquier arma el cliente se enfada
            Debug.Log("TO DO LLAMAR AL GAME MANAGER PARA PONER QUE EL CLIENTE ESTA ENFADADO");
            Destroy(other.gameObject);
        }
    }
}
