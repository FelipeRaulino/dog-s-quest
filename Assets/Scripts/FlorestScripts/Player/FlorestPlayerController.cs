using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlorestPlayerController : MonoBehaviour
{
    private GameManager _GameManager;
    private CharacterController controller;
    private Animator animator;


    [Header("Movement Config")]
    public float movimentSpeed = 4f;
    private Vector3 direction;
    private bool isWalk;


    [Header("Attack Config")]
    public ParticleSystem fxAttack;
    public Transform hitBox;
    public LayerMask hitLayer;
    public Collider[] hitInfo;
    [Range(0.2f, 1f)]
    public float hitRange = 0.5f;
    private bool isAttack;
    private int amountDmg = 1;



    private float horizontal;
    private float vertical;



    [Header("Sound Effects")]
    public AudioSource getHitSound;
    public AudioSource deathSound;
    public AudioSource swordSound;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        _GameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
    }

    void Update()
    {
        if (_GameManager.gameState != GameState.GAMEPLAY) { return; }
        
        Inputs();

        MoveCharacter();

        UpdateAnimator();
    }

    #region MEUS MÉTODOS

    void Inputs() {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Fire1") && isAttack == false){
            Attack();
        }
    }

    void Attack(){
        if (swordSound != null) {
            swordSound.Play();
        }

        isAttack = true;
        animator.SetTrigger("Attack");
        fxAttack.Emit(1);

        hitInfo = Physics.OverlapSphere(hitBox.position, hitRange, hitLayer);

        foreach(Collider c in hitInfo) {
            c.SendMessage("GetHit", amountDmg, SendMessageOptions.DontRequireReceiver);
        }
    }

    void AttackIsDone(){
        isAttack = false;
    }

    void MoveCharacter(){
        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude > 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            isWalk = true;
        } else {
            isWalk = false;
        }

        controller.Move(movimentSpeed * Time.deltaTime * direction);
    }

    void UpdateAnimator(){
        animator.SetBool("isWalk", isWalk);
    }

    void GetHit(float amount){
        if (getHitSound != null) {
            getHitSound.Play();
        }

        _GameManager.DecreaseHP(amount);

        if (_GameManager.HP > 0){
            animator.SetTrigger("Hit");
        } else {
            if (deathSound != null) {
                deathSound.Play();
            }
            _GameManager.ChangeGameState(GameState.DIE);
            animator.SetTrigger("Die");
            _GameManager.GameOver();
        }
    }

    #endregion

    private void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "TakeDamage"){
            GetHit(0.5f);
        }
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(hitBox.position, hitRange);
    }
}
