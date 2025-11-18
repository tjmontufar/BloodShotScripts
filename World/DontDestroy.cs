using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static DontDestroy instance;

    void Awake()
    {
        // Implementación de Singleton para el Audio de Música
        if (instance == null)
        {
            instance = this;
            // ¡Esto es crucial! Mantiene el objeto vivo a través de las cargas de escena.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe una instancia de música, destruye esta nueva.
            Destroy(gameObject);
        }
    }
}