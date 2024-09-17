using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggers : MonoBehaviour
{
    [Header("Camera config")]
    public GameObject camB;

    private GameManager _GameManager;


    [Header("Sound Effects")]
    public AudioSource hpCollectedSound;


    void Start(){
        _GameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    private void OnTriggerEnter(Collider other) {
        switch(other.gameObject.tag){
            case "CamTrigger":
                camB.SetActive(true);
            break;

            case "LifePoint":
                if (hpCollectedSound != null) {
                    hpCollectedSound.Play();
                }
                
                _GameManager.IncreaseHP(1);
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
