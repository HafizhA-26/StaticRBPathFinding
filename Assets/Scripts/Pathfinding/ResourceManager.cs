using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Pathfinding
{
    public class ResourceManager : MonoBehaviour
    {
        public GameObject splines;
        public GameObject[] resourceSlots;

        private ColorAsset[] pathAssets;
        private List<int> defaultStock = new List<int>();
        private List<ColorEnum> resource = new List<ColorEnum>();
        private List<GameObject> resourceUse = new List<GameObject>();
        private int maxResource;

        public List<ColorEnum> Resource { get => resource; set => resource = value; }
        public int MaxResource { get => maxResource; set => maxResource = value; }

        private void Start()
        {
            pathAssets = splines.GetComponent<MoveAlongSpline>().pathAssets;
            for (int i = 0; i < pathAssets.Length; i++)
            {
                pathAssets[i].showText.text = pathAssets[i].stock.ToString();
                defaultStock.Add(pathAssets[i].stock);
            }
            maxResource = resourceSlots.Length;

           
        }
        public void AddResource(int typeIndex)
        {
            
            ColorEnum type;
            switch (typeIndex)
            {
                case 1:
                    type = ColorEnum.FullSail; break;
                case 2:
                    type = ColorEnum.HalfSail; break;
                case 3:
                    type = ColorEnum.NoSail; break;
                default:
                    type = ColorEnum.FullSail; break;
            }
            int index = typeIndex - 1;
            ColorAsset ca = pathAssets[index];
            if(ca.stock == 0)
            {
                Debug.Log("Stock Resource Habis");
                return;
            }
            if (resource.Count < maxResource)
            {
                resource.Add(type);
                Vector3 newResourcePos = resourceSlots[resource.Count - 1].transform.position;
                GameObject assignedResource = (GameObject) Instantiate(ca.assignedResourceAssets, newResourcePos, Quaternion.identity);
                resourceUse.Add(assignedResource);
                pathAssets[index].stock -= 1;
                pathAssets[index].showText.text = pathAssets[index].stock.ToString();
            }
            else
            {
                Debug.Log("Telah Mencapai Batas Resource");
            }
        }
        public void RemoveResourceAt(int index)
        {
            if(resource.Count - 1 < index) {
                Debug.Log("Tidak Bisa Hapus Resource");
                return;
            }

        }
        public void ClearResources()
        {
            resource.Clear();
            foreach (var item in resourceUse)
            {
                Destroy(item);
            }
            resourceUse.Clear();
            for (int i = 0; i < pathAssets.Length; i++)
            {
                pathAssets[i].stock = defaultStock.ElementAt(i);
                pathAssets[i].showText.text = pathAssets[i].stock.ToString();
            }
        }

        public int indexOfColor(ColorEnum color)
        {
            int index = 0;
            foreach(var c in pathAssets)
            {
                if(c.color == color)
                {
                    return index;
                }
            }
            return -1;
        }

    }
}