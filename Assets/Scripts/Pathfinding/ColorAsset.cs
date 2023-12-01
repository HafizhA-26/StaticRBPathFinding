using Pathfinding;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Pathfinding
{
    [Serializable]
    public class ColorAsset
    {
        public ColorEnum color;
        public GameObject pathAssets;
        public GameObject assignedResourceAssets;
        public int stock;
        public TextMeshProUGUI showText;
    }
}