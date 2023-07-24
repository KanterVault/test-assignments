using System;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public Guid Guid;
    public object Content;
    public virtual void UpdateContent() { }
}
