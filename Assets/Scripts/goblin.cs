using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class GoblinController : MonoBehaviour
{
    public Transform jogador; // Referência ao jogador para calcular a distância
    private int vidas = 3;
    private bool morto = false;
    private GameObject espada; // Referência à espada do Goblin (com o Capsule Collider)
    private CapsuleCollider espadaCollider;
    public float velocidadeMovimento = 3f; // Velocidade de movimento do Goblin
    private Animator animator; // Referência ao Animator

    // Adicionando variável para cooldown
    public float cooldownColisao = 1.5f; // Tempo de cooldown entre colisões
    private bool podeCausarDano = true;  // Controla se a espada pode causar dano

    void Start()
    {
        // Obtém o componente Animator que está no mesmo GameObject
        animator = GetComponent<Animator>();

        // Encontra a espada do Goblin pelo nome na hierarquia
        Transform espadaTransform = transform.Find("MainSword");
        espadaCollider = espadaTransform.GetComponent<CapsuleCollider>();

        // Desativa o Collider da espada no início para evitar colisões fora do ataque
        espadaCollider.enabled = false;
    }

    void Update()
    {
        // Calcula a distância entre o Goblin e o jogador
        float distanciaParaJogador = Vector3.Distance(transform.position, jogador.position);

        // Define o valor da variável 'distancia' no Animator
        animator.SetFloat("distancia", distanciaParaJogador);

        // Se o Goblin está longe o suficiente do jogador, mova-se em direção a ele
        if (distanciaParaJogador > 2.5f && !morto && distanciaParaJogador < 15) // Parar perto do jogador
        {
            DesativarColliderEspada();
            // Calcula a direção para o jogador
            Vector3 direcao = (jogador.position - transform.position).normalized;

            // Faz o Goblin olhar na direção do jogador
            transform.LookAt(new Vector3(jogador.position.x, transform.position.y, jogador.position.z));

            // Move o Goblin na direção do jogador
            transform.Translate(direcao * velocidadeMovimento * Time.deltaTime, Space.World);
        }
        else if(!morto)
        {
            AtivarColliderEspada();
        }
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
        if (other.CompareTag("Player") && podeCausarDano) // Certifique-se de que o jogador tem a tag "Player"
        {
            PlayerController jogador = other.GetComponentInParent<PlayerController>();

            if (jogador != null)
            {
                jogador.TomarDano(1); // Causa 1 de dano ao jogador
                Debug.Log("O jogador foi atingido pela espada do Goblin!");

                // Inicia o cooldown para impedir múltiplas colisões consecutivas
                StartCoroutine(CooldownColisao());
            }
        }
    }

    // Corrotina que controla o cooldown de colisão
    IEnumerator CooldownColisao()
    {
        podeCausarDano = false; // Desativa a capacidade de causar dano
        yield return new WaitForSeconds(cooldownColisao); // Espera pelo tempo de cooldown
        podeCausarDano = true;  // Permite causar dano novamente
    }
    public void TomarDano(){
        vidas -= 1;
        if (vidas <= 0)
        {
            morrer();
        }else{
            tomarhit();
        }
    }
    void Suma(){
        Destroy(gameObject);
    }
    void morrer(){
        animator.SetBool("morto", true);
        Invoke("Suma",3f);
    }
    void tomarhit(){
        animator.SetTrigger("dano");
    }
}
