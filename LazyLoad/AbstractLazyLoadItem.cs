using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractLazyLoadItem : MonoBehaviour
{
    public bool ActiveSelf { get => gameObject.activeSelf; set => gameObject.SetActive(value); }
    public RectTransform RectTransfrom { get => GetComponent<RectTransform>(); }
    public abstract void SetData(string jsonBody);
}
