using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    private enum Mode {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraBackwardInverted,
    }
    [SerializeField]private Mode mode;
    [SerializeField] private Camera FollowedCamera;
    void Start() {
        if(FollowedCamera == null) {
            GameObject cameraObj = GameObject.FindWithTag("MainCamera");
            if (cameraObj != null) {
                FollowedCamera = cameraObj.GetComponent<Camera>();
            } else {
                Debug.LogError("can not find any camera!");
            }
        }
    }
    private void LateUpdate() {
        switch (mode) {
            case Mode.LookAt:
                transform.LookAt(FollowedCamera.transform); 
                break;
            case Mode.LookAtInverted:
                //使得进度条始终都是从左到右，并且看向摄像头
                Vector3 dirFromCamera = transform.position - FollowedCamera.transform.position;
                transform.LookAt(transform.position+dirFromCamera);
                break;
            case Mode.CameraForward:
                transform.forward = FollowedCamera.transform.forward;
                break;
            case Mode.CameraBackwardInverted:
                transform.forward = -FollowedCamera.transform.forward;
                break;
        }
    }
}
