using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sMainMenuCameraController : MonoBehaviour
{

    Coroutine moving;

    public void MoveCameraToPosition(Transform target, float lerpTime)
    {
        if (moving == null)
        {
            moving = StartCoroutine(CameraMoveRoutine(target, lerpTime));
        }
    }

    IEnumerator CameraMoveRoutine(Transform target, float lerpTime)
    {
        float t = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        do
        {
            t += Time.deltaTime / lerpTime;
            transform.position = Vector3.Lerp(startPos, target.position, Lerp.SmootherStep(t));
            transform.rotation = Quaternion.Lerp(startRot, target.rotation, Lerp.SmootherStep(t));
            yield return null;
        } while (t <= 1f);
        moving = null;
    }
}
