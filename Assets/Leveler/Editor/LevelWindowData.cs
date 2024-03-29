﻿using System.Collections.Generic;
using UnityEngine;

namespace Leveler
{
    public class LevelWindowData : ScriptableObject
    {
        public List<GameObject> prefabs;
        public List<string> validTypes = new();
        public int selectedPrefabIndex;
        public bool orientToNormal;
    }
}