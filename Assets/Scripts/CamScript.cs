using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Referência ao personagem que a câmera seguirá
    Vector3 offset = new Vector3(20f, 10f, 0f);    // Distância e ângulo fixos da câmera em relação ao personagem
    public float smoothSpeed = 0.001f;  // Controla a suavidade da movimentação da câmera

    void LateUpdate()
    {
        // Posição desejada da câmera com base na posição do personagem e o offset
        Vector3 desiredPosition = target.position + offset;

        // Suaviza a movimentação da câmera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Mantém a rotação fixa para evitar que a câmera gire com o personagem
        transform.LookAt(target);
    }
}