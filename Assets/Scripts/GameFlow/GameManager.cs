// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameProgress
{
    Start,
    TalkedToNatalie,
    SlimeBossKilled,
    VisitedBlacksmith,
    Level2Reached,
    BeholderCured,
    Finished
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int playerLevel = 1;
    public int combatPoints = 0;
    public GameProgress progress = GameProgress.Start;

    const string SAVE_KEY = "RemediumSave";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        playerLevel = 1;
        combatPoints = 0;
        progress = GameProgress.Start;
        Save();
        SceneManager.LoadScene("Village");
    }

    public void ContinueGame()
    {
        Load();
        SceneManager.LoadScene("Village");
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public void AddCP(int amount)
    {
        combatPoints += amount;

        if (combatPoints >= 10 && playerLevel < 2)
        {
            playerLevel = 2;
            progress = GameProgress.Level2Reached;
        }

        Save();
    }

    public void SetProgress(GameProgress newProgress)
    {
        progress = newProgress;
        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetString(SAVE_KEY, playerLevel + "|" + combatPoints + "|" + progress);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (!HasSave())
            return;

        string[] data = PlayerPrefs.GetString(SAVE_KEY).Split('|');

        playerLevel = int.Parse(data[0]);
        combatPoints = int.Parse(data[1]);
        progress = (GameProgress)System.Enum.Parse(typeof(GameProgress), data[2]);
    }
}