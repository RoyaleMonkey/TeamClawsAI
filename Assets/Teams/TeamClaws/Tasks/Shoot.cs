using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class Shoot : Action
    {
        public SharedBool CanShoot;
        public override TaskStatus OnUpdate()
        {
            InputData inputData = GetComponent<BOTController>().inputData;

            inputData.shoot = true;
            //GetComponent<BOTController>().inputData.shoot = true;
            return TaskStatus.Success;
        }
    }
}
