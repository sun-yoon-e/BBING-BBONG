using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleVisualizer : MonoBehaviour
{
    public LSystemGenerator lsystem;
    public List<Vector3> positions = new List<Vector3>();
    public GameObject prefab;
    public Material lineMaterial;

    private int length = 8;
    private float angle = 90;

    public int Length
    {
        get{
            return length;
        }
        set => length = value;
    }

    private void Start()
    {
        var sequence = lsystem.GenerateSentence();
        VisualizeSequence(sequence);
    }

    private void VisualizeSequence(string sequence)
    {
        Stack<AgentParameters> savePoint = new Stack<AgentParameters>();
        var currentPosition = Vector3.zero;

        Vector3 direction = Vector3.forward;
        Vector3 tempPositon = Vector3.zero;

        positions.Add(currentPosition);

        foreach(var letter in sequence)
        {
            EncodingLeters encoding = (EncodingLeters)letter;
            switch (encoding)
            {
                case EncodingLeters.save:
                    savePoint.Push(new AgentParameters
                    {
                        position = currentPosition,
                        direction = direction,
                        length = Length
                    });
                    break;
                case EncodingLeters.load:
                    if(savePoint.Count > 0)
                    {
                        var agentParameter = savePoint.Pop();
                        currentPosition = agentParameter.position;
                        direction = agentParameter.direction;
                        Length = agentParameter.length;
                    }
                    else
                    {
                        throw new System.Exception("Dont have saved point in our stack");
                    }
                    break;
                case EncodingLeters.draw:
                    tempPositon = currentPosition;
                    currentPosition += direction * length;
                    DrawLine(tempPositon, currentPosition, Color.red);
                    Length = Random.Range(4,8);
                    positions.Add(currentPosition);
                    break;
                case EncodingLeters.turnRight:
                    direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                    break;
                case EncodingLeters.trunLeft:
                    direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;

                    break;
                default:
                    break;
            }
        }

        foreach(var position in positions)
        {
            Instantiate(prefab, position, Quaternion.identity);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject line = new GameObject("line");
        line.transform.position = start;
        var lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    public enum EncodingLeters
    {
        unknown = '1',
        save = '[',
        load = ']',
        draw = 'F',
        turnRight = '+',
        trunLeft = '-'
    }
}
