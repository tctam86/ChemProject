// Assets/Scripts/NPC/WaypointMover.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NPC))]
public class WaypointMover : MonoBehaviour
{
    public float moveSpeed = 2f;
    private NPC npc;
    private Coroutine moveCoroutine;

    void Awake()
    {
        npc = GetComponent<NPC>();
    }

    public void MoveToWaypoint(Transform waypoint, NPCDialogue nextDialogue)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveToPosition(waypoint, nextDialogue));
    }

    private IEnumerator MoveToPosition(Transform waypoint, NPCDialogue nextDialogue)
    {
        Vector3 targetPosition = waypoint.position;


        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        if (nextDialogue != null)
        {
            npc.dialogueData = nextDialogue;
            npc.Interact();
        }
    }
}
