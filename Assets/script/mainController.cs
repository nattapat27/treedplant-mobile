using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System;

public class MainController : MonoBehaviour
{
    public Camera FirstPersonCamera;

    public GameObject DetectedPlanePrefab;

    public GameObject PlanePrefab;

    public GameObject AndyPointPrefab;

    public GameObject ListModel;

    public GameObject ButtonArea;

    private const float k_ModelRotation = 180.0f;

    private bool m_IsQuitting = false;

    public void Start()
    {
        ButtonArea.SetActive(true);
        ListModel.SetActive(false);
        StartCoroutine(ConnectRestApi.sendGetMethod("/asset/getAllAsset/tree"));
    }

    public void Update()
    {
        _UpdateApplicationLifecycle();

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Should not handle input on UI.
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Choose model
                GameObject prefab;
                if (hit.Trackable is FeaturePoint)
                {
                    prefab = AndyPointPrefab;
                }
                else
                {
                    prefab = PlanePrefab;
                }

                // Instantiate model at the hit pose.
                var TreeObject = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

                // Compensate for the hitPose rotation facing away from the raycast (i.e.
                // camera).
                TreeObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of
                // the physical world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make Andy model a child of the anchor.
                TreeObject.transform.parent = anchor.transform;
            }
        }
    }

    private void _UpdateApplicationLifecycle()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            _DoQuit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Permission and ARCore
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage(
                "ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    private void _DoQuit()
    {
        Application.Quit();
    }

    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

    public void showList()
    {
        ButtonArea.SetActive(false);
        ListModel.SetActive(true);
        Debug.Log(ConnectRestApi.getRespone());
        string response = ConnectRestApi.getRespone();
        AssetAndTree[] assetAndTrees = JsonHelper.toAssetAndTree<AssetAndTree>(response);
        //Debug.Log(JsonHelper.toAssetAndTree(response));
        
    }

    public void quitList()
    {
        ButtonArea.SetActive(true);
        ListModel.SetActive(false);
    }

}
