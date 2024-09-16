using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class GiantController : MonoBehaviour
{   
    
    private MazeGameManager _GameManager;
    private Animator animator;
    private NavMeshAgent agent;
    public enemyState state;
    private Vector3 destination;
    private bool isAttack;
    private bool isPlayerVisible;
    private bool isWalk;
    public int HP = 3;
    private bool isDied;
    
    void Start()
    {
        _GameManager = FindObjectOfType(typeof(MazeGameManager)) as MazeGameManager;
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
    }

    void StateManager(){
        if (_GameManager.gameState == GameState.DIE && state == enemyState.FURY){
            ChangeState(enemyState.IDLE);
        }

        switch(state){
            case enemyState.FURY:
                LookAt();

                destination = _GameManager.player.position;
                agent.destination = destination;
                

                if (agent.remainingDistance <= agent.stoppingDistance){
                    Attack();
                }

                break;
        }

    }

    void ChangeState(enemyState newState){
        isAttack = true;

        switch(newState){
            case enemyState.FURY:

                destination = transform.position;
                agent.stoppingDistance = 2.5f;
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

    IEnumerator ATTACK(){
        yield return new WaitForSeconds(3f);
        isAttack = false;
    }

    IEnumerator Died(){
        isDied = true;
        yield return new WaitForSeconds(5f);

        /* if (_GameManager.Perc(_GameManager.percDrop)){
            Instantiate(_GameManager.gemPrefab, transform.position, _GameManager.gemPrefab.transform.rotation);
        } */

        Destroy(this.gameObject);
    }

    void LookAt(){
        Vector3 lookDirection = (_GameManager.player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _GameManager.slimeLookAtSpeed * Time.deltaTime);
    }

    public void Attack(){
        if (!isAttack && isPlayerVisible == true){
            isAttack = true;
            animator.SetTrigger("Attack");
        }

        StartCoroutine("ATTACK");
    }

    void GetHit(int amount){

        if (isDied == true) { return; }

        HP -= amount;

        if (HP > 0) {
            ChangeState(enemyState.FURY);

            animator.SetTrigger("GetHit");
        } else {
            ChangeState(enemyState.DIE);
            animator.SetTrigger("Die");
            StartCoroutine("Died");
        }
    }

    private void OnTriggerEnter(Collider other){
        if (_GameManager.gameState != GameState.GAMEPLAY) { return; }

        if (other.gameObject.tag == "Player"){
            isPlayerVisible = true;
            ChangeState(enemyState.FURY);
        }   
    }

    private void OnTriggerExit(Collider other){
        if (other.gameObject.CompareTag("Player")){
            isPlayerVisible = false;
        }
    }

}
