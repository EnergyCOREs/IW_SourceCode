using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Этот скрипт писал не я
/// </summary>
public class IsometricCameraController : MonoBehaviour
{
    public Transform FollowTarget;
    CharacterController targetCharacterController;
    public float Smoothing = 5f;
    public float CameraDistOffset = 0.02f;

    public Vector3 targetOffset;
    Vector3 targetCamPos;
    Vector3 tempVelocity;
    public Camera cam;
    void Start()
    {
        targetCharacterController = FollowTarget.GetComponent<CharacterController>();
//        cam.depthTextureMode = DepthTextureMode.None;
        //оффсет получается очень тупо, можно переделать
        // targetOffset = transform.position - FollowTarget.position;
    }


    // если есть чар контроллер то двигаем камеру по оси велосити ,  иначе просто мягко двигаем камеру к обьекту с оффсетом
    void LateUpdate()
    {
        targetCamPos = FollowTarget.position + targetOffset;
        if (targetCharacterController)
        {
            tempVelocity = Vector3.Lerp(tempVelocity,  targetCharacterController.velocity, Time.deltaTime);
            tempVelocity.y = 0;
            targetCamPos = targetCamPos + (tempVelocity * (tempVelocity.magnitude * CameraDistOffset));
        }
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Smoothing * Time.deltaTime);
    }
}
