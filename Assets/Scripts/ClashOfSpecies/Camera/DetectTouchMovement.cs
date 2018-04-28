using UnityEngine;
using System.Collections;

public class DetectTouchMovement : MonoBehaviour
{
    public static float pinchTurnRatio = 8;
    const float minTurnAngle = 1f;

    public static float pinchRatio = 5;
    const float minPinchDistance = 2.6f;

    const float panRatio = 1;
    public const float minPanDistance = 1;

    //    public static bool rotating = false, zooming = false;

    /// <summary>
    ///   The delta of the angle between two touch points
    /// </summary>
    static public float turnAngleDelta;

    public static float turnAngleDeltaAccumulated = 0;

    /// <summary>
    ///   The angle between two touch points
    /// </summary>
    static public float turnAngle;

    /// <summary>
    ///   The delta of the distance between two touch points that were distancing from each other
    /// </summary>
    static public float pinchDistanceDelta;

    public static float pinchDistanceDeltaAccumulated = 0.0f;

    /// <summary>
    ///   The distance between two touch points that were distancing from each other
    /// </summary>
    static public float pinchDistance;

    /// <summary>
    ///   Calculates Pinch and Turn - This should be used inside LateUpdate
    /// </summary>
    /// 
    static public COSTouchState Calculate(COSTouchState touchState)
    {
        pinchDistance = pinchDistanceDelta = 0;
        turnAngle = turnAngleDelta = 0;

        // if two fingers are touching the screen at the same time ...
//        if (Input.touchCount == 2)
//        {
        Touch touch1 = Input.touches[0];
        Touch touch2 = Input.touches[1];

        // ... if at least one of them moved ...
        if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
                

            // ... or check the delta angle between them ...
            turnAngle = Angle(touch1.position, touch2.position);
            float prevTurn = Angle(touch1.position - touch1.deltaPosition,
                                 touch2.position - touch2.deltaPosition);
            turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);
            turnAngleDeltaAccumulated += turnAngleDelta;

            // ... check the delta distance between them ...
            pinchDistance = Vector2.Distance(touch1.position, touch2.position);
            float prevDistance = Vector2.Distance(touch1.position - touch1.deltaPosition,
                                     touch2.position - touch2.deltaPosition);
            pinchDistanceDelta = pinchDistance - prevDistance;
            if (Mathf.Abs(turnAngleDeltaAccumulated) < minTurnAngle)
                pinchDistanceDeltaAccumulated += pinchDistanceDelta;
            else
                pinchDistanceDeltaAccumulated = 0;

            // ... if it's greater than a minimum threshold, it's a pinch!
            if (touchState == COSTouchState.IsZooming)
            {
                pinchDistanceDelta *= pinchRatio;
            }
            else if (Mathf.Abs(pinchDistanceDeltaAccumulated) > minPinchDistance
                     && touchState != COSTouchState.IsRotating
                     && Mathf.Abs(turnAngleDeltaAccumulated) < minTurnAngle)
            {
//                    pinchDistanceDelta *= pinchRatio;
                pinchDistanceDelta = 0;
                pinchDistanceDeltaAccumulated = 0f;
                touchState = COSTouchState.IsZooming;
//                    zooming = true;
                return touchState;
            }
            else
            {
                pinchDistance = pinchDistanceDelta = 0;
            }



            // ... if it's greater than a minimum threshold, it's a turn!
            if (touchState == COSTouchState.IsRotating)
            {
                turnAngleDelta *= pinchTurnRatio;
            }
            else if (Mathf.Abs(turnAngleDeltaAccumulated) > minTurnAngle)
            {
//                    turnAngleDelta *= pinchTurnRatio;
                turnAngleDelta = 0;
                turnAngleDeltaAccumulated = 0;
//                    rotating = true;
                touchState = COSTouchState.IsRotating;
            }
            else
            {
                turnAngle = turnAngleDelta = 0;
            }
        }
//        }
//        else
//        {
//            turnAngleDeltaAccumulated = 0;
//            pinchDistanceDeltaAccumulated = 0f;
//        }

        return touchState;
    }

    static private float Angle(Vector2 pos1, Vector2 pos2)
    {
        Vector2 from = pos2 - pos1;
        Vector2 to = new Vector2(1, 0);

        float result = Vector2.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        if (cross.z > 0)
        {
            result = 360f - result;
        }

        return result;
    }
}