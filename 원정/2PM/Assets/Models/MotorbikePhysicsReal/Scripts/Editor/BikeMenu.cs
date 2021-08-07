using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gadd420;
public class BikeMenu
{
    [MenuItem("Gadd.Games/Vehicles/Without Rider/Set Up New Bike")]
    public static void BuildNewBike()
    {
        GameObject newBike = new GameObject("New_Bike", typeof(RB_Controller));

        GameObject newColliderGRP = new GameObject("Collider_GRP");
        newColliderGRP.transform.SetParent(newBike.transform);

        GameObject newMeshGRP = new GameObject("Mesh_GRP");
        newMeshGRP.transform.SetParent(newBike.transform);

        GameObject newWheelAndSteerGRP = new GameObject("WheelsAndSteering");
        newWheelAndSteerGRP.transform.SetParent(newBike.transform);

        GameObject frontWheelPos = new GameObject("FrontWheelPos");
        frontWheelPos.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject rearWheelPos = new GameObject("RearWheelPos");
        rearWheelPos.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject forksParent = new GameObject("Fork_Parent");
        forksParent.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject forkPivot = new GameObject("Fork_Pivot");
        forkPivot.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject newCrashTriggers = new GameObject("Crash_Triggers");
        newCrashTriggers.transform.SetParent(newBike.transform);

        GameObject nitrousGRP = new GameObject("Nitrous_GRP");
        nitrousGRP.transform.SetParent(newBike.transform);

        GameObject rayPosGRP = new GameObject("RayPosition_GRP");
        rayPosGRP.transform.SetParent(newBike.transform);

        GameObject rearRay = new GameObject("RearRayPos");
        rearRay.transform.SetParent(rayPosGRP.transform);

        GameObject frontRay = new GameObject("FrontRayPos");
        frontRay.transform.SetParent(rayPosGRP.transform);

        GameObject NewCOG = new GameObject("COG");
        NewCOG.transform.SetParent(newBike.transform);

        Selection.activeGameObject = newBike;
    }

    [MenuItem("Gadd.Games/Vehicles/Without Rider/Set Up New Push Bike")]
    public static void BuildNewPushBike()
    {
        GameObject newBike = new GameObject("New_PushBike", typeof(PedalManager));

        GameObject newColliderGRP = new GameObject("Collider_GRP");
        newColliderGRP.transform.SetParent(newBike.transform);

        GameObject newMeshGRP = new GameObject("Mesh_GRP");
        newMeshGRP.transform.SetParent(newBike.transform);

        GameObject newWheelAndSteerGRP = new GameObject("WheelsAndSteering");
        newWheelAndSteerGRP.transform.SetParent(newBike.transform);

        GameObject frontWheelPos = new GameObject("FrontWheelPos");
        frontWheelPos.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject rearWheelPos = new GameObject("RearWheelPos");
        rearWheelPos.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject forksParent = new GameObject("Fork_Parent");
        forksParent.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject forkPivot = new GameObject("Fork_Pivot");
        forkPivot.transform.SetParent(newWheelAndSteerGRP.transform);

        

        GameObject newCrashTriggers = new GameObject("Crash_Triggers");
        newCrashTriggers.transform.SetParent(newBike.transform);

        GameObject nitrousGRP = new GameObject("Nitrous_GRP");
        nitrousGRP.transform.SetParent(newBike.transform);

        GameObject pedalGRP = new GameObject("Pedal GRP");
        pedalGRP.transform.SetParent(newBike.transform);

        GameObject pedalPivot = new GameObject("Pedals Pivot");
        pedalPivot.transform.SetParent(pedalGRP.transform);

        GameObject lPedal = new GameObject("Left Pedal");
        lPedal.transform.SetParent(pedalPivot.transform);
        
        GameObject rPedal = new GameObject("Right Pedal");
        rPedal.transform.SetParent(pedalPivot.transform);

        GameObject rayPosGRP = new GameObject("RayPosition_GRP");
        rayPosGRP.transform.SetParent(newBike.transform);

        GameObject rearRay = new GameObject("RearRayPos");
        rearRay.transform.SetParent(rayPosGRP.transform);

        GameObject frontRay = new GameObject("FrontRayPos");
        frontRay.transform.SetParent(rayPosGRP.transform);

        GameObject NewCOG = new GameObject("COG");
        NewCOG.transform.SetParent(newBike.transform);


        Selection.activeGameObject = newBike;
    }

    [MenuItem("Gadd.Games/Vehicles/With Rider/Set Up New Bike")]
    public static void BuildNewBikeWithRiderSupport()
    {
        GameObject newBike = new GameObject("New_Bike", typeof(RB_Controller));

        GameObject newColliderGRP = new GameObject("Collider_GRP");
        newColliderGRP.transform.SetParent(newBike.transform);

        GameObject newMeshGRP = new GameObject("Mesh_GRP");
        newMeshGRP.transform.SetParent(newBike.transform);

        GameObject newWheelAndSteerGRP = new GameObject("WheelsAndSteering");
        newWheelAndSteerGRP.transform.SetParent(newBike.transform);

        GameObject frontWheelPos = new GameObject("FrontWheelPos");
        frontWheelPos.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject rearWheelPos = new GameObject("RearWheelPos");
        rearWheelPos.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject forksParent = new GameObject("Fork_Parent");
        forksParent.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject forkPivot = new GameObject("Fork_Pivot");
        forkPivot.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject handPosParent = new GameObject("HandPositions");
        handPosParent.transform.SetParent(forkPivot.transform);

        GameObject rightHandPos = new GameObject("RightHandPos");
        rightHandPos.transform.SetParent(handPosParent.transform);

        GameObject rightHandPole = new GameObject("RightHandPole");
        rightHandPole.transform.SetParent(handPosParent.transform);

        GameObject leftHandPos = new GameObject("LeftHandPos");
        leftHandPos.transform.SetParent(handPosParent.transform);

        GameObject leftHandPole = new GameObject("LeftHandPole");
        leftHandPole.transform.SetParent(handPosParent.transform);

        GameObject newCrashTriggers = new GameObject("Crash_Triggers");
        newCrashTriggers.transform.SetParent(newBike.transform);

        GameObject nitrousGRP = new GameObject("Nitrous_GRP");
        nitrousGRP.transform.SetParent(newBike.transform);

        GameObject rayPosGRP = new GameObject("RayPosition_GRP");
        rayPosGRP.transform.SetParent(newBike.transform);

        GameObject rearRay = new GameObject("RearRayPos");
        rearRay.transform.SetParent(rayPosGRP.transform);

        GameObject frontRay = new GameObject("FrontRayPos");
        frontRay.transform.SetParent(rayPosGRP.transform);

        GameObject NewCOG = new GameObject("COG");
        NewCOG.transform.SetParent(newBike.transform);

        GameObject feetPosParent = new GameObject("FeetPositions");
        feetPosParent.transform.SetParent(newBike.transform);

        GameObject rightFootPos = new GameObject("RightFootPos");
        rightFootPos.transform.SetParent(feetPosParent.transform);

        GameObject rightFootPole = new GameObject("RightFootPole");
        rightFootPole.transform.SetParent(feetPosParent.transform);

        GameObject leftFootPos = new GameObject("LeftFootPos");
        leftFootPos.transform.SetParent(feetPosParent.transform);

        GameObject leftFootPole = new GameObject("LeftFootPole");
        leftFootPole.transform.SetParent(feetPosParent.transform);

        GameObject headFollowPos = new GameObject("HeadFollowPosition",typeof(PlayerLeaning));
        headFollowPos.transform.SetParent(newBike.transform);


        Selection.activeGameObject = newBike;
    }

    [MenuItem("Gadd.Games/Vehicles/With Rider/Set Up New Push Bike")]
    public static void BuildNewPushBikeWithRiderSupport()
    {
        GameObject newBike = new GameObject("New_PushBike", typeof(PedalManager));

        GameObject newColliderGRP = new GameObject("Collider_GRP");
        newColliderGRP.transform.SetParent(newBike.transform);

        GameObject newMeshGRP = new GameObject("Mesh_GRP");
        newMeshGRP.transform.SetParent(newBike.transform);

        GameObject newWheelAndSteerGRP = new GameObject("WheelsAndSteering");
        newWheelAndSteerGRP.transform.SetParent(newBike.transform);

        GameObject frontWheelPos = new GameObject("FrontWheelPos");
        frontWheelPos.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject rearWheelPos = new GameObject("RearWheelPos");
        rearWheelPos.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject forksParent = new GameObject("Fork_Parent");
        forksParent.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject forkPivot = new GameObject("Fork_Pivot");
        forkPivot.transform.SetParent(newWheelAndSteerGRP.transform);

        GameObject handPosParent = new GameObject("HandPositions");
        handPosParent.transform.SetParent(forkPivot.transform);

        GameObject rightHandPos = new GameObject("RightHandPos");
        rightHandPos.transform.SetParent(handPosParent.transform);

        GameObject rightHandPole = new GameObject("RightHandPole");
        rightHandPole.transform.SetParent(handPosParent.transform);

        GameObject leftHandPos = new GameObject("LeftHandPos");
        leftHandPos.transform.SetParent(handPosParent.transform);

        GameObject leftHandPole = new GameObject("LeftHandPole");
        leftHandPole.transform.SetParent(handPosParent.transform);


        GameObject newCrashTriggers = new GameObject("Crash_Triggers");
        newCrashTriggers.transform.SetParent(newBike.transform);

        GameObject nitrousGRP = new GameObject("Nitrous_GRP");
        nitrousGRP.transform.SetParent(newBike.transform);

        GameObject pedalGRP = new GameObject("Pedal GRP");
        pedalGRP.transform.SetParent(newBike.transform);

        GameObject pedalPivot = new GameObject("Pedals Pivot");
        pedalPivot.transform.SetParent(pedalGRP.transform);

        GameObject feetPoleParent = new GameObject("FeetPoles");
        feetPoleParent.transform.SetParent(pedalGRP.transform);

        GameObject rightFootPole = new GameObject("RightFootPole");
        rightFootPole.transform.SetParent(feetPoleParent.transform);

        GameObject leftFootPole = new GameObject("LeftFootPole");
        leftFootPole.transform.SetParent(feetPoleParent.transform);

        GameObject lPedal = new GameObject("Left Pedal");
        lPedal.transform.SetParent(pedalPivot.transform);

        GameObject rPedal = new GameObject("Right Pedal");
        rPedal.transform.SetParent(pedalPivot.transform);

        GameObject rightFootPos = new GameObject("RightFootPos");
        rightFootPos.transform.SetParent(rPedal.transform);

        GameObject leftFootPos = new GameObject("LeftFootPos");
        leftFootPos.transform.SetParent(lPedal.transform);

        GameObject rayPosGRP = new GameObject("RayPosition_GRP");
        rayPosGRP.transform.SetParent(newBike.transform);

        GameObject rearRay = new GameObject("RearRayPos");
        rearRay.transform.SetParent(rayPosGRP.transform);

        GameObject frontRay = new GameObject("FrontRayPos");
        frontRay.transform.SetParent(rayPosGRP.transform);

        GameObject NewCOG = new GameObject("COG");
        NewCOG.transform.SetParent(newBike.transform);

        GameObject headFollowPos = new GameObject("HeadFollowPosition", typeof(PlayerLeaning));
        headFollowPos.transform.SetParent(newBike.transform);

        Selection.activeGameObject = newBike;
    }
}
