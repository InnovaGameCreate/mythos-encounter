using UnityEngine;
using System;
public class InstantiateTester : MonoBehaviour
{
    enum testType
    {
        small,
        medium,
        large
    }
    private float startTime;
    private float endTime;
    [SerializeField]
    private GameObject smallBurdem;
    [SerializeField]
    private GameObject mediumBurdem;
    [SerializeField]
    private GameObject largeBurdem;
    [SerializeField]
    private testType test;
    [SerializeField]
    private int testCount;

    void Start()
    {
        switch (test)
        {
            case testType.small:
                InstantiateTest(smallBurdem, testCount * 50);
                break;
            case testType.medium:
                InstantiateTest(mediumBurdem, testCount * 5);
                break;
            case testType.large:
                InstantiateTest(largeBurdem, testCount );
                break;
            default:
                break;
        }
    }

    private void InstantiateTest(GameObject testTarget,int count)
    {
        startTime = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
        for (int i = 0; i < count; i++)
        {
            Instantiate(testTarget);
        }
        endTime = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
        Debug.Log($"Time{endTime - startTime}:start{startTime}:end{endTime}");
    }
}
