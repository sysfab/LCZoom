using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LCZoom.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class CameraPatch
    {
        internal static bool isZooming = false;
        internal static int zoomTicks = 0;
        internal static float ScrollZoomLevel = 0f;

        private static ConfigEntry<float> NormalFov = ZoomBase.NormalFovMultiplier;
        private static ConfigEntry<float> ZoomFov = ZoomBase.ZoomFovMultiplier;
        private static ConfigEntry<bool> EnableScrollZoom = ZoomBase.EnableScrollZoom;
        private static ConfigEntry<float> ScrollZoomSpeed = ZoomBase.ScrollZoomSpeed;

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        static void patchAwake()
        {
            CameraPatch.isZooming = false;
            CameraPatch.zoomTicks = 0;
            CameraPatch.ScrollZoomLevel = 0.5f;
        }

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
            if (___inTerminalMenu || ___quickMenuManager.isMenuOpen || ___isTypingChat || ___disableMoveInput || ___inSpecialInteractAnimation && ___isClimbingLadder && ___inShockingMinigame) { isZooming = false;  return; }

            CameraPatch.isZooming = ZoomBase.InputInstance.ZoomKey.IsPressed();

            if (isZooming)
            {
                if (CameraPatch.zoomTicks == 0) {
                    CameraPatch.ScrollZoomLevel = EnableScrollZoom.Value ? 0.5f : 1f;
                }
                CameraPatch.zoomTicks++;
            } else
            {
                CameraPatch.zoomTicks = 0;
            }

            // Hold “C” to zoom  
            float fovModifier = CameraPatch.isZooming ? Mathf.Lerp(NormalFov.Value, ZoomFov.Value, CameraPatch.ScrollZoomLevel) : NormalFov.Value;

            ___gameplayCamera.fieldOfView = Mathf.Lerp(___gameplayCamera.fieldOfView, ___targetFOV * fovModifier, 6f * Time.deltaTime);
        }

        [HarmonyPatch("ScrollMouse_performed")]
        [HarmonyPrefix]
        static void patchScroll(ref InputAction.CallbackContext context, ref float ___timeSinceSwitchingSlots)
        {
            if (!EnableScrollZoom.Value) return;

            float num = context.ReadValue<float>();
            float sign = Mathf.Sign(num);

            if (CameraPatch.isZooming)
            {
                CameraPatch.ScrollZoomLevel = Math.Max(0f, Math.Min(1f, CameraPatch.ScrollZoomLevel + (0.05f * sign)));
                ___timeSinceSwitchingSlots = 0f;
                return;
            }
        }
    }
}
