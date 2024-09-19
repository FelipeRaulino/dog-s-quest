using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI; 

public class ScriptCutscene : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject loadingScreen;
    public Button skipButton; 

    void Start()
    {
        loadingScreen.SetActive(true);
        videoPlayer.playOnAwake = false;
        StartCoroutine(LoadVideo());

        skipButton.gameObject.SetActive(false);
        Invoke("ShowSkipButton", 10);
        skipButton.onClick.AddListener(SkipVideo);
    }
    void ShowSkipButton()
    {
        skipButton.gameObject.SetActive(true);
        Invoke("HideSkipButton", 15);
    }

    void HideSkipButton()
    {
        skipButton.gameObject.SetActive(false);
    }

    IEnumerator LoadVideo()
    {
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        loadingScreen.SetActive(false);
        videoPlayer.Play();
        videoPlayer.loopPointReached += CheckVideoEnd;
    }

    void CheckVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("FlorestScene");
    }

    public void SkipVideo()
    {
        videoPlayer.Stop(); 
        CheckVideoEnd(videoPlayer);
    }
}
