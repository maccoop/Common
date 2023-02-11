using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRatioChangesize : MonoBehaviour
{
    [SerializeField] private InfiniteContentPadding _padding;
    void Start()
    {
        var index = Screen.height * 1f / Screen.width;
        GetComponent<RectTransform>().sizeDelta = new Vector2(1080 - _padding.Left - _padding.Right, 1080 * index - _padding.Top - _padding.Down);
    }
}
