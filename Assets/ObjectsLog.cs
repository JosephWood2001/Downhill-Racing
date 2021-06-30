using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsLog : MonoBehaviour
{

    public class PartInfo
    {
        public PartInfo(PartPanel part, GameObject obj)
        {
            this.part = part;
            this.obj = obj;
        }

        public PartPanel part;
        public GameObject obj;
    }

    public List<PartInfo> parts;

    private void Start()
    {
        parts = new List<PartInfo>();
    }

    public void Add(PartInfo part)
    {
        parts.Add(part);
    }

    public void Remove(PartInfo part)
    {
        parts.Remove(part);
    }

    public List<PartInfo> GetParts()
    {
        return parts;
    }
}
