using UnityEngine;
using UnityEngine.UI;

public class MoveBubbles : MonoBehaviour
{
    public float speed = 100f; // Velocidad de movimiento
    public RectTransform[] bubbleImages; // Array de las imágenes de las burbujas

    private Vector3[] initialPositions; // Posiciones iniciales de las imágenes
    private float imageWidth; // Ancho de la imagen

    void Start()
    {
        initialPositions = new Vector3[bubbleImages.Length];
        for (int i = 0; i < bubbleImages.Length; i++)
        {
            initialPositions[i] = bubbleImages[i].anchoredPosition;
        }

        // Calcula el ancho de la imagen
        imageWidth = bubbleImages[0].rect.width;
    }

    void Update()
    {
        // Movimiento de las imágenes
        for (int i = 0; i < bubbleImages.Length; i++)
        {
            bubbleImages[i].anchoredPosition += Vector2.right * speed * Time.deltaTime;

            // Si la imagen sale de la vista
            if (bubbleImages[i].anchoredPosition.x > Screen.width + imageWidth / 2f)
            {
                // Reposiciona la imagen al final de las imágenes a la izquierda
                bubbleImages[i].anchoredPosition = bubbleImages[(i + bubbleImages.Length - 1) % bubbleImages.Length].anchoredPosition - Vector2.right * imageWidth;
            }
        }
    }
}
