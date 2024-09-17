using UnityEngine;

public class PlayerPortaController : MonoBehaviour
{
    private bool pertoValve1 = false;
    private bool pertoValve2 = false;

    public GameObject valve1;
    public GameObject valve2;
    public AbrirPortaoController portaController; // Referência para o script da porta

    void Update()
    {
        // Detecta a tecla de espaço para pegar ou trocar itens
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pertoValve1)
            {
                portaController.AbrirRampa();
            }
            else if (pertoValve2)
            {
                portaController.AbrirGrade();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == valve1)
        {
            pertoValve1 = true;
        }
        else if (collision.gameObject == valve2)
        {
            pertoValve2 = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == valve1)
        {
            pertoValve1 = false;
        }
        else if (collision.gameObject == valve2)
        {
            pertoValve2 = false;
        }
    }
}
