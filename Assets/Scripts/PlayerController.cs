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
    private bool podeAtacar = true; // Controle de cooldown de ataque

    private bool isDefending = false;
    private int vidas = 5;

    [SerializeField]
    private float horizontal;
    private float vertical;
    private bool morto = false;

    private CapsuleCollider espadaCollider;
    public float cooldownColisao = 1.5f; // Tempo de cooldown entre colisões
    public AudioClip somCaminhada;
    public AudioClip somDefesa;
    public AudioClip somEspada;
    public AudioClip somDano;
    private AudioSource audioSource;
    private bool podeTomarDano = true;

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
        // Inicializa o AudioSource
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!morto)
        {
            Inputs();
            UpdateAnimator();
            MoveCharacter();
        }
    }
    #region MEUS MÉTODOS

    void Inputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Fire1") && !isAttacking && podeAtacar)
        {
            isAttacking = true;
            podeAtacar = false;  // Bloqueia novos ataques até o cooldown acabar
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
        else if (Input.GetButtonUp("Fire2"))
        {
            animator.SetBool("idle", false);
            isDefending = false;
            animator.SetBool("defender", false);
        }
        else
        {
            animator.SetBool("idle", true);
        }
    }

    void Attack()
    {
        audioSource.clip = somEspada;
        audioSource.Play();
        animator.SetTrigger("isAttack");
        fxAttack.Emit(1);

        Invoke("AttackIsDone", 0.8f); // Tempo para terminar a animação do ataque
        Invoke("ResetAttackCooldown", 0.7f); // 3 segundos de cooldown de ataque
        Invoke("paraAudio",0.4f);
    }

    void AttackIsDone()
    {
        isAttacking = false;
        DesativarColliderEspada();
    }

    void ResetAttackCooldown()
    {
        podeAtacar = true; // Permite atacar novamente após o cooldown
    }

    public void AtivarColliderEspada()
    {
        espadaCollider.enabled = true;
    }

    public void DesativarColliderEspada()
    {
        //espadaCollider.enabled = false;
    }

    void MoveCharacter()
    {
        // Pegue a referência à direção da câmera
        if (Camera.main != null && Camera.main.enabled && !isDefending)
        {
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
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = somCaminhada;
                    audioSource.time = 0.5f;
                    audioSource.loop = true; // Deixa o som de caminhada em loop
                    audioSource.Play();
                }

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
                // Parar o som de caminhada quando parar de andar
                if (audioSource.isPlaying && audioSource.clip == somCaminhada)
                {
                    audioSource.Stop();
                }
            }
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
        if (isDefending)
        {
            // Emitir som de defesa
            audioSource.PlayOneShot(somDefesa);
        }
        else if (podeTomarDano)
        {
            podeTomarDano = false;
            vidas -= dano;
            audioSource.clip = somDano;
            audioSource.Play();

            if (vidas == 0)
            {
                Morrer();
            }
            else if (vidas > 0)
            {
                TomarHitAnimacao();
            }
            Invoke("imunidade", 1f);
            Invoke("paraAudio", 0.4f);
        }
    }

    void paraAudio()
    {
        audioSource.Stop();
    }

    void imunidade()
    {
        podeTomarDano = true;
    }

    void Morrer()
    {
        animator.SetBool("morto", true);
        morto = true;
        animator.SetTrigger("morreu");
        StartCoroutine(ReiniciarFaseAposDelay(3f)); // Inicia a corrotina para reiniciar a fase após 5 segundos
    }

    void Sairdohit()
    {
        // Define o bool 'tomarHit' como false após o intervalo
        animator.SetBool("tomarHit", false);
    }

    void TomarHitAnimacao()
    {
        // Define o bool 'tomarHit' como true para iniciar a animação de hit
        animator.SetBool("tomarHit", true);
        Invoke("Sairdohit", 0.5f);
    }

    IEnumerator ReiniciarFaseAposDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Espera o tempo especificado (5 segundos)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarrega a cena atual
    }
}