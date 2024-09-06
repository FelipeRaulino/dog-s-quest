using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SlimeIA : MonoBehaviour
{
    private GameManager _GameManager;

    private Animator animator;
    public ParticleSystem fxBasicHit;
    public int HP = 3;

    private bool isDied;

    public enemyState state;

    //IA
    private bool isWalk;
    private bool isAlert;
    private bool isPlayerVisible;
    private NavMeshAgent agent;
    private int idWayPoint;
    private Vector3 destination;

    void Start()
    {
        _GameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ChangeState(state);
    }

    void Update()
    {
        StateManager();

        if (agent.desiredVelocity.magnitude >= 0.1f){
            isWalk = true;
        } else {
            isWalk = false;
        }

        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isAlert", isAlert);
    }

    IEnumerator Died(){
        isDied = true;
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }

    void GetHit(int amount){

        if (isDied == true) { return; }

        HP -= amount;

        if (HP > 0) {
            ChangeState(enemyState.FURY);
            animator.SetTrigger("GetHit");
            fxBasicHit.Emit(20);
        } else {
            isDied = true;
            animator.SetTrigger("Die");
            StartCoroutine("Died");
        }
    }

    private void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Player") && (state == enemyState.IDLE || state == enemyState.PATROL)){
            isPlayerVisible = true;
            ChangeState(enemyState.ALERT);
        }
    }

    private void OnTriggerExit(Collider other){
        if (other.gameObject.CompareTag("Player")){
            isPlayerVisible = false;
        }
    }

    void StateManager(){
        switch(state){
            case enemyState.FOLLOW:
                destination = _GameManager.player.position;
                agent.destination = destination;
                break;
            
            case enemyState.FURY:
                destination = _GameManager.player.position;
                agent.destination = destination;
                break;
            
            case enemyState.PATROL:
                break;
        }
    }

    void ChangeState(enemyState newState){

        StopAllCoroutines();
        state = newState;

        switch(state){
            case enemyState.IDLE:
                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;
                StartCoroutine("IDLE");
                break;
            
            case enemyState.ALERT:
                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;
                isAlert = true;
                StartCoroutine("ALERT");
                break;
            
            case enemyState.PATROL:
                agent.stoppingDistance = 0;
                idWayPoint = Random.Range(0, _GameManager.slimeWayPoints.Length);
                destination = _GameManager.slimeWayPoints[idWayPoint].position;
                agent.destination = destination;
                StartCoroutine("PATROL");
                break;
            
            case enemyState.FOLLOW:
                agent.stoppingDistance = _GameManager.slimeStoppingDistance;
                break;
            
            case enemyState.FURY:
                destination = transform.position;
                agent.stoppingDistance = _GameManager.slimeStoppingDistance;
                agent.destination = destination;
                break;
        }
    }

    IEnumerator IDLE(){
        yield return new WaitForSeconds(_GameManager.slimeIdleWaitTime);
        StayStill(50);
    }

    IEnumerator PATROL(){
        yield return new WaitUntil(() => agent.remainingDistance <= 0);
        StayStill(30);
    }

    IEnumerator ALERT(){
        yield return new WaitForSeconds(_GameManager.slimeAlertWaitTime);
        if (isPlayerVisible == true){
            ChangeState(enemyState.FOLLOW);
        } else {
            StayStill(10);
        }

    }

    private void StayStill(int yes){
        if (Rand() <= yes){
            ChangeState(enemyState.IDLE);
        } else {
            ChangeState(enemyState.PATROL);
        }
    }

    public int Rand(){
        return Random.Range(0, 100);
    }

}
