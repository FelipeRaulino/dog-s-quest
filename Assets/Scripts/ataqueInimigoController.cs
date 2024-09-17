using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ataqueInimigoController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Verifique se colidiu com o inimigo
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();

            if (player != null)
            {
                player.TomarDano(1);
            }
        }
    }
}
