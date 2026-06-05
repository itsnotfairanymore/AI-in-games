using UnityEngine;

namespace K_PathFinder.Samples
{
    [RequireComponent(typeof(PathFinderAgent), typeof(CharacterController))]
    public class PathfinderCleaner : MonoBehaviour
    {
        public Transform[] trashItems;
        public float speed = 3f;
        public float collectDistance = 1f;

        private PathFinderAgent agent;
        private CharacterController controller;
        private Transform currentTarget;

        void Start()
        {
            agent = GetComponent<PathFinderAgent>();
            controller = GetComponent<CharacterController>();

            if (agent.properties == null)
            {
                Debug.LogError("Agent Properties is missing on Path Finder Agent.");
                return;
            }

            PathFinder.QueueGraph(new Bounds(transform.position, Vector3.one * 40), agent.properties);

            ChooseNearestTrash();
        }

        void Update()
        {
            if (currentTarget == null)
                return;

            float distance = Vector3.Distance(
                new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(currentTarget.position.x, 0, currentTarget.position.z)
            );

            if (distance <= collectDistance)
            {
                CollectCurrentTrash();
                ChooseNearestTrash();
                return;
            }

            if (agent.haveNextNode)
            {
                agent.RemoveNextNodeIfCloserThanRadiusVector2();

                if (agent.haveNextNode)
                {
                    Vector2 moveDirection = agent.nextNodeDirectionVector2.normalized;
                    Vector3 movement = new Vector3(moveDirection.x, 0, moveDirection.y) * speed;

                    controller.SimpleMove(movement);
                }
            }
            else
            {
                SetPathToCurrentTarget();
            }
        }

        void ChooseNearestTrash()
        {
            float closestDistance = float.MaxValue;
            Transform closestTrash = null;

            foreach (Transform trash in trashItems)
            {
                if (trash == null)
                    continue;

                if (!trash.gameObject.activeInHierarchy)
                    continue;

                float distance = Vector3.Distance(transform.position, trash.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTrash = trash;
                }
            }

            currentTarget = closestTrash;

            if (currentTarget != null)
            {
                Debug.Log("New target: " + currentTarget.name);
                SetPathToCurrentTarget();
            }
            else
            {
                Debug.Log("All trash collected. Cleaner stopped.");
            }
        }

        void SetPathToCurrentTarget()
        {
            if (currentTarget == null)
                return;

            PathFinder.QueueGraph(new Bounds(transform.position, Vector3.one * 30), agent.properties);
            PathFinder.QueueGraph(new Bounds(currentTarget.position, Vector3.one * 30), agent.properties);

            agent.SetGoalMoveHere(currentTarget.position);
        }

        void CollectCurrentTrash()
        {
            Debug.Log("Collected: " + currentTarget.name);
            currentTarget.gameObject.SetActive(false);
        }
    }
}