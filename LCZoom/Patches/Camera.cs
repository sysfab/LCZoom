using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LCZoom.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class CameraPatch
    {
        private static ConfigEntry<float> NormalFov = ZoomBase.NormalFovMultiplier;
        private static ConfigEntry<float> ZoomFov = ZoomBase.ZoomFovMultiplier;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void patchUpdate(ref float ___targetFOV, ref Camera ___gameplayCamera,
                                ref bool ___inTerminalMenu,
                                ref QuickMenuManager ___quickMenuManager,
                                ref bool ___isTypingChat,
                                ref bool ___disableMoveInput,
                                ref bool ___inSpecialInteractAnimation,
                                ref bool ___isClimbingLadder,
                                ref bool ___inShockingMinigame)
        {
            if (___inTerminalMenu || ___quickMenuManager.isMenuOpen || ___isTypingChat || ___disableMoveInput || ___inSpecialInteractAnimation && ___isClimbingLadder && ___inShockingMinigame) { return; }

            bool isZooming = ZoomBase.InputInstance.ZoomKey.IsPressed();

            // Hold “C” to zoom  
            float fovModifier = isZooming ? ZoomFov.Value : NormalFov.Value;

            ___gameplayCamera.fieldOfView = Mathf.Lerp(___gameplayCamera.fieldOfView, ___targetFOV * fovModifier, 6f * Time.deltaTime);
        }
    }
}
