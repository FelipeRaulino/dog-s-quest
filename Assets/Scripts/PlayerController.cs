using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private CharacterController controller;
    private Animator animator;

    [Header("Moviment Config")]
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
    [SerializeField]
    private int amountDmg = 1;

    private float horizontal;
    private float vertical;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
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

    #endregion

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(hitBox.position, hitRange);
    }

}
