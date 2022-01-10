using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Fsm2.Behaviours.Ai
{
    public interface IAiTargetFinder
    {
        IAiTarget target { get; }
        bool hasTarget { get; }
    }
}