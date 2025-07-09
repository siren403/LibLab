using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Floor"))
        {
            GetComponent<Rigidbody>().MovePosition(respawnPoint.transform.position);
        }
    }
}
