using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum NPCState { Default, Idle, Wander, Talk }
    public NPCState currentState = NPCState.Wander;
    private NPCState defaultstate;

    public NPC_Wander wander;
    public NPC_Talk talk;

    void Start()
    {
        defaultstate = currentState;
        SwitchState(currentState);
        
    }

    private void SwitchState(NPCState newState)
    {
        currentState = newState;

        wander.enabled = (currentState == NPCState.Wander);
        talk.enabled = (currentState == NPCState.Talk);
    }

    private void OnTrigerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(NPCState.Talk);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(defaultstate);
        }
    }

}
