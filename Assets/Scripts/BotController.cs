using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    [SerializeField] [Range(0, 3)] private float maxLinearSpeed = 2.0f;
    [SerializeField] [Range(0, 30)] private float maxAngularSpeed = 30.0f;

    private float acceleration = 1.0f;
    private float friction = 2.0f;

    private float linearSpeed = 0.0f;
    private float angularSpeed = 0.0f;

    private UDPSend sender;

    private GameObject bot;

    private float[] lidar = new float[720];

    void Start()
    {
        bot = this.gameObject;
        sender = bot.GetComponent<UDPSend>();
    }

    void Update() 
    {
        SendBotData();
    }

    void SendBotData() 
    {
        float x = bot.transform.position.x;
        float y = bot.transform.position.z;
        float theta = bot.transform.rotation.eulerAngles.y;
        string range = string.Join(",", lidar);
        string packet = "{\"x\":" + x + ",\"y\":" + y + ",\"theta\":" + theta + ",\"range\":[" + range + "]}";
        sender.SendData(packet);
    }

    void FixedUpdate()
    {
        UpdateLidar();
        UpdateControls();
        ApplyControls();
    }

    void UpdateControls() 
    {
        float af = acceleration * Time.deltaTime;
        float ff = friction * Time.deltaTime;

        // Computing Velocities
        if (Input.GetKey(KeyCode.W))
        {
            linearSpeed = (linearSpeed + af) > maxLinearSpeed ? maxLinearSpeed : linearSpeed + af;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            linearSpeed = (linearSpeed - af) < -maxLinearSpeed ? -maxLinearSpeed : linearSpeed - af;
        }
        else {
            if (linearSpeed > 0) linearSpeed = linearSpeed - ff < 0 ? 0 : linearSpeed - ff;
            else if (linearSpeed < 0) linearSpeed = linearSpeed + ff > 0 ? 0 : linearSpeed + ff;
        }

        if (Input.GetKey(KeyCode.A)) angularSpeed = -maxAngularSpeed;
        else if (Input.GetKey(KeyCode.D)) angularSpeed = maxAngularSpeed;
        else angularSpeed = 0;
    }

    void ApplyControls()
    {
        bot.transform.position += bot.transform.forward * linearSpeed * Time.deltaTime;
        bot.GetComponent<Rigidbody>().angularVelocity = Vector3.up * angularSpeed;
    }

    void UpdateLidar()
    {
        // raycast 720 rays from the bot and store the distance in the lidar array
        // lidar[0] is the distance of the ray that is directly in front of the bot

        for (int i = 0; i < 720; i++)
        {
            float angle = i * 0.5f;
            float x = Mathf.Sin(angle * Mathf.Deg2Rad);
            float y = Mathf.Cos(angle * Mathf.Deg2Rad);
            Vector3 direction = new Vector3(x, 0, y);
            RaycastHit hit;
            if (Physics.Raycast(bot.transform.position, direction, out hit, 100))
            {
                // draw ray debug
                Debug.DrawRay(bot.transform.position, direction * hit.distance, Color.yellow);
                lidar[i] = hit.distance;
            }
            else
            {
                lidar[i] = Mathf.Infinity;
            }
        }
    }
}
