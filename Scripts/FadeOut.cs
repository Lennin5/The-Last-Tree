using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public float fadeDuration = 0.2f; // Duración de la desaparición gradual
    public Image image;
    public Rigidbody2D rb;

    void Start()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("No se ha encontrado ningún componente de imagen en el objeto.");
            return;
        }

        // Inicia la desaparición gradual con un lapso de tiempo aleatorio
        StartCoroutine(StartFadeOutWithDelay());

    }

    IEnumerator StartFadeOutWithDelay()
    {
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        StartCoroutine(FadeOutCoroutine());
    }


    IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0f;
        // almacenar la posición inicial
        int initialX = (int)image.rectTransform.position.x;
        int initialY = (int)image.rectTransform.position.y;
        Color initialColor = image.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f); // Alfa = 0

        rb.gravityScale = 10f;


        while (elapsedTime < fadeDuration)
        {
            // Interpola gradualmente el color del objeto Image
            image.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegúrate de que el color final sea el color de destino exacto
        image.color = targetColor;

        // Desactiva el objeto después de desaparecer completamente (opcional)
        //gameObject.SetActive(false);

        // Reiniciar la opacidad
        image.color = initialColor;

        // obtener un initialXRandom en base de un rango de valores basado en la resolución de la pantalla en X
        int initialXRandom = Random.Range(0, Screen.width);

        // Reiniciar la posición de la imagen
        image.rectTransform.position = new Vector3(initialXRandom, initialY, image.rectTransform.position.z);

        // Reiniciar la gravedad
        rb.gravityScale = 0f;

        // Detener el movimiento del objeto
        rb.velocity = Vector2.zero;

        // Volver a iniciar la desaparición gradual en un lapso de tiempo aleatorio
        yield return new WaitForSeconds(Random.Range(0.1f, 0.9f));
        StartCoroutine(FadeOutCoroutine());
        
    }

}
