using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class GoToNextPoint : Action
    {
        //references
        public SharedTransform wayPointTransform;
        private Blackboard _blackboard;
        private SpaceShip spaceShip;

        //dodge variables
        private Vector2 minY;
        private Vector2 maxY;
        private Vector2 minX;
        private Vector2 maxX;

        private bool goUp;
        private bool goDown;
        private bool goLeft;
        private bool goRight;

        private float newOrient;
        private bool asteroidDetected;
        private bool distanceCheck;
        public override TaskStatus OnUpdate()
        {
            //references
            ref InputData inputData = ref GetComponent<BOTController>().inputData;
            _blackboard = GetComponent<BOTController>()._blackboard;
            spaceShip = _blackboard._latestGameData.SpaceShips[_blackboard._owner];

            //raycast
            RaycastHit2D[] hit2D = Physics2D.LinecastAll(spaceShip.transform.position, spaceShip.transform.position + spaceShip.transform.right * 2);
            Debug.DrawLine(spaceShip.transform.position, spaceShip.transform.position + spaceShip.transform.right * 2);

            //check for asteroids
            foreach (RaycastHit2D hit in hit2D)
            {
                if (hit.collider != null && !asteroidDetected)
                {

                    if (hit.collider.CompareTag("Asteroid"))
                    {
                        //asteroid detected

                        //get collider bounds
                        minY = new Vector2(hit.collider.bounds.center.x, hit.collider.bounds.min.y);
                        maxY = new Vector2(hit.collider.bounds.center.x, hit.collider.bounds.max.y);

                        minX = new Vector2(hit.collider.bounds.min.x, hit.collider.bounds.center.y);
                        maxX = new Vector2(hit.collider.bounds.max.x, hit.collider.bounds.center.y);

                        inputData.thrust = 0;
                        asteroidDetected = true;

                        if (spaceShip.Position.y < minY.y || spaceShip.Position.y > maxY.y)
                        {
                            if (!distanceCheck && Vector2.Distance(minX, spaceShip.Position) < Vector2.Distance(maxX, spaceShip.Position))
                            {
                                //dodge toward left bound
                                Debug.Log("gauche");
                                newOrient = (Mathf.Atan2(minX.y - spaceShip.Position.y, minX.x - spaceShip.Position.x) * 180 / Mathf.PI) + 45;
                                goLeft = true;
                                distanceCheck = true;
                            }
                            else if (!distanceCheck && Vector2.Distance(minX, spaceShip.Position) > Vector2.Distance(maxX, spaceShip.Position))
                            {
                                //dodge toward right bound
                                Debug.Log("droite");
                                newOrient = (Mathf.Atan2(maxX.y - spaceShip.Position.y, maxX.x - spaceShip.Position.x) * 180 / Mathf.PI) - 45;
                                goRight = true;
                                distanceCheck = true;
                            }
                        }

                        if (spaceShip.Position.x < minX.x || spaceShip.Position.x > maxX.x)
                        {
                            if (!distanceCheck && Vector2.Distance(minY, spaceShip.Position) < Vector2.Distance(maxY, spaceShip.Position))
                            {
                                // dodge toward down bound
                                Debug.Log("bas");
                                newOrient = (Mathf.Atan2(minY.y - spaceShip.Position.y, minY.x - spaceShip.Position.x) * 180 / Mathf.PI) + 45;
                                goDown = true;
                                distanceCheck = true;
                            }
                            else if (!distanceCheck && Vector2.Distance(minY, spaceShip.Position) > Vector2.Distance(maxY, spaceShip.Position))
                            {
                                //dodge toward up bound
                                Debug.Log("haut");
                                newOrient = (Mathf.Atan2(maxY.y - spaceShip.Position.y, maxY.x - spaceShip.Position.x) * 180 / Mathf.PI) - 45;
                                goUp = true;
                                distanceCheck = true;
                            }
                        }

                        inputData.targetOrientation = newOrient;
                    }

                    if (hit.collider.CompareTag("Mine"))
                    {
                        spaceShip.Shoot();
                    }
                }
            }

            if (asteroidDetected)
            {
                if (distanceCheck && Mathf.Approximately(spaceShip.GetComponent<Rigidbody2D>().rotation, newOrient))
                {
                    //dodge asteroid

                    if (goUp)
                    {
                        Debug.Log("stop");
                        goUp = false;
                        distanceCheck = false;
                        asteroidDetected = false;
                    }
                    if (goDown)
                    {

                        Debug.Log("stop");
                        goDown = false;
                        distanceCheck = false;
                        asteroidDetected = false;
                    }
                    if (goLeft)
                    {
                        Debug.Log("stop");
                        goLeft = false;
                        distanceCheck = false;
                        asteroidDetected = false;
                    }
                    if (goRight)
                    {
                        Debug.Log("stop");
                        goRight = false;
                        distanceCheck = false;
                        asteroidDetected = false;
                    }
                }
            }

            else
            {
                //go to position
                newOrient = Mathf.Atan2(wayPointTransform.Value.position.y - spaceShip.Position.y, wayPointTransform.Value.position.x - spaceShip.Position.x) * 180 / Mathf.PI;
                inputData.targetOrientation = newOrient;
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
