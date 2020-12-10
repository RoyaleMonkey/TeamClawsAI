using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class GoToNextPoint : Action
    {
        public SharedTransform wayPointTransform;
        private Blackboard _blackboard;
        private SpaceShip spaceShip;

        private float newOrient;
        private bool asteroidDetected;
        private bool distanceCheck;
        public override TaskStatus OnUpdate()
        {
            ref InputData inputData = ref GetComponent<BOTController>().inputData;
            _blackboard = GetComponent<BOTController>()._blackboard;
            spaceShip = _blackboard._latestGameData.SpaceShips[_blackboard._owner];

            RaycastHit2D[] hit2D = Physics2D.LinecastAll(spaceShip.transform.position, spaceShip.transform.position + spaceShip.transform.right * 3);
            Debug.DrawLine(spaceShip.transform.position, spaceShip.transform.position + spaceShip.transform.right * 3);

            //check for asteroids
            foreach (RaycastHit2D hit in hit2D)
            {
                if (hit.collider != null)
                {

                    if (hit.collider.CompareTag("Asteroid"))
                    {
                        //asteroid detected
                        Vector2 min = new Vector2(hit.collider.bounds.center.x, hit.collider.bounds.min.y);
                        Vector2 max = new Vector2(hit.collider.bounds.center.x, hit.collider.bounds.max.y);

                        Debug.Log("min " + Vector2.Distance(min, spaceShip.Position));
                        Debug.Log("max " + Vector2.Distance(max, spaceShip.Position));

                        inputData.thrust = 0;
                        asteroidDetected = true;

                        if(!distanceCheck && Vector2.Distance(min, spaceShip.Position) < Vector2.Distance(max, spaceShip.Position))
                        {
                            newOrient = -90;
                            distanceCheck = true;
                        }
                        else if(!distanceCheck && Vector2.Distance(min, spaceShip.Position) > Vector2.Distance(max, spaceShip.Position))
                        {
                            newOrient = 90;
                            distanceCheck = true;
                        }

                        inputData.targetOrientation = newOrient;
                    }
                }
            }

            if (asteroidDetected)
            {
                if (spaceShip.GetComponent<Rigidbody2D>().rotation == newOrient)
                {
                    //dodge asteroid
                    asteroidDetected = false;
                    distanceCheck = false;
                }
            }

            if (!asteroidDetected)
            {
                //go to position
                inputData.targetOrientation = Mathf.Atan2(wayPointTransform.Value.position.y - spaceShip.Position.y, wayPointTransform.Value.position.x - spaceShip.Position.x) * 180 / Mathf.PI;
                inputData.thrust = 1;
            }


            if (wayPointTransform.Value.gameObject.GetComponent<WayPoint>().Owner == spaceShip.Owner)
            {
                //change target
                inputData.thrust = 0;
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}
