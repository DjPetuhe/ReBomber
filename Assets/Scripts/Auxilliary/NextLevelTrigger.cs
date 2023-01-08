using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    private bool _activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_activated)
        {
            _activated = true;
            LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
            StartCoroutine(levelManager.LoadToNextLevel());
        }
    }
}
