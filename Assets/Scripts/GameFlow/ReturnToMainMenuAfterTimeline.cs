using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class ReturnToMainMenuAfterTimeline : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu_Final";

    private PlayableDirector director;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    private void OnEnable()
    {
        if (director != null)
            director.stopped += OnTimelineFinished;
    }

    private void OnDisable()
    {
        if (director != null)
            director.stopped -= OnTimelineFinished;
    }

    private void OnTimelineFinished(PlayableDirector obj)
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}