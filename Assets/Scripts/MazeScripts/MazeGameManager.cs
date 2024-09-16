using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class MazeGameManager : MonoBehaviour
{
    public GameState gameState;

    [Header("Info Player")]
    public Transform player;
    /* private int gems = 0; */

    /* [Header("UI")]
    public Text txtGem; */


    [Header("Slime IA")]
    public float slimeIdleWaitTime = 5f;
    public Transform[] slimeWayPoints;
    public Transform[] slimeWayPoints2;
    public float slimeAlertWaitTime = 2f;
    public float slimeDistanceToAttack = 2.8f;
    public float giantDistanceToAttack = 1f;
    public float slimeAttackDelay = 1f;
    public float slimeLookAtSpeed = 1f;
    public float slimeStopDistance = 1f;

    public void ChangeGameState(GameState newState) {
        gameState = newState;
    }
}
