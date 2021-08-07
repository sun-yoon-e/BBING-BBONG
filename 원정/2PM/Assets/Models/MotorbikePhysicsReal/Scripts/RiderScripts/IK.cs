using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gadd420;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class IK : MonoBehaviour
{
    //How Many Parents Should Be Included
    public int chainLength = 2;

    public Transform target;
    public Transform pole;

    public int iterations = 10;

    [Header("Distance To Stop Calculations")]
    public float delta = 0.001f;

    [Range(0, 1)]
    public float snapBackStrength = 1f;

    public Vector3 extraRotation;
    public int rotationParentNumber;

    float[] bonesLength; 
    float completeLength;

    Transform[] bones;

    Vector3[] positions;
    Vector3[] startDirection;

    Quaternion[] startRotationBone;
    Quaternion startRotationTarget;

    Transform root;

    void Awake()
    {
        Initialise();
    }

    void Initialise()
    {
        //initial array
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];
        startDirection = new Vector3[chainLength + 1];
        startRotationBone = new Quaternion[chainLength + 1];

        //find root
        root = transform;
        for (var i = 0; i <= chainLength; i++)
        {
            if (root == null)
                throw new UnityException("The chain value is longer than the ancestor chain!");
            root = root.parent;
        }

        //init target
        if (target == null)
        {
            target = new GameObject(gameObject.name + " Target").transform;
            SetPositionRootSpace(target, GetPositionRootSpace(transform));
        }
        startRotationTarget = GetRotationRootSpace(target);


        //init data
        var current = transform;
        completeLength = 0;
        for (var i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            startRotationBone[i] = GetRotationRootSpace(current);

            if (i == bones.Length - 1)
            {
                //leaf
                startDirection[i] = GetPositionRootSpace(target) - GetPositionRootSpace(current);
            }
            else
            {
                //mid bone
                startDirection[i] = GetPositionRootSpace(bones[i + 1]) - GetPositionRootSpace(current);
                bonesLength[i] = startDirection[i].magnitude;
                completeLength += bonesLength[i];
            }

            current = current.parent;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        DoIK();
        
        bones[bones.Length - 1 - rotationParentNumber].localEulerAngles += extraRotation;
    }

    private void DoIK()
    {
        if (target == null)
        {
            return;
        }

        if (bonesLength.Length != chainLength)
        {
            Initialise();
        }
           
        //get position
        for (int i = 0; i < bones.Length; i++)
        {
            positions[i] = GetPositionRootSpace(bones[i]);
        }

        var targetPosition = GetPositionRootSpace(target);
        var targetRotation = GetRotationRootSpace(target);

        if ((targetPosition - GetPositionRootSpace(bones[0])).sqrMagnitude >= completeLength * completeLength)
        {
            var direction = (targetPosition - positions[0]).normalized;

            //set everything after root
            for (int i = 1; i < positions.Length; i++)
            {
                positions[i] = positions[i - 1] + direction * bonesLength[i - 1];
            }
                
        }
        else
        {
            for (int i = 0; i < positions.Length - 1; i++)
            {
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + startDirection[i], snapBackStrength);
            }

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = positions.Length - 1; i > 0; i--)
                {
                    if (i == positions.Length - 1)
                    {
                        //set it to target
                        positions[i] = targetPosition;
                    }
                    else
                    {
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i];
                    } 
                }

                for (int i = 1; i < positions.Length; i++)
                {
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * bonesLength[i - 1];
                }

                //Check if Close Enough
                if ((positions[positions.Length - 1] - targetPosition).sqrMagnitude < delta * delta)
                {
                    break;
                }    
            }
        }

        //move towards pole
        if (pole != null)
        {
            var polePosition = GetPositionRootSpace(pole);
            for (int i = 1; i < positions.Length - 1; i++)
            {
                var plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(polePosition);
                var projectedBone = plane.ClosestPointOnPlane(positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }

        //set position & rotation
        for (int i = 0; i < positions.Length; i++)
        {
            if (i == positions.Length - 1)
            {
                SetRotationRootSpace(bones[i], Quaternion.Inverse(targetRotation) * startRotationTarget * Quaternion.Inverse(startRotationBone[i]));
            }
            else
            {
                SetRotationRootSpace(bones[i], Quaternion.FromToRotation(startDirection[i], positions[i + 1] - positions[i]) * Quaternion.Inverse(startRotationBone[i]));
            }
                
            SetPositionRootSpace(bones[i], positions[i]);
        }
    }

    private Vector3 GetPositionRootSpace(Transform current)
    {
        if (root == null)
        {
            return current.position;
        }
        else
        {
            return Quaternion.Inverse(root.rotation) * (current.position - root.position);
        }
    }

    private void SetPositionRootSpace(Transform current, Vector3 position)
    {
        if (root == null)
        {
            current.position = position;
        }
        else
        {
            current.position = root.rotation * position + root.position;
        }
    }

    private Quaternion GetRotationRootSpace(Transform current)
    {
        if (root == null)
        {
            return current.rotation;
        }
        else
        {
            return Quaternion.Inverse(current.rotation) * root.rotation;
        }
    }

    private void SetRotationRootSpace(Transform current, Quaternion rotation)
    {
        if (root == null)
        {
            current.rotation = rotation;
        }
        else
        {
            current.rotation = root.rotation * rotation;
        }
            
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        var current = this.transform;
        for (int i = 0; i < chainLength && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.red;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
#endif
    }
}
