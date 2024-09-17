using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerControllerMaze : MonoBehaviour
{
    private MazeGameManager _GameManager;
    private GiantController giantController;
    private CharacterController controller;
    private Animator animator;

    [Header("Sound effects")]
    public AudioSource swordSound;
    public AudioSource hpCollectedSound;
    public AudioSource deathSound;
    public AudioSource getHitSound;

    [Header("Config Config")]
    public float movimentSpeed = 8f;
    public float turnSpeed = 90f;
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

    public Text txtHeartCount;


    // Start is called before the first frame update
    void Start()
    {   
        _GameManager = FindObjectOfType(typeof(MazeGameManager)) as MazeGameManager;
        giantController = FindObjectOfType(typeof(GiantController)) as GiantController;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        txtHeartCount.text = _GameManager.HP.ToString();
    }

    // Update is called once per frame
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
        isWalk = Mathf.Abs(vertical) > 0 || Mathf.Abs(horizontal) > 0;
        controller.Move(transform.forward * vertical * movimentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * horizontal * turnSpeed * Time.deltaTime);
    }

    void UpdateAnimator(){
        animator.SetBool("isWalk", isWalk);
    }

    void GetHit(float amount){
        if (getHitSound != null) {
            getHitSound  .Play();
        }

        _GameManager.decreaseHP(amount);

        txtHeartCount.text = _GameManager.HP.ToString();

        if (_GameManager.HP > 0){
            animator.SetTrigger("Hit");
        } else {
            if (deathSound != null) {
                deathSound  .Play();
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

        if (other.gameObject.tag == "LifePoint"){
            if (hpCollectedSound != null) {
                hpCollectedSound.Play();
            }
            _GameManager.increaseHP(1f);
            Destroy(other.gameObject);    
        }

        if (other.gameObject.tag == "PassportSensor" && giantController.HP <= 0){
            SceneManager.LoadScene("fase3-castelo");   
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(hitBox.position, hitRange);
    }

}
