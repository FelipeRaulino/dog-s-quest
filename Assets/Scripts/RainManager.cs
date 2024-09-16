using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RainManager : MonoBehaviour
{
    private GameManager _GameManager;

    public bool isRain;

    void Start()
    {
        _GameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    private void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "Player"){
            _GameManager.OnOffRain(isRain);
        }
    }

}
