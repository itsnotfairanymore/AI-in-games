using System;
using K_PathFinder.Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace K_PathFinder.Samples 
{
    [RequireComponent(typeof(PathFinderAgent), typeof(CharacterController))]
    public class Exercise : MonoBehaviour 
    {
        public LineRenderer line;
        public SimplePatrolPath patrol;

        [Range(0f, 5f)] 
        public float speed = 3;

        public bool playerFinded = false; 
        public Transform player; 

        private CharacterController controler;
        private PathFinderAgent agent;  
        private int currentPoint;

        void Start() 
        {
            if (patrol == null || patrol.Count == 0)
                Debug.LogError("Not valid patrol path");

            controler = GetComponent<CharacterController>();
            agent = GetComponent<PathFinderAgent>();     

            float sqrDist = float.MaxValue;
            Vector3 pos = transform.position;

            for (int i = 0; i < patrol.Count; i++) 
            {
                float curSqrDist = (patrol[i] - pos).sqrMagnitude;

                if (curSqrDist < sqrDist) 
                {
                    sqrDist = curSqrDist;
                    currentPoint = i;
                }
            }

            agent.SetRecievePathDelegate(RecivePathDelegate, AgentDelegateMode.ThreadSafe);

            PathFinder.QueueGraph(new Bounds(transform.position, Vector3.one * 20), agent.properties);
        }
        
        void Update() 
        { 
            if (playerFinded && player != null)
            {
                agent.SetGoalMoveHere(player.position);
            }

            if (agent.haveNextNode) 
            {
                if (agent.RemoveNextNodeIfCloserThanRadiusVector2()) 
                {
                    if (agent.haveNextNode == false) 
                    {
                        if (!playerFinded)
                        {
                            currentPoint++;

                            if (currentPoint >= patrol.Count)
                                currentPoint = 0;

                            RecalculatePath();
                        }
                    }
                }

                if (agent.haveNextNode) 
                {
                    Vector2 moveDirection = agent.nextNodeDirectionVector2.normalized;                  
                    controler.SimpleMove(new Vector3(moveDirection.x, 0, moveDirection.y) * speed);
                }
            }
            else
            {
                if (!playerFinded)
                {
                    RecalculatePath();
                }
            }
        } 

        public void RecalculatePath() 
        {
            agent.SetGoalMoveHere(patrol[currentPoint]);
        }

        void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);
            }
        }
        
        private void RecivePathDelegate(Path path) 
        {
            if (path.pathType != PathResultType.Valid)
                Debug.LogWarningFormat("path is not valid. reason: {0}", path.pathType);

            ExampleThings.PathToLineRenderer(agent.positionVector3, line, path, 0.2f);
        }
    }
}