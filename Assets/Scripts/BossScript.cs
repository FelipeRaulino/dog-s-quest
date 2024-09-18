using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class BossController : MonoBehaviour
{
    private Transform jogador; // Referência ao jogador para calcular a distância
    private bool morto = false;
    private CapsuleCollider machadoCollider;
    public float velocidadeMovimento = 5f; // Velocidade de movimento do Goblin
    private Animator animator; // Referência ao Animator
    private bool podeAndar = false;
    public AudioClip somCaminhada;
    public GameObject machado;
    public AudioClip somEspada; 
    private AudioSource audioSource;

    void Start()
    {
        // Obtém o componente Animator que está no mesmo GameObject
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        // Encontra o jogador pelo tag "Player" e pega o componente Transform
        GameObject playerObject = GameObject.FindWithTag("Player");

        // Verifica se o jogador foi encontrado
        if (playerObject != null)
        {
            jogador = playerObject.transform; // Atribui o Transform do jogador
        }

        // Encontra a espada do Goblin pelo nome na hierarquia
        Transform machadoTransform = machado.transform;
        machadoCollider = machadoTransform.GetComponent<CapsuleCollider>();

        // Desativa o Collider da espada no início para evitar colisões fora do ataque
        machadoCollider.enabled = false;
    }
    void Update()
    {
        morto = animator.GetBool("morto");
        podeAndar = animator.GetBool("podeAndar");
        // Calcula a distância entre o Goblin e o jogador
        float distanciaParaJogador = Vector3.Distance(transform.position, jogador.position);

        // Define o valor da variável 'distancia' no Animator
        animator.SetFloat("distancia", distanciaParaJogador);

        // Se o Goblin está longe o suficiente do jogador, mova-se em direção a ele
        if (distanciaParaJogador > 4f && !morto && podeAndar) // Parar perto do jogador
        {
            // Calcula a direção para o jogador
            Vector3 direcao = (jogador.position - transform.position).normalized;

            // Faz o Goblin olhar na direção do jogador
            transform.LookAt(new Vector3(jogador.position.x, transform.position.y, jogador.position.z));

            // Move o Goblin na direção do jogador
            transform.Translate(direcao * velocidadeMovimento * Time.deltaTime, Space.World);
            if (!audioSource.isPlaying)
                {
                    audioSource.clip = somCaminhada;
                    audioSource.time = 0.5f;
                    audioSource.loop = true; // Deixa o som de caminhada em loop
                    audioSource.Play();
                }
        }
        else if(distanciaParaJogador < 4f && !morto)
        {
            audioSource.Stop();
            audioSource.clip = somEspada;
            audioSource.Play();
            Invoke("paraAudio",0.4f);
            Invoke("AtivarColliderMachado", 0.2f);
            Invoke("DesativarColliderMachado", 0.8f);
        }
    }
    public void AtivarColliderMachado()
    {
        machadoCollider.enabled = true;
    }
    void paraAudio()
    {
        audioSource.Stop();
    }

    public void DesativarColliderMachado()
    {
        machadoCollider.enabled = false;
    }
}