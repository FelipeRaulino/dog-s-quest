using UnityEngine;

public class MachadoController : MonoBehaviour
{
    private bool temMachado = false; // Verifica se o jogador pegou o machado
    private bool temEspada = false;
    private bool portao1 = false;
    private bool portao2 = false;
    public GameObject machadoNovo; // Referência ao machado que o jogador está segurando
    public GameObject espadaNova;

    private GameObject objetoMachado;
    private GameObject objetoEspada;

    void Update()
    {
        // Detecta a tecla de espaço para pegar ou trocar itens
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (portao1 && portao2)
            {
                AbrirPortao();
            }
            else
            {
                if (!temMachado && objetoMachado != null)
                {
                    PegarMachado();
                }
                else if (temMachado && objetoMachado != null)
                {
                    TrocarMachado();
                }

                if (!temEspada && objetoEspada != null)
                {
                    PegarEspada();
                }
                else if (temEspada && objetoEspada != null)
                {
                    TrocarEspada();
                }
            }
        }
    }

    // Função para pegar o machado
    void PegarMachado()
    {
        if (objetoMachado != null)
        {
            temMachado = true; // Marca que o jogador pegou o machado
            objetoMachado.SetActive(false); // Desativa o machado na cena
            Debug.Log("Machado pegado!");
        }
    }

    // Função para pegar a espada
    void PegarEspada()
    {
        if (objetoEspada != null)
        {
            temEspada = true; // Marca que o jogador pegou a espada
            objetoEspada.SetActive(false); // Desativa a espada na cena
            Debug.Log("Espada pegada!");
        }
    }

    // Função para trocar o machado de posição com outro objeto
    void TrocarMachado()
    {
        if (objetoMachado != null && machadoNovo != null)
        {
            Vector3 posicaoTemporaria = objetoMachado.transform.position;
            objetoMachado.transform.position = machadoNovo.transform.position;
            machadoNovo.transform.position = posicaoTemporaria;
            portao1 = true;
            Debug.Log("Machado trocado de posição!");
        }
    }

    // Função para trocar a espada de posição com outro objeto
    void TrocarEspada()
    {
        if (objetoEspada != null && espadaNova != null)
        {
            Vector3 posicaoTemporaria = objetoEspada.transform.position;
            objetoEspada.transform.position = espadaNova.transform.position;
            espadaNova.transform.position = posicaoTemporaria;
            portao2 = true;
            Debug.Log("Espada trocada de posição!");
        }
    }

    // Detecta quando o jogador entra em contato com objetos
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("pegarMachado"))
        {
            objetoMachado = other.gameObject;
            Debug.Log("Machado detectado!");
        }
        else if (other.CompareTag("pegarEspada"))
        {
            objetoEspada = other.gameObject;
            Debug.Log("Espada detectada!");
        }
    }
    void OnCollisionEnter(Collision collision)
{
    Debug.Log("Colidiu com: " + collision.gameObject.name);
}

    // Detecta quando o jogador sai de contato com os objetos
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("pegarMachado"))
        {
            objetoMachado = null;
            Debug.Log("Machado fora de alcance!");
        }
        else if (other.CompareTag("pegarEspada"))
        {
            objetoEspada = null;
            Debug.Log("Espada fora de alcance!");
        }
    }

    // Função para abrir o portão, rotacionando os filhos
    void AbrirPortao()
    {
        Transform porta1 = transform.Find("porta1boss");
        Transform porta2 = transform.Find("porta2boss");

        if (porta1 != null)
        {
            porta1.Rotate(0, 85, 0);
        }
        if (porta2 != null)
        {
            porta2.Rotate(0, -85, 0);
        }

        Debug.Log("Portão aberto!");
    }
}
