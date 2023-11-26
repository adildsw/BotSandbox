using UnityEngine;

public class BotFollower : MonoBehaviour
{
    public GameObject bot;
    public float distance = 5f;
    public float elevationAngle = 30f;

    void LateUpdate()
    {
        if (bot != null)
        {
            Vector3 behindPosition = bot.transform.position - bot.transform.forward * distance;
            behindPosition.y += distance * Mathf.Tan(elevationAngle * Mathf.Deg2Rad);
            transform.position = behindPosition;
            transform.LookAt(bot.transform.position);
        }
    }
}