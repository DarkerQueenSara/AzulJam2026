using System.Collections;
using System.Linq;
using UnityEngine;
using BuzzControllerSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using static WitchGameplay;

[RequireComponent(typeof(WitchInput))]
public class TitleScreenManager : MonoBehaviour
{
    private async Awaitable Start()
    {
        StartCoroutine(BuzzHypeCoroutine());
        await GetComponent<WitchInput>().onConfirmation;
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private static readonly YieldInstruction Wait = new WaitForSeconds(0.25f);
    
    private static IEnumerator BuzzHypeCoroutine()
    {
        var device = BuzzInputDevice.current;
        if (device == null) yield break;

        for (var p = 0; /* forever */; p = (p + 1) % PlayerCount)
        {
            var lights = BuzzOutputReport.Create(p == 0, p == 1, p == 2, p == 3);
            device.ExecuteCommand(ref lights);
            yield return Wait;
        }
    }
}