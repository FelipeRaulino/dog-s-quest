using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggers : MonoBehaviour
{
    [Header("Camera config")]
    public GameObject camB;

    private GameManager _GameManager;

    void Start(){
        _GameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    private void OnTriggerEnter(Collider other) {
        switch(other.gameObject.tag){
            case "CamTrigger":
                camB.SetActive(true);
            break;

            case "Coletavel":
                _GameManager.setGems(1);
                Destroy(other.gameObject);
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
