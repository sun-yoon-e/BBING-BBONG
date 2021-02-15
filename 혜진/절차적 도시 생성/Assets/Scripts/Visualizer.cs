using System;
using System.Collections.Generic;
using UnityEngine;

namespace SVS
{
    public class Visualizer : MonoBehaviour
    {
        public LSystemGenerator lsystem;
        List<Vector3> positions = new List<Vector3>();

        public RoadHelper RoadHelper;
        
        //public GameObject prefab;
        //public Material lineMaterial;

        private int length = 8;
        private float angle = 90;

        public int Length
        {
            get
            {
                if (length > 0)
                {
                    return length;
                }
                else
                {
                    return 1;
                }
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
            Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
            var currentPosition = Vector3.zero;

            Vector3 direction = Vector3.forward;
            Vector3 tempPosition = Vector3.zero;

            positions.Add(currentPosition);

            foreach (var letter in sequence)
            {
                SimpleVisualizer.EncodingLetters encoding = (SimpleVisualizer.EncodingLetters) letter;
                switch (encoding)
                {
                    case SimpleVisualizer.EncodingLetters.save:
                        savePoints.Push(new AgentParameters
                        {
                            position = currentPosition,
                            direction = direction,
                            length = Length
                        });
                        break;
                    case SimpleVisualizer.EncodingLetters.load:
                        if (savePoints.Count > 0)
                        {
                            var agentParameter = savePoints.Pop();
                            currentPosition = agentParameter.position;
                            direction = agentParameter.direction;
                            Length = agentParameter.length;
                        }
                        else
                        {
                            throw new System.Exception("Dont have saved point in our stack");
                        }

                        break;
                    case SimpleVisualizer.EncodingLetters.draw:
                        tempPosition = currentPosition;
                        currentPosition += direction * length;
                        RoadHelper.PlaceStreetPositions(tempPosition, Vector3Int.RoundToInt(direction), length);
                        Length -= 2;
                        positions.Add(currentPosition);
                        break;
                    case SimpleVisualizer.EncodingLetters.turnRight:
                        direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                        break;
                    case SimpleVisualizer.EncodingLetters.turnLeft:
                        direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}