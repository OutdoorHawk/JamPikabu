using System.Collections;
using UnityEngine;

namespace Code.Infrastructure.Common.CoroutineRunner
{
    /// <summary>
    /// Used to run coroutines from non MonoBehavior classes
    /// </summary>
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(IEnumerator coroutine);
    }
}