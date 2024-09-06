using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enemyState {
    IDLE, ALERT, PATROL, FOLLOW, FURY
}

public class GameManager : MonoBehaviour
{
    public Transform player;

    [Header("Slime IA")]
    public float slimeIdleWaitTime;
    public float slimeAlertWaitTime = 3f;
    public Transform[] slimeWayPoints;
    public float slimeStoppingDistance = 2.8f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
