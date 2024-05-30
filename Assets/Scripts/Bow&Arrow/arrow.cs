using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float arrowSpeed = 2000.0f;
    public Transform arrowTip = null;

    private bool isInAir = false;
    private Vector3 lastPosition = Vector3.zero;

    private Rigidbody rigidBody = null;

    protected void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isInAir)
        {
            CheckForCollision();
            lastPosition = arrowTip.position;
        }
    }

    private void CheckForCollision()
    {
        if (Physics.Linecast(lastPosition, arrowTip.position))
            StopArrow();
    }

    private void StopArrow()
    {
        isInAir = false;
        SetPhysics(false);
    }

    public void ReleaseArrow(float pullValue)
    {
        isInAir = true;
        SetPhysics(true);

        FireArrow(pullValue);
        StartCoroutine(RotateWithVelocity());

        lastPosition = arrowTip.position;
    }

    private void SetPhysics(bool usePhysics)
    {
        rigidBody.isKinematic = !usePhysics;
        rigidBody.useGravity = usePhysics;
    }

    private void FireArrow(float power)
    {
        Vector3 force = transform.forward * (power * arrowSpeed);
        rigidBody.AddForce(force);
    }

    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();

        while (isInAir)
        {
            Quaternion newRotation = Quaternion.LookRotation(rigidBody.velocity, transform.up);
            transform.rotation = newRotation;
            yield return null;
        }
    }
}
