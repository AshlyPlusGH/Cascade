using System.Collections.Generic;
using UnityEngine;

namespace Shadowhulk
{
    public class Ship : Entity
    {
        private SHIP_Thruster[] thrusters;

        private float speed = 1;

        #region Functions

            public override void Move()
            {
                transform.position = Vector3.Lerp(transform.position,targetPosition,Time.deltaTime*speed);
            }

        #endregion

        #region Queries

            

        #endregion
    }
}