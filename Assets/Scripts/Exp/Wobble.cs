using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    [SerializeField] private float _wobbleSpeed;
    [SerializeField] private float _wobbleOffset;
    [SerializeField] private float _wobbleRange;

    private Vector3 _initialPosition;

    private void Awake()
    {
        _initialPosition = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = _initialPosition + Vector3.up * Mathf.Sin(Time.time * _wobbleSpeed + _wobbleOffset) * _wobbleRange;
        transform.localEulerAngles = Vector3.forward * Mathf.Sin(Time.time * _wobbleSpeed + _wobbleOffset) * _wobbleRange;
    }
}
