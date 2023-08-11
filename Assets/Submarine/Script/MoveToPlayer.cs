using UnityEngine;
using System.Collections;

public class MoveToPlayer : MonoBehaviour
{
    public float speed = 10;

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, GameManager.Instance.Player.transform.position,
            speed * Time.deltaTime);
    }
}