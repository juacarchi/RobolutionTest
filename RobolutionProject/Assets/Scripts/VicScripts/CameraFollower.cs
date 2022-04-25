using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Set here the transform of the player.")]
    public Transform target;
    [Header("Camera Settings")]
    [Tooltip("Speed of the camera movement.")]
    public float smoothSpeed = 0.1f;
    [Tooltip("Relative camera position.")]
    public Vector3 offsetCamera;

    public Vector3 offsetPlayer;

    //An posibility for stop the camera
    [Tooltip("Attach here the floor for calculate the zone of stop the camera")]
    [SerializeField]
    Collider floor;
    [Tooltip("Percent of stop zone")]
    [Range (0f,1f)]
    public float percentageStopZone = 0f;
    [SerializeField, Tooltip("Dont touch this value, only for check")]
    private float valueStopZone;
  
    private void Start()
    {
        CalculateFloor(floor);
    }
    public void CalculateFloor(Collider newfloor)
    {
        
        floor = newfloor;
        float mapX = (floor.bounds.size.x) / 2;
        float quantityRest = mapX * percentageStopZone;
        valueStopZone = mapX - quantityRest;
        
    }


    void FixedUpdate()
    { 

        if (target.position.x<valueStopZone && target.position.x>-valueStopZone)
        {
            Vector3 playerOffsestplus = target.position + offsetPlayer; 
            Vector3 desiredPosition = playerOffsestplus + offsetCamera;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            gameObject.transform.position = smoothedPosition;
            transform.LookAt(playerOffsestplus);
        }
        else if(target.position.x<=-valueStopZone||target.position.x>=valueStopZone)
        {
            float leftOrRight = valueStopZone;
            if (target.position.x <= -valueStopZone) leftOrRight *= -1;
            Vector3 stopPosition = new Vector3(leftOrRight, target.position.y, target.position.z)+offsetPlayer; 
            Vector3 desiredPosition = stopPosition + offsetCamera;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            gameObject.transform.position = smoothedPosition;

            transform.LookAt(stopPosition);
        }
        
        
    }
}
