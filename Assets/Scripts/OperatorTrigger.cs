using UnityEngine;

public class OperatorTrigger : MonoBehaviour
{
    public OperatorText operatorText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            operatorText.StartTyping();
        }
    }
}
