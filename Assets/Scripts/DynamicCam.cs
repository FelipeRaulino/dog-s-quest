using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCam : MonoBehaviour
{
    [Header("Camera config")]
    public GameObject camB;

    private void OnTriggerEnter(Collider other) {
        switch(other.gameObject.tag){
            case "CamTrigger":
                camB.SetActive(true);
            break;
        }
    }

    private void OnTriggerExit(Collider other) {
        switch(other.gameObject.tag){
            case "CamTrigger":
                camB.SetActive(false);
            break;
        }
    }
}
