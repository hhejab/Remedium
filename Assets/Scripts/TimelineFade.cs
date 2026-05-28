using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimelineFade : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 2f;
    public string nextSceneName;

    private void Start()
    {
        FadeIn();
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(1, 0));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(0, 1, true));
    }

    IEnumerator Fade(float from, float to, bool loadNext = false)
    {
        Color c = fadeImage.color;
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;

        if (loadNext && !string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}