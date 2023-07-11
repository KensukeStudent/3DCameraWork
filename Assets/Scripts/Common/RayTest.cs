using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    [SerializeField, Range(0.0f, 10)]
    private float radius = 0.5f;

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private LayerMask layerMask;

    private RaycastHit[] hits = new RaycastHit[3];

    private RaycastHit hit = new RaycastHit();

    private void Update()
    {
        // hits = new RaycastHit[3];
        // TestA();
        Test2();
    }

    private void TestA()
    {
        // CapsuleCastNonAllocを使用して領域を決めた当たり判定を作る
        Vector3 center = transform.position;
        var t = target.transform.position;
        var direction = t - center;
        var distance = Vector3.Distance(t, center);
        int count = Physics.SphereCastNonAlloc(center, radius, direction, hits, distance, layerMask, QueryTriggerInteraction.Collide);

        Debug.DrawRay(center, direction, Color.red, 0.2f);

        Debug.Log($"方向 : {direction} / distance : {distance} / count : {count}");
    }

    private void Test2()
    {
        Vector3 center = transform.position;
        var t = target.transform.position;
        var direction = center - t;
        var distance = Vector3.Distance(t, center);
        Physics.SphereCast(t, radius, direction, out hit, distance, layerMask);

        Debug.DrawRay(t, direction, Color.red, 0.2f);

        Debug.Log($"方向 : {direction} / distance : {distance}");
    }

    private void OnDrawGizmos()
    {
        // foreach (var hit in hits)
        // {
        //     if (hit.collider == null)
        //         continue;

        //     Gizmos.DrawSphere(hit.point, radius);
        // }

        if (hit.collider == null)
            return;

        Gizmos.DrawSphere(hit.point, radius);
    }
}
