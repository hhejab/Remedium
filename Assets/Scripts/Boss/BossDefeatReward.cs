using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDefeatReward : MonoBehaviour
{
    [Header("Skill Point Reward")]
    public int skillPointsReward = 5;

    [Header("Skill XP Reward")]
    public int skillXPReward = 0;

    [Header("Combat Points Reward")]
    public int combatPointsReward = 1;

    [Header("Return Scene")]
    public string sceneToLoad = "Forest";
    public string spawnPointName = "BackFromDefeatingBoss";

    private bool rewardGiven = false;

    public void GiveRewardAndReturn()
    {
        if (rewardGiven) return;
        rewardGiven = true;

        PlayerSkillPointWallet skillWallet = FindFirstObjectByType<PlayerSkillPointWallet>();

        if (skillWallet != null)
        {
            if (skillPointsReward > 0)
                skillWallet.AddSkillPoints(skillPointsReward);

            if (skillXPReward > 0)
                skillWallet.AddSkillXP(skillXPReward);
        }

        PlayerCPWallet cpWallet = FindFirstObjectByType<PlayerCPWallet>();

        if (cpWallet != null && combatPointsReward > 0)
            cpWallet.AddCP(combatPointsReward);

        SceneSpawnManager.nextSpawnPointName = spawnPointName;
        SceneManager.LoadScene(sceneToLoad);
    }
}