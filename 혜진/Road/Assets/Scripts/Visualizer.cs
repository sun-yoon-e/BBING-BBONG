using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SVS.SimpleVisualizer;

namespace SVS
{
    public class Visualizer : MonoBehaviour
    {
        public LSystemGenerator lsystem;
        private List<Vector3> positions = new List<Vector3>();
        private Terrain terrain;

        public RoadHelper roadHelper;
        public StructureHelper structureHelper;
        public ChangeTerrainHeight changeTerrainHeight;
        public TerrainGenerator terrainGenerator;

        private int length = 15;
        private float angle = 90;

        public int Length
        {
            get
            {
                if (length > 5)
                {
                    return length;
                }
                else
                {
                    return 5;
                }
            }
            set => length = value;
        }

        private void Start()
        {
            terrain = GameObject.Find("Terrain").GetComponent<Terrain>();

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
                EncodingLetters encoding = (EncodingLetters)letter;
                switch (encoding)
                {
                    case EncodingLetters.save:
                        savePoints.Push(new AgentParameters
                        {
                            position = currentPosition,
                            direction = direction,
                            length = Length
                        });
                        break;
                    case EncodingLetters.load:
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
                    case EncodingLetters.draw:
                        tempPosition = currentPosition;
                        currentPosition += direction * length;
                        roadHelper.PlaceStreetPositions(tempPosition, Vector3Int.RoundToInt(direction), length);
                        //Length -= 1;
                        positions.Add(currentPosition);
                        break;
                    case EncodingLetters.turnRight:
                        direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                        break;
                    case EncodingLetters.turnLeft:
                        direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                        break;
                }
            }
            roadHelper.FixRoad();
            //structureHelper.PlaceStructuresAroundRoad(roadHelper.GetRoadPositions());

            terrain.terrainData = terrainGenerator.GenerateTerrain(terrain.terrainData);

            //changeTerrainHeight.ConvertWordCor2TerrCor(roadHelper.GetRoadPositions());
            changeTerrainHeight.FixTerrainHeight(roadHelper.GetRoadPositions());
        }
    }
}