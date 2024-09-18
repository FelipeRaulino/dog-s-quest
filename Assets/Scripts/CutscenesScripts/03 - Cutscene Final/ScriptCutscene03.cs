using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ScriptCutscene03 : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject loadingScreen;

    void Start()
    {
        loadingScreen.SetActive(true);
        videoPlayer.playOnAwake = false;
        StartCoroutine(LoadVideo());
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
        SceneManager.LoadScene("Menu");
    }
}
