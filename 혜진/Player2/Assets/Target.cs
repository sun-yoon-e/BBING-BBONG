using UnityEngine;

public class Target : MonoBehaviour
{
    public void Hit()
    {
        Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
