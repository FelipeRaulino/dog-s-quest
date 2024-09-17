using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class SlimeMazeIA : MonoBehaviour
{
    private MazeGameManager _GameManager;
    private Animator animator;
    public ParticleSystem fxBasicHit;
    
    public int HP = 3;
    private bool isDied;

    public enemyState state;

    //IA
    private bool isWalk;
    private bool isAlert;
    private bool isAttack;
    private bool isPlayerVisible;
    private NavMeshAgent agent;
    private int idWayPoint;
    private Vector3 destination;

    public Transform[] slimeWayPoints;

    [Header("Sound effects")]
    public AudioSource getHitSound;

    void Start()
    {
        _GameManager = FindObjectOfType(typeof(MazeGameManager)) as MazeGameManager;

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (slimeWayPoints.Length == 0)
        {
            Debug.LogError("Slime sem waypoints configurados.");
        }

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
    
    void GetHit(int amount){

        if (isDied == true) { return; }

        HP -= amount;

        if (getHitSound != null) {
            getHitSound.Play();
        }

        if (HP > 0) {
            ChangeState(enemyState.FURY);

            animator.SetTrigger("GetHit");

            fxBasicHit.Emit(20);
        } else {
            ChangeState(enemyState.DIE);
            animator.SetTrigger("Die");
            StartCoroutine("Died");
        }
    }

    void StateManager(){

        if (_GameManager.gameState == GameState.DIE && 
            (state == enemyState.FOLLOW || state == enemyState.FURY || state == enemyState.ALERT)){

            ChangeState(enemyState.IDLE);
            
        }

        switch(state){

            case enemyState.ALERT:
                LookAt();
                break;

            case enemyState.FOLLOW:
                LookAt();

                destination = _GameManager.player.position;
                agent.destination = destination;
                

                if (agent.remainingDistance <= agent.stoppingDistance){
                    Attack();
                }

                break;
            
            case enemyState.FURY:
                LookAt();

                destination = _GameManager.player.position;
                agent.destination = destination;
                
                if (agent.remainingDistance <= agent.stoppingDistance){
                    Attack();
                }

                break;
            
            case enemyState.PATROL:
                break;
        }
    }

    void ChangeState(enemyState newState){

        StopAllCoroutines();

        isAlert = false;
        isAttack = true;

        switch(newState){

            case enemyState.IDLE:

                agent.stoppingDistance = _GameManager.slimeStopDistance;
                destination = transform.position;
                agent.destination = destination;

                StartCoroutine("IDLE");

                break;
            
            case enemyState.ALERT:

                agent.stoppingDistance = _GameManager.slimeStopDistance;
                destination = transform.position;
                agent.destination = destination;
                isAlert = true;
                StartCoroutine("ALERT");

                break;
            
            case enemyState.PATROL:

                agent.stoppingDistance = _GameManager.slimeStopDistance;

                // Verifica se há waypoints específicos para esse Slime
                if (slimeWayPoints != null && slimeWayPoints.Length > 0)
                {
                    idWayPoint = Random.Range(0, slimeWayPoints.Length);
                    destination = slimeWayPoints[idWayPoint].position;
                    agent.destination = destination;
                }
                else
                {
                    Debug.LogWarning("Nenhum waypoint configurado para este slime.");
                    ChangeState(enemyState.IDLE); // Ou você pode colocar outro comportamento aqui
                }

                StartCoroutine("PATROL");

                break;
            
            case enemyState.FOLLOW:

                agent.stoppingDistance = _GameManager.slimeDistanceToAttack;
                StartCoroutine("FOLLOW");

                break;
            
            case enemyState.FURY:

                destination = transform.position;
                agent.stoppingDistance = _GameManager.slimeDistanceToAttack;
                agent.destination = destination;

                break;
            
            case enemyState.DIE:
                
                destination = transform.position;
                agent.destination = destination;
                
                break;
        }

        StartCoroutine("ATTACK");
        state = newState;

    }

    IEnumerator IDLE(){
        yield return new WaitForSeconds(_GameManager.slimeIdleWaitTime);

        Debug.Log("Vai tomar uma decisão");

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

    IEnumerator FOLLOW(){
        yield return new WaitUntil(() => !isPlayerVisible);

        print("perdi você");

        yield return new WaitForSeconds(_GameManager.slimeAlertWaitTime);

        StayStill(50);
    }

    IEnumerator ATTACK(){
        yield return new WaitForSeconds(_GameManager.slimeAttackDelay);
        isAttack = false;
    }

    IEnumerator Died(){
        isDied = true;
        yield return new WaitForSeconds(2.5f);

        if (_GameManager.Perc(_GameManager.percDrop)){
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
            Instantiate(_GameManager.heartPrefab, spawnPosition, _GameManager.heartPrefab.transform.rotation);
        }

        Destroy(this.gameObject);
    }

    public int Rand(){
        return Random.Range(0, 100);
    }

    private void StayStill(int yes){
        if (Rand() <= yes){
            ChangeState(enemyState.IDLE);
        } else {
            ChangeState(enemyState.PATROL);
        }
    }

    public void Attack(){
        if (!isAttack && isPlayerVisible == true){
            isAttack = true;
            animator.SetTrigger("Attack");
        }
    }

    void LookAt(){
        Vector3 lookDirection = (_GameManager.player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _GameManager.slimeLookAtSpeed * Time.deltaTime);
    }

    public void AttackIsDone(){
        StartCoroutine("ATTACK");
    }

    private void OnTriggerEnter(Collider other){
        if (_GameManager.gameState != GameState.GAMEPLAY) { return; }

        if (other.gameObject.tag == "Player"){
            isPlayerVisible = true;

            if (state == enemyState.IDLE || state == enemyState.PATROL){
                ChangeState(enemyState.ALERT);
            } else if (state == enemyState.FOLLOW){
                StopCoroutine("FOLLOW");
                ChangeState(enemyState.FOLLOW);
            }

        }   
    }

    private void OnTriggerExit(Collider other){
        if (other.gameObject.CompareTag("Player")){
            isPlayerVisible = false;
        }
    }

}
