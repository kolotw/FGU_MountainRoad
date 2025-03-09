﻿using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Saves debug settings
    /// </summary>
    public class DebugSettings : ScriptableObject
    {
        public bool debug = false;
        public bool debugSpeed = false;
        public bool debugIntersections = false;
        public bool debugWaypoints = false;
        public bool debugDisabledWaypoints = false;
        public bool drawBodyForces = false;
        public bool debugDensity = false;
        public bool debugPathFinding = false;
        public bool DebugSpawnWaypoints = false;
        public bool DebugPlayModeWaypoints = false;
        public bool ShowIndex = false;
        public bool ShowPosition = false;
        public bool DebugGiveWay =  false;
    }
}