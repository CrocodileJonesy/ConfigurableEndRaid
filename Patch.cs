using BepInEx.Logging;
using EFT;
using EFT.UI.SessionEnd;
using EFT.Weather;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurableEndRaidStatus
{
    // Get the target method of EFT
    [HarmonyPatch(typeof(BaseLocalGame<EftGamePlayerOwner>), nameof(BaseLocalGame<EftGamePlayerOwner>.Stop))]
    public class EndRaidPatch : ModulePatch
    {
#if DEBUG
        private static ManualLogSource logSource = BepInEx.Logging.Logger.CreateLogSource("EndRaidPatch");
#endif

        // Get the method to patch
        protected override MethodBase GetTargetMethod()
        {
            return typeof(BaseLocalGame<EftGamePlayerOwner>).GetMethod("Stop", BindingFlags.Public | BindingFlags.Instance);
        }

        // Method for patching and changing the exit status result of a raid
        [PatchPrefix]
        public static bool PatchPrefix(ref ExitStatus exitStatus)
        {

            // Check the exit status for result and set based what user sets
            switch (exitStatus)
            {
                case ExitStatus.Survived:
                {
                    // Check to make sure setting to apply changes for survived are enabled
                    if (EndRaidPlugin.m_survivedEnabled.Value)
                    {
                        exitStatus = (ExitStatus)EndRaidPlugin.survivedStatus.Value;
                    }
                    break;
                }

                case ExitStatus.Killed:
                    exitStatus = (ExitStatus)EndRaidPlugin.killedStatus.Value;
                    break;

                case ExitStatus.Left:
                    exitStatus = (ExitStatus)EndRaidPlugin.leftStatus.Value;
                    break;

                case ExitStatus.Runner:
                    exitStatus = (ExitStatus)EndRaidPlugin.runnerStatus.Value;
                    break;

                case ExitStatus.MissingInAction:
                    exitStatus = (ExitStatus)EndRaidPlugin.miaStatus.Value;
                    break;

                // No need to worry about transit so do nothing
                case ExitStatus.Transit:
                    break;
            }
#if DEBUG
            Logger.LogInfo("Sucessfully patched");
#endif   
            // Allow the rest of the method to run since we just want to change the exit status
            return true; 
        }
    }
}
