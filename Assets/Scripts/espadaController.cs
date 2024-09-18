using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspadaController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Exibe o nome do objeto que colidiu

        if (other.CompareTag("enemy")) // Verifique se colidiu com o inimigo
        {
            bossVida inimigo = other.GetComponentInParent<bossVida>();

            if (inimigo != null)
            {
                inimigo.TomarDano(); // Causa dano ao inimigo
            }else{
                inimigoVida inimigoMenor = other.GetComponent<inimigoVida>();
                if (inimigoMenor != null)
                {
                    inimigoMenor.TomarDano(); // Causa dano ao inimigo
                }
            }
        }
    }
}
