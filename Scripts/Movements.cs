using UnityEngine;

public class Movements : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento del sprite

    void Update()
    {
        // mover en derecha e izquierda al presionar las teclas de flecha derecha e izquierda
        float horizontal = Input.GetAxis("Horizontal");
        transform.Translate(Vector2.right * horizontal * moveSpeed * Time.deltaTime);


        // mover hacia arriba y abajo al presionar las teclas de flecha arriba y abajo
        float vertical = Input.GetAxis("Vertical");
        transform.Translate(Vector2.up * vertical * moveSpeed * Time.deltaTime);
    }
}
