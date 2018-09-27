using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;



public class SceneController : MonoBehaviour
{

    public Camera firstPersonCamera;

    public ScoreboardController scoreboard;

    public SnakeController snakeController;

    // Use this for initialization
    void Start()
    {
        QuitConnectionErrors();

    }

    // Update is called once per frame
    void Update()
    {

        if (Session.Status != SessionStatus.Tracking)
        {

            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        ProcessTouches();

        scoreboard.SetScore(snakeController.GetLength());

    }

    //Checking errors and exit
    void QuitConnectionErrors()
    {

        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {

            StartCoroutine(CodelabUtils.ToastAndExit("Camera permission not given", 5));

        }
        else if (Session.Status.IsError())
        {

            StartCoroutine(CodelabUtils.ToastAndExit("Error has occured restart the app", 5));

        }
    }

    void ProcessTouches()
    {
        Touch touch;
        if (Input.touchCount != 1 ||
            (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter =
            TrackableHitFlags.PlaneWithinBounds |
            TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            SetSelectedPlane(hit.Trackable as DetectedPlane);
        }
    }

    void SetSelectedPlane(DetectedPlane selectedPlane)
    {
        // Add to the end of SetSelectedPlane.
        scoreboard.SetSelectedPlane(selectedPlane);
        // Add to SetSelectedPlane()
        snakeController.SetPlane(selectedPlane);

        GetComponent<FoodController>().SetSelectedPlane(selectedPlane);
    }
}
