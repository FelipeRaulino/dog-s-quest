using UnityEngine;
using System.Collections.Generic;
using System;

public class CameraTransparency : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public LayerMask obstacleLayer; // Layer dos objetos que podem bloquear a visão
    public string groundTag = "Ground"; // A tag para identificar o chão e outros objetos a serem ignorados
    public string outsideWallTag = "OutsideWall"; // A tag para identificar as paredes externas
    public String enemyTag = "enemy";

    private List<Renderer> currentRenderers = new List<Renderer>(); // Lista dos renderizadores atuais
    private List<Material[]> originalMaterials = new List<Material[]>(); // Lista dos materiais originais

    void Update()
    {
        // Limpa as listas para começar a detectar os novos objetos
        ClearTransparency();

        // Lança um Raycast da câmera até o jogador e detecta todos os objetos no caminho
        Vector3 direction = player.position - transform.position;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, direction.magnitude, obstacleLayer);

        foreach (RaycastHit hit in hits)
        {
            // Verifica se o objeto tem a tag de chão para ignorá-lo
            if (hit.collider.CompareTag(groundTag) || hit.collider.CompareTag(enemyTag))
                continue;


            Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Armazena o material original
                currentRenderers.Add(renderer);
                originalMaterials.Add(renderer.materials);

                // Verifica se o objeto é uma parede externa e o torna completamente transparente
                if (hit.collider.CompareTag(outsideWallTag))
                {
                    foreach (Material mat in renderer.materials)
                    {
                        ChangeMaterialToCompletelyTransparent(mat);
                    }
                }
                else
                {
                    // Faz a transição para parcialmente transparente para outros objetos
                    foreach (Material mat in renderer.materials)
                    {
                        ChangeMaterialToPartiallyTransparent(mat);
                    }
                }
            }
        }
    }

    // Restaura os materiais originais e limpa as listas
    private void ClearTransparency()
    {
        for (int i = 0; i < currentRenderers.Count; i++)
        {
            if (currentRenderers[i] != null)
            {
                currentRenderers[i].materials = originalMaterials[i];
            }
        }
        currentRenderers.Clear();
        originalMaterials.Clear();
    }

    // Função para alterar o material para parcialmente transparente
    private void ChangeMaterialToPartiallyTransparent(Material material)
    {
        material.SetFloat("_Mode", 2); // Transparente
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        Color color = material.color;
        color.a = 0.3f; // Transparência parcial
        material.color = color;
    }

    // Função para alterar o material para completamente transparente
    private void ChangeMaterialToCompletelyTransparent(Material material)
    {
        material.SetFloat("_Mode", 2); // Transparente
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        Color color = material.color;
        color.a = 0f; // Transparente
        material.color = color;
    }
}
