using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum enemyState {
    IDLE, ALERT, PATROL, FOLLOW, FURY, DIE
}

public enum GameState {
    GAMEPLAY, DIE
}

public class GameManager : MonoBehaviour
{   
    public GameState gameState;
    public GameObject gameOverUI;

    
    [Header("Player Info")]
    public Transform player;
    public float HP;



    [Header("Slime IA")]
    public Transform[] slimeWayPoints;
    public float slimeIdleWaitTime;
    public float slimeAlertWaitTime = 3f;
    public float slimeStoppingDistance = 2.3f;
    public float slimeDistanceToAttack = 2.3f;
    public float slimeLookAtSpeed = 1f;
    public float slimeAttackDelay = 1f;



    [Header("Rain Manager")]
    public PostProcessVolume postB;
    public ParticleSystem rainParticle;
    private ParticleSystem.EmissionModule rainModule;
    public int rainRateOverTime;
    public int rainIncrement;
    public float rainIncrementDelay;



    [Header("Drop Item")]
    public GameObject heartPrefab;
    public int percDrop = 25;
    public Text txtHP;


    private void Start(){
        rainModule = rainParticle.emission;
        txtHP.text = HP.ToString();
    }

    public void OnOffRain(bool isRain){
        StopCoroutine("RainManager");
        StopCoroutine("PostBManager");
        StartCoroutine("RainManager", isRain);
        StartCoroutine("PostBManager", isRain);
    }

    IEnumerator RainManager(bool isRain){
        switch(isRain){
            case true:
                for(float r = rainModule.rateOverTime.constant; r < rainRateOverTime; r += rainIncrement){
                    rainModule.rateOverTime = r;
                    yield return new WaitForSeconds(rainIncrementDelay);
                }

                rainModule.rateOverTime = rainRateOverTime;

                break;
            case false:
                for(float r = rainModule.rateOverTime.constant; r > 0; r -= rainIncrement){
                    rainModule.rateOverTime = r;
                    yield return new WaitForSeconds(rainIncrementDelay);
                }

                rainModule.rateOverTime = rainRateOverTime;

                break;
        }
    }

    IEnumerator PostBManager(bool isRain){
        switch(isRain){
            case true:
                for(float w = postB.weight; w < 1; w += 1 * Time.deltaTime){
                    postB.weight = w;
                    yield return new WaitForEndOfFrame();
                }

                postB.weight = 1;

                break;
            case false:
                for(float w = postB.weight; w > 0; w -= 1 * Time.deltaTime){
                    postB.weight = w;
                    yield return new WaitForEndOfFrame();
                }

                postB.weight = 0;

                break;
        }
    }

    public void IncreaseHP(float amount){
        HP += amount;
        txtHP.text = HP.ToString();
    }

    public void DecreaseHP(float amount){
        HP -= amount;

        if (HP >= 0){
            txtHP.text = HP.ToString();
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
