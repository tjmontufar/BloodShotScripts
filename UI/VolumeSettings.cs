using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    // 1. Referencia al Mezclador de Audio de Unity (Audio Mixer)
    public AudioMixer gameAudioMixer;

    // 2. Nombres de los parámetros expuestos en el Audio Mixer (DEBEN COINCIDIR)
    private const string MusicVolumeParam = "MusicVolume"; // Asegúrate de que este nombre sea exacto
    private const string SFXVolumeParam = "SFXVolume";     // Asegúrate de que este nombre sea exacto

    // Nombres para guardar la preferencia en PlayerPrefs
    private const string MusicKey = "MusicVolume";
    private const string SFXKey = "SFXVolume";

    // Sliders para cargar el valor al inicio
    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Cargar los valores guardados al inicio del juego
        LoadVolume();

        // Asignar los listeners de cambio a los sliders si no se hace en el Inspector
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }


    // Método llamado por el Slider de MÚSICA
    public void SetMusicVolume(float volume)
    {
        // Los Sliders van de 0 a 1. Necesitamos convertir ese valor a la escala de dB (decibelios).
        // Log10(0.0001) = -4. Log10(1) = 0. Esta función da un rango de -80dB (casi silencio) a 0dB (máximo).
        float dbVolume = Mathf.Log10(volume) * 20;

        // Establecer el parámetro en el Audio Mixer
        gameAudioMixer.SetFloat(MusicVolumeParam, dbVolume);
    }

    // Método llamado por el Slider de SFX (Sonido)
    public void SetSFXVolume(float volume)
    {
        float dbVolume = Mathf.Log10(volume) * 20;

        // Establecer el parámetro en el Audio Mixer
        gameAudioMixer.SetFloat(SFXVolumeParam, dbVolume);
    }

    // Método llamado al presionar el botón "GUARDAR CAMBIOS"
    public void SaveVolume()
    {
        // Guardar la posición actual de los sliders (que va de 0 a 1)
        if (musicSlider != null) PlayerPrefs.SetFloat(MusicKey, musicSlider.value);
        if (sfxSlider != null) PlayerPrefs.SetFloat(SFXKey, sfxSlider.value);

        PlayerPrefs.Save();
        Debug.Log("Configuración de volumen guardada.");
    }

    // Método para cargar la configuración al iniciar el juego o al abrir Opciones
    private void LoadVolume()
    {
        // 1. Cargar las preferencias guardadas (por defecto 0.5 o la mitad)
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicKey, 0.5f);
        float savedSFXVolume = PlayerPrefs.GetFloat(SFXKey, 0.5f);

        // 2. Establecer el valor en los Sliders de la UI
        if (musicSlider != null) musicSlider.value = savedMusicVolume;
        if (sfxSlider != null) sfxSlider.value = savedSFXVolume;

        // 3. Aplicar el volumen al Mixer al cargar la escena
        SetMusicVolume(savedMusicVolume);
        SetSFXVolume(savedSFXVolume);
    }
}