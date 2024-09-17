using UnityEngine;

public class AbrirPortaoController : MonoBehaviour
{
    private bool rampa = false;
    private bool grade = false;

    private GameObject ramp;
    private GameObject grid;

    // Referências para as câmeras
    public Camera mainCamera;    // A câmera principal (Main Camera)
    public Camera camera2;       // A segunda câmera

    // Referências para os sons
    public AudioClip somRampa;
    public AudioClip somGrade;
    private AudioSource audioSource;

    void Start(){
        // Encontra o objeto filho chamado "ramp"
        ramp = transform.Find("Ramp").gameObject;
        // Encontra o objeto filho chamado "grid"
        grid = transform.Find("Grid").gameObject;

        // Inicializa o AudioSource
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (rampa)
        {
            Vector3 currentRotation = ramp.transform.localEulerAngles;

            if (currentRotation.x > 180)
            {
                currentRotation.x -= 360;
            }

            currentRotation.x = Mathf.Lerp(currentRotation.x, 0f, Time.deltaTime * 2f);
            ramp.transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z);

            if (Mathf.Abs(currentRotation.x) < 0.1f)
            {
                rampa = false;
            }
        }

        if (grade)
        {
            Vector3 currentPosition = grid.transform.localPosition;
            currentPosition.y = Mathf.Lerp(currentPosition.y, 2.4f, Time.deltaTime * 2f);
            grid.transform.localPosition = currentPosition;

            if (Mathf.Abs(currentPosition.y - 2.4f) < 0.1f)
            {
                grade = false;
            }
        }
    }

    public void AbrirRampa()
    {
         // Definir o ponto inicial do áudio (começando em 0.5 segundos)
        audioSource.clip = somRampa;
        audioSource.time = 0.5f; // Começar a partir de 0.5 segundos
        audioSource.Play();
        rampa = true;
        MeshCollider meshCollider = ramp.GetComponent<MeshCollider>();
        meshCollider.enabled = false;

        mudarCamera();
        Invoke("voltarCamera", 2f);
    }

    public void AbrirGrade()
    {
        grade = true;
        BoxCollider meshCollider = grid.GetComponent<BoxCollider>();
        meshCollider.enabled = false;
        
        audioSource.PlayOneShot(somGrade);

        mudarCamera();
        Invoke("voltarCamera", 2f);
    }

    void mudarCamera(){
        mainCamera.enabled = false;
        camera2.enabled = true;

        // Desativar o AudioListener da mainCamera e ativar na camera2
        mainCamera.GetComponent<AudioListener>().enabled = false;
        camera2.GetComponent<AudioListener>().enabled = true;
    }
    
    void voltarCamera(){
        camera2.enabled = false;
        mainCamera.enabled = true;

        // Desativar o AudioListener da camera2 e ativar na mainCamera
        camera2.GetComponent<AudioListener>().enabled = false;
        mainCamera.GetComponent<AudioListener>().enabled = true;
        audioSource.Stop();
    }
}
