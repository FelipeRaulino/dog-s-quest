using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspadaController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("enemy")) // Verifique se colidiu com o inimigo
        {
            inimigoVida inimigo = other.GetComponentInParent<inimigoVida>();

            if (inimigo != null)
            {
                inimigo.TomarDano(); // Causa dano ao inimigo
            }
        }
    }
}
