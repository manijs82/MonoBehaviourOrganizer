﻿using System.Collections.Generic;
using UnityEngine;

public class LevelWindowData : ScriptableObject
{
    public List<PlaceableObject> prefabs;
    public List<string> validTypes = new();
    public int selectedPrefabIndex;
    public bool orientToNormal;
}