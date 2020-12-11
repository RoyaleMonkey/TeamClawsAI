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
            ref Blackboard _blackboard = ref GetComponent<BOTController>()._blackboard;
            SpaceShip spaceShip = _blackboard._latestGameData.SpaceShips[_blackboard._owner];

            //raycast
            RaycastHit2D hit2D = Physics2D.Linecast(spaceShip.transform.position, spaceShip.transform.position + spaceShip.transform.right, 1 << 16);
            Debug.DrawLine(spaceShip.transform.position, spaceShip.transform.position + spaceShip.transform.right * 2);
            
            RaycastHit2D velocityHit = Physics2D.Linecast(spaceShip.transform.position, spaceShip.transform.position + (Vector3)spaceShip.Velocity, 1 << 13);
            Vector3 velocityRight = Quaternion.AngleAxis(5, Vector3.forward) * ((spaceShip.transform.position + (Vector3)spaceShip.Velocity) - spaceShip.transform.position);
            Vector3 velocityLeft = Quaternion.AngleAxis(-5, Vector3.forward) * ((spaceShip.transform.position + (Vector3)spaceShip.Velocity) - spaceShip.transform.position);
            RaycastHit2D velocityHitRight = Physics2D.Linecast(spaceShip.transform.position, spaceShip.transform.position + velocityRight, 1 << 13);
            RaycastHit2D velocityHitLeft = Physics2D.Linecast(spaceShip.transform.position, spaceShip.transform.position + velocityLeft, 1 << 13);
            Debug.DrawLine(spaceShip.transform.position, spaceShip.transform.position + (Vector3)spaceShip.Velocity);
            Debug.DrawLine(spaceShip.transform.position, spaceShip.transform.position + velocityRight);
            Debug.DrawLine(spaceShip.transform.position, spaceShip.transform.position + velocityLeft);


            Vector3 right = Quaternion.AngleAxis(15, Vector3.forward) * ((spaceShip.transform.position + spaceShip.transform.right) - spaceShip.transform.position);
            Vector3 left = Quaternion.AngleAxis(-15, Vector3.forward) * ((spaceShip.transform.position + spaceShip.transform.right) - spaceShip.transform.position);

            RaycastHit2D hit2DRight = Physics2D.Linecast(spaceShip.transform.position, spaceShip.transform.position + right, 1 << 12);
            RaycastHit2D hit2DLeft = Physics2D.Linecast(spaceShip.transform.position, spaceShip.transform.position + left, 1 << 12);
            Debug.DrawLine(spaceShip.transform.position, spaceShip.transform.position + right);
            Debug.DrawLine(spaceShip.transform.position, spaceShip.transform.position + left);

            float velocityOrientation = Vector2.SignedAngle(Vector2.right, spaceShip.Velocity);

            if (hit2D.collider && hit2D.collider.CompareTag("Mine"))
            {
                Debug.Log("Shoot Mine!");
                GetComponent<BOTController>()._blackboard._latestGameData.SpaceShips[GetComponent<BOTController>()._blackboard._owner].Shoot();
            }
            else if ((velocityHit.collider && velocityHit.collider.CompareTag("Mine")))
            {
                float DiffVelocityToTarget = Vector2.SignedAngle(spaceShip.Velocity, (Vector2)velocityHit.collider.transform.position - spaceShip.Position);
                float DiffIncreased = DiffVelocityToTarget * 1.5f;
                DiffIncreased = Mathf.Clamp(DiffIncreased, -179, 179);
                inputData.targetOrientation = velocityOrientation + DiffIncreased;

                inputData.thrust = 0f;
                return TaskStatus.Running;
            }
            else if (velocityHitRight.collider && velocityHitRight.collider.CompareTag("Mine"))
            {
                float DiffVelocityToTarget = Vector2.SignedAngle(spaceShip.Velocity, (Vector2)velocityHitRight.collider.transform.position - spaceShip.Position);
                float DiffIncreased = DiffVelocityToTarget * 1.5f;
                DiffIncreased = Mathf.Clamp(DiffIncreased, -179, 179);
                inputData.targetOrientation = velocityOrientation + DiffIncreased;

                inputData.thrust = 0f;
                return TaskStatus.Running;
            }
            else if (velocityHitLeft.collider && velocityHitLeft.collider.CompareTag("Mine"))
            {
                float DiffVelocityToTarget = Vector2.SignedAngle(spaceShip.Velocity, (Vector2)velocityHitLeft.collider.transform.position - spaceShip.Position);
                float DiffIncreased = DiffVelocityToTarget * 1.5f;
                DiffIncreased = Mathf.Clamp(DiffIncreased, -179, 179);
                inputData.targetOrientation = velocityOrientation + DiffIncreased;

                inputData.thrust = 0f;
                return TaskStatus.Running;
            }


            float diffVelocityToTarget = Vector2.SignedAngle(spaceShip.Velocity, (Vector2)wayPointTransform.Value.position - spaceShip.Position);
            float diffIncreased = diffVelocityToTarget * 1.5f;
            diffIncreased = Mathf.Clamp(diffIncreased, -179, 179);
            float OrientTarget = velocityOrientation + diffIncreased;

            inputData.thrust = 1f;

            if (hit2DRight.collider == null && hit2DLeft.collider == null)
            {
                //Debug.Log("FIRST TARGET");
                newOrient = OrientTarget;
            }
            else if (hit2DRight.collider == null && hit2DLeft.collider != null && hit2DLeft.collider.CompareTag("Asteroid"))
            {
                //Debug.Log("RIGHT");
                newOrient = (Mathf.Atan2((spaceShip.transform.position + right).y, (spaceShip.transform.position + right).x) * 180 / Mathf.PI) - 90;
            }
            else if (hit2DRight.collider != null && hit2DLeft.collider == null && hit2DRight.collider.CompareTag("Asteroid"))
            {
                //Debug.Log("LEFT");
                newOrient = (Mathf.Atan2((spaceShip.transform.position + left).y, (spaceShip.transform.position + left).x) * 180 / Mathf.PI) + 90;
            }

            else if (hit2DRight.distance >= hit2DLeft.distance && hit2DLeft.collider.CompareTag("Asteroid") && hit2DRight.collider.CompareTag("Asteroid"))
            {
                //Debug.Log("RIGHT");
                newOrient = (Mathf.Atan2((spaceShip.transform.position + right).y, (spaceShip.transform.position + right).x) * 180 / Mathf.PI) - 90;
            }
            else if (hit2DRight.distance < hit2DLeft.distance && hit2DLeft.collider.CompareTag("Asteroid") && hit2DRight.collider.CompareTag("Asteroid"))
            {
                //Debug.Log("LEFT");
                newOrient = (Mathf.Atan2((spaceShip.transform.position + left).y, (spaceShip.transform.position + left).x) * 180 / Mathf.PI) + 90;
            }
            else
            {
                //Debug.Log("LAST TARGET");
                newOrient = OrientTarget;
            }

            inputData.targetOrientation = newOrient;


            //check for asteroids
            /*foreach (RaycastHit2D hit in hit2D)
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
            }*/


            if (wayPointTransform.Value.gameObject.GetComponent<WayPoint>().Owner == spaceShip.Owner)
            {
                //change target
                inputData.thrust = 0;
                int decrement = (_blackboard._behaviorTree.GetVariable("NbWayPointToTakeInCluster") as SharedInt).Value - 1;
                if (decrement < 0)
                    decrement = 0;
                _blackboard._behaviorTree.SetVariableValue("NbWayPointToTakeInCluster", decrement);
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}
