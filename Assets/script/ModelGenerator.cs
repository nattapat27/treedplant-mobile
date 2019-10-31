using GoogleARCore;
using GoogleARCore.Examples.ObjectManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModelGenerator : Manipulator
{
    public Camera FirstPersonCamera;
    public GameObject ManipulatorPrefab;
    public GameObject ui;
    public static string id = "1";
    public static Dictionary<string, GameObject> modelList = new Dictionary<string, GameObject>();
    public static Dictionary<string, Cart> cart = new Dictionary<string, Cart>();
    public static int Index;
    public static string AssetId;
    
    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if (gesture.TargetObject == null)
        {
            return true;
        }

        return false;
    }
    protected override void OnEndManipulation(TapGesture gesture)
    {
        if (gesture.WasCancelled)
        {
            return;
        }

        // If gesture is targeting an existing object we are done.
        if (gesture.TargetObject != null)
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(
            gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject() && 
                    EventSystem.current.currentSelectedGameObject != null 
                    )
                    return;
                if (id.Equals("1"))
                    return;

                // Instantiate Andy model at the hit pose.
                var andyObject = Instantiate(modelList[id], hit.Pose.position, hit.Pose.rotation);

                if (!cart.ContainsKey(id))
                {
                    Cart temp  = new Cart(Index, id, 0, AssetId);                    
                    cart[id] = temp;
                }
                Debug.Log(id);
                cart[id].SetNumber(cart[id].GetNumber() + 1);

                // Instantiate manipulator.
                var manipulator =
                    Instantiate(ManipulatorPrefab, hit.Pose.position, hit.Pose.rotation);

                // Make Andy model a child of the manipulator.
                andyObject.transform.parent = manipulator.transform;

                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make manipulator a child of the anchor.
                manipulator.transform.parent = anchor.transform;

                // Select the placed object.
                manipulator.GetComponent<Manipulator>().Select();

                manipulator.tag = andyObject.tag;

            }
        }
    }
    public static Dictionary<string, Cart> GetCart()
    {
        return cart;
    }
}
