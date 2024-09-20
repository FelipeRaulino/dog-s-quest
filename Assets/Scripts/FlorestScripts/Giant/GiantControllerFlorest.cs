using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


public class GiantControllerFlorest : MonoBehaviour
{   
    
    private GameManager _GameManager;
    private Animator animator;
    private NavMeshAgent agent;
    public enemyState state;
    private Vector3 destination;
    private bool isAttack;
    private bool isPlayerVisible;
    private bool isWalk;
    public int HP = 3;
    private bool isDied;



    [Header("Sound effects")]
    public AudioSource getHitSound;
    public AudioSource missionCompleted;
    
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
    }

    void StateManager(){
        if (_GameManager.gameState == GameState.DIE && state == enemyState.FURY){
            ChangeState(enemyState.IDLE);
        }

        switch(state){
            case enemyState.FURY:
                LookAt();

                if (agent.isOnNavMesh && agent.isActiveAndEnabled) {
                    destination = _GameManager.player.position;
                    agent.destination = destination;
                    

                    if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance){
                        Attack();
                    }
                } else {
                    Debug.LogWarning("NavMeshAgent não está no NavMesh ou não está ativo.");
                }

                break;
        }

    }

    void ChangeState(enemyState newState){
        isAttack = true;

        // Verifica se o NavMeshAgent está ativo e no NavMesh antes de definir o destino
        if (agent.isOnNavMesh && agent.enabled)
        {
            switch (newState)
            {
                case enemyState.FURY:
                    destination = transform.position;
                    agent.stoppingDistance = 2.3f;
                    agent.SetDestination(destination);
                    break;

                case enemyState.DIE:
                    destination = transform.position;
                    agent.SetDestination(destination);
                    break;
            }

            StartCoroutine("ATTACK");
            state = newState;
        }
        else
        {
            Debug.LogWarning("NavMeshAgent não está no NavMesh ou não está ativo.");
        }

    }

    IEnumerator ATTACK(){
        yield return new WaitForSeconds(3f);
        isAttack = false;
    }

    IEnumerator Died(){
        isDied = true;
        
        if (missionCompleted != null) {
            missionCompleted.Play();
        }

        yield return new WaitForSeconds(4f);
        
        SceneManager.LoadScene("CutsceneFase1"); 

        Destroy(this.gameObject);
    }

    void LookAt(){
        Vector3 lookDirection = (_GameManager.player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _GameManager.slimeLookAtSpeed * Time.deltaTime);
    }

    public void Attack(){
        // Verifica se o inimigo está morto antes de permitir o ataque
        if (isDied || isAttack || !isPlayerVisible) { return; }

        isAttack = true;
        animator.SetTrigger("Attack");
    }

    public void AttackIsDone(){
        StartCoroutine("ATTACK");
    }

    void GetHit(int amount){

        if (isDied == true) { return; }

        HP -= amount;

        if (getHitSound != null) {
            getHitSound.Play();
        }

        if (HP > 0) {
            animator.CrossFade("gethit", 0.1f);

            isAttack = false;
        } else {
            ChangeState(enemyState.DIE);
            
            animator.CrossFade("gethit", 0.1f);
            animator.CrossFade("death", 0.1f); 
            
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
