using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecreatingLevel : MonoBehaviour
{
    [SerializeField]
    private GameObject level;
    private float _yDifference;
    private float _yActuall;
    private Transform _myTransform;
    private Transform _levelTransform;

    void Start()
    {
        _myTransform = transform;
        _levelTransform = _myTransform;
        _yDifference = 13.5f;
        StartCoroutine(recreate());
    }

    void Update()
    {
        
    }

    private IEnumerator recreate()
    {
       
        yield return new WaitForSeconds(60);
        
        _yActuall += _yDifference;

        Instantiate(level, _levelTransform);
        level.transform.position += new Vector3(0, _yDifference, 0);
        StartCoroutine(recreate());
    }
}
