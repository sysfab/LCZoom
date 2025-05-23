﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LCZoom.Patches;
using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using UnityEngine.InputSystem;

namespace LCZoom
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils",
                     BepInDependency.DependencyFlags.HardDependency)]
    public class ZoomBase : BaseUnityPlugin
    {
        private const string modGUID = "sysfab.lczoom";
        private const string modName = "LC Zoom";
        private const string modVersion = "1.1.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ZoomBase Instance;
        public static ZoomInputClass InputInstance;

        internal static ConfigEntry<float> NormalFovMultiplier;
        internal static ConfigEntry<float> ZoomFovMultiplier;
        internal static ConfigEntry<bool> EnableScrollZoom;
        internal static ConfigEntry<float> ScrollZoomSpeed;

        internal ManualLogSource Log;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InputInstance = new ZoomInputClass();
            }

            NormalFovMultiplier = Config.Bind("Settings", "NormalFovMultiplier", 1f,
                "Normal FOV multiplier. Default is 1.0");
            ZoomFovMultiplier = Config.Bind("Settings", "ZoomFovMultiplier", 0.001f,
                "Zoom FOV multiplier. Default is 0.001");
            EnableScrollZoom = Config.Bind("Controls", "EnableScrollZoom", true, 
                "Enable scroll zoom");
            ScrollZoomSpeed = Config.Bind("Controls", "ScrollZoomSpeed", 0.05f,
                "Scroll Zoom Speed. Default is 0.05");

            Log = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            Log.LogInfo($"{modName} is loaded!");

            harmony.PatchAll(typeof(ZoomBase));
            harmony.PatchAll(typeof(CameraPatch));
        }
    }

    public class ZoomInputClass : LcInputActions
    {
        [InputAction(KeyboardControl.C, Name = "Zoom")]
        public InputAction ZoomKey { get; set; }
    }
}
