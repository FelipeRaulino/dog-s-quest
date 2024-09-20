using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MazeGameManager : MonoBehaviour
{
    public GameState gameState;

    public GameObject gameOverUI;

    [Header("Info Player")]
    public Transform player;
    public float HP;

    [Header("Slime IA")]
    public float slimeIdleWaitTime = 5f;
    public float slimeAlertWaitTime = 2f;
    public float slimeDistanceToAttack = 2.8f;
    public float giantDistanceToAttack = 1f;
    public float slimeAttackDelay = 1f;
    public float slimeLookAtSpeed = 1f;
    public float slimeStopDistance = 1f;

    public Text txtHP;
    public GameObject heartPrefab;
    public int percDrop = 75;
    

    public void increaseHP(float amount){
        HP += amount;
        txtHP.text = HP.ToString();
    }

    public void decreaseHP(float amount){
        HP -= amount;

        int HPConverted = System.Convert.ToInt32(HP);

        if (HP >= 0){
            txtHP.text = HPConverted.ToString();
        } else {
            txtHP.text = "0";
        }
    }

    public bool Perc(int p){
        int temp = Random.Range(0, 100);
        bool retorno = temp <= p ? true : false;
        return retorno;
    }

    public void ChangeGameState(GameState newState) {
        gameState = newState;
    }

    public void GameOver(){
        gameOverUI.SetActive(true);
    }

    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit(){
        Application.Quit();
    }

}
