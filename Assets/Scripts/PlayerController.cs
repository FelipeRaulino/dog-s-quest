using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;

    [Header("Moviment Config")]
    public float movementSpeed = 4f;
    public float rotationSpeed = 720f;  // Velocidade de rotação (graus por segundo)
    private Vector3 direction;
    private bool isWalking = false;

    [Header("Attack Config")]
    public ParticleSystem fxAttack;
    public Transform hitBox;
    public LayerMask hitLayer;
    public Collider[] hitInfo;
    [Range(0.2f, 1f)]
    public float hitRange = 0.5f;
    private bool isAttacking = false;

    private bool isDefending = false;
    private int vidas = 5;

    [SerializeField]
    private int amountDmg = 1;

    private float horizontal;
    private float vertical;
    private bool morto = false;

    public GameObject espada; // Referência à espada do Goblin (com o Capsule Collider)
    private CapsuleCollider espadaCollider;
    public float cooldownColisao = 1.5f; // Tempo de cooldown entre colisões
    private bool podeCausarDano = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Certifique-se de que o Rigidbody não seja kinematic
        rb.isKinematic = false;
        rb.freezeRotation = true;  // Previne a rotação automática pelo Rigidbody
        // Encontra a espada do Goblin pelo nome na hierarquia
        Transform espadaTransform = transform.Find("root/pelvis/Weapon");
        espadaCollider = espadaTransform.GetComponent<CapsuleCollider>();

        // Desativa o Collider da espada no início para evitar colisões fora do ataque
        espadaCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!morto){
            Inputs();
            UpdateAnimator();
        }
    }

    void FixedUpdate()
    {
        if(!isAttacking && !isDefending){
            MoveCharacter();
        }
    }

    #region MEUS MÉTODOS

    void Inputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            animator.SetBool("idle", false);
            AtivarColliderEspada();
            Attack();
        }// Continua defendendo enquanto o botão direito estiver pressionado
        else if (Input.GetButton("Fire2") && !isAttacking)
        {
            animator.SetBool("idle", false);
            isDefending = true;
            animator.SetBool("defender", true);
        }
        else if(Input.GetButtonUp("Fire2"))
        {
            animator.SetBool("idle", false);
            isDefending = false;
            animator.SetBool("defender", false);
        }else{
            animator.SetBool("idle", true);
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttack");
        fxAttack.Emit(1);

        hitInfo = Physics.OverlapSphere(hitBox.position, hitRange, hitLayer);

        foreach (Collider c in hitInfo)
        {
            c.SendMessage("GetHit", amountDmg, SendMessageOptions.DontRequireReceiver);
        }
        Invoke("AttackIsDone", 0.5f);
    }

    void AttackIsDone()
    {
        isAttacking = false;
        DesativarColliderEspada();
    }
    public void AtivarColliderEspada()
    {
        espadaCollider.enabled = true;
    }

    public void DesativarColliderEspada()
    {
        espadaCollider.enabled = false;
    }

    // Detecta a colisão contínua entre a espada e o jogador usando OnTriggerStay
    void OnTriggerStay(Collider other)
    {
        Debug.Log("colidiu");
        if (other.CompareTag("enemy") && podeCausarDano) // Certifique-se de que o jogador tem a tag "Player"
        {
            GoblinController goblin = other.GetComponentInParent<GoblinController>();

            if (goblin != null)
            {
                goblin.TomarDano(); // Causa 1 de dano ao jogador
                Debug.Log("toma dano!");

                // Inicia o cooldown para impedir múltiplas colisões consecutivas
                StartCoroutine(CooldownColisao());
            }
        }
    }
    IEnumerator CooldownColisao()
    {
        podeCausarDano = false; // Desativa a capacidade de causar dano
        yield return new WaitForSeconds(cooldownColisao); // Espera pelo tempo de cooldown
        podeCausarDano = true;  // Permite causar dano novamente
    }

    void MoveCharacter()
    {
        // Pegue a referência à direção da câmera
        Transform cameraTransform = Camera.main.transform;

        // Direção para mover com base na câmera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Elimine qualquer movimento no eixo Y (altura), para manter o movimento no plano
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Combina os inputs de movimento com as direções da câmera
        Vector3 direction = (forward * vertical + right * horizontal).normalized;

        // Se o personagem estiver se movendo, rotacione e mova
        if (direction.magnitude > 0.1f)
        {
            isWalking = true;

            // Rotaciona o personagem suavemente em direção à direção desejada
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));

            // Move o personagem
            Vector3 moveDirection = direction * movementSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + moveDirection);
        }
        else
        {
            isWalking = false;
        }
    }

    void UpdateAnimator()
    {
        AttackIsDone();
        animator.SetBool("isWalk", isWalking);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(hitBox.position, hitRange);
    }
    public void TomarDano(int dano)
    {
        vidas -= dano;
        Debug.Log(vidas);

        if (vidas == 0)
        {
            Morrer();
        }else if(vidas > 0){
           TomarHitAnimacao();
        }
    }

    void Morrer()
    {
        animator.SetBool("morto", true);
        animator.SetTrigger("morreu");
        //animator.SetBool("", false);
        StartCoroutine(ReiniciarFaseAposDelay(5f)); // Inicia a corrotina para reiniciar a fase após 5 segundos
    }
    void Sairdohit(){
        // Define o bool 'tomarHit' como false após o intervalo
        animator.SetBool("tomarHit", false);
    }
    void TomarHitAnimacao()
    {
        // Define o bool 'tomarHit' como true para iniciar a animação de hit
        animator.SetBool("tomarHit", true);
        Invoke("Sairdohit",0.5f);
    }

    IEnumerator ReiniciarFaseAposDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Espera o tempo especificado (5 segundos)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarrega a cena atual
    }
}
