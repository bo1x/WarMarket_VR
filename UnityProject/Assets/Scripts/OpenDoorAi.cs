using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorAi : MonoBehaviour
{
    public Animator _anim;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entraste" + " " + other.tag);
        if (other.tag != "IA") return;

        Debug.Log("IA");

        _anim.SetTrigger("Open");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "IA") return;

        _anim.SetTrigger("Close");
    }
}
