using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour
{


    //Кол/во секунд в дне и кол/во апдейтов времени и позиции в секунду
    public float SecondsInFullDay = 120f;
    public float UpdateRateInSeconds = 5f;


    //текущее время 0-1 
    [Range(0, 1)]
    public float CurrentTimeOfDay = 0.45f;

    //Множитель времени для эффектов ускорения времени
    public float TimeMultiplier = 1f;

    //трансформ солнца
    [Space(10)]
    public Transform SunTransform;

    //Цвета солнца и амбиента для шейдера
    public Gradient SunColor;
    public Gradient AmbientColor;

    //счетчик времени
    float _tempTime = 0;

    private void Start()
    {
#if UNITY_EDITOR
        UpdateShaderValues();
#endif
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            //Считаем время между апдейтами, если время пришло то делаем апдейт времени и пересчитываем угол солнца
            _tempTime = _tempTime + Time.deltaTime;
            if (_tempTime > UpdateRateInSeconds)
            {
                _tempTime = 0;
                UpdateTime();
                UpdatePosition();
                UpdateShaderValues();
            }
        }
    }



    void UpdateTime()
    {
        //Добавляем время в часы дня
        CurrentTimeOfDay += ((Time.deltaTime + UpdateRateInSeconds) / SecondsInFullDay) * TimeMultiplier;
        if (CurrentTimeOfDay >= 1) CurrentTimeOfDay = 0;
    }

    void UpdatePosition()
    {
        //Вращаем солнце
        SunTransform.localRotation = Quaternion.Euler((CurrentTimeOfDay * 360f) - 90, 170, 0);
    }

    void UpdateShaderValues()
    {
        //Применяем значения в шейдер
        Shader.SetGlobalVector("_SunDirection", -SunTransform.forward);
        Shader.SetGlobalColor("_SunColor", SunColor.Evaluate(CurrentTimeOfDay));
        Shader.SetGlobalColor("_SkyColor", AmbientColor.Evaluate(CurrentTimeOfDay));
    }
}