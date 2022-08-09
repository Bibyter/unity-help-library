using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Ai
{
    public sealed class AiData
    {
        public enum MoveType { Walk, Run }
        public MoveType moveType;

        public AiActionState actionState;

        public Vector3 destination;

        public float pursuitStopDistance;

        public bool hasTarget;
        public IAiTarget target;
        public float targetDistance;

        public float attackDistance;

        public string GetMoveAnimKey()
        {
            switch (moveType)
            {
                case MoveType.Walk: return "walk";
                case MoveType.Run: return "run";
                default: return "none";
            }
        }
    }
}
