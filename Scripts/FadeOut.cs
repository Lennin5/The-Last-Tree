using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public float fadeDuration = 2f; // Duración de la desaparición gradual
    public Image image;

    void Start()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("No se ha encontrado ningún componente de imagen en el objeto.");
            return;
        }

        // Inicia la desaparición gradual
        StartCoroutine(FadeOutCoroutine());

        // Inicia la desaparición gradual con un retraso de 2 segundos

    }

    IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0f;
        Color initialColor = image.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f); // Alfa = 0

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
        gameObject.SetActive(false);
    }
}
