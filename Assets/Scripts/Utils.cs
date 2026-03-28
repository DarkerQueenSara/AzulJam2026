using System;
using System.Collections.Generic;
using BuzzControllerSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;

public static class Utils
{
    public static async Awaitable<T> SubscribeUntil<T>(UnityEvent<T> unityEvent, Predicate<T> predicate)
    {
        T last;
        do
        {
            last = await unityEvent;
        } while (!predicate(last));

        return last;
    }
    
    public static async Awaitable<T> SubscribeUntil<T>(UnityEvent<T> unityEvent, T sentinel)
    {
        T last;
        do
        {
            last = await unityEvent;
        } while (!EqualityComparer<T>.Default.Equals(last, sentinel));

        return last;
    }

    public static async IAsyncEnumerable<T> StreamAsync<T>(this UnityEvent<T> unityEvent)
    {
        for (;;)
        {
            yield return await  unityEvent;
        }
        // ReSharper disable once IteratorNeverReturns
    }

    public static async Awaitable UntilAsync(Func<bool> predicate)
    {
        while (!predicate())
            await Awaitable.NextFrameAsync();
    }

    public static void TurnOffAllBuzzLights()
    {
        var lightsOff = BuzzOutputReport.Create(false, false, false, false);
        BuzzInputDevice.current.ExecuteCommand(ref lightsOff);
    }
}