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
    //public class LocalGame : BaseLocalGame<EftGamePlayerOwner>, IBotGame
    //{
    //    public BotsController BotsController => throw new NotImplementedException();
    //    public IWeatherCurve WeatherCurve => throw new NotImplementedException();
    //    public BossSpawnScenario BossSpawnScenario => throw new NotImplementedException();


    //    public override void Stop(string profileId, ExitStatus exitStatus, string exitName, float delay = 0f)
    //    {


    //        base.Stop(profileId, newExitStatus, exitName, delay);
    //    }

    //    public override IEnumerator vmethod_2()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    //[HarmonyPatch]
    //public class LocalGamePatch
    //{
    //    private static LocalGame
    //}

    [HarmonyPatch(typeof(BaseLocalGame<EftGamePlayerOwner>), nameof(BaseLocalGame<EftGamePlayerOwner>.Stop))]
    public class EndRaidPatch : ModulePatch
    {
        private static ManualLogSource logSource = BepInEx.Logging.Logger.CreateLogSource("EndRaidPatch");

        protected override MethodBase GetTargetMethod()
        {
            return typeof(BaseLocalGame<EftGamePlayerOwner>).GetMethod("Stop", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        public static bool PatchPrefix(ref string profileId, ref ExitStatus exitStatus, ref string exitName, ref float delay)
        {

            // Check the exit status for result and set based what user sets
            switch (exitStatus)
            {
                case ExitStatus.Survived:
                    exitStatus = EndRaidPlugin.survivedStatus.Value;
                    break;

                case ExitStatus.Killed:
                    exitStatus = EndRaidPlugin.killedStatus.Value;
                    break;

                case ExitStatus.Left:
                    exitStatus = EndRaidPlugin.leftStatus.Value;
                    break;

                case ExitStatus.Runner:
                    exitStatus = EndRaidPlugin.runnerStatus.Value;
                    break;

                case ExitStatus.MissingInAction:
                    exitStatus = EndRaidPlugin.miaStatus.Value;
                    break;

                // No need to worry about transit so do nothing
                case ExitStatus.Transit:
                    break;
            }

            Logger.LogInfo("Sucessfully patched");
            return true; // Allow the rest of the method to run
        }
    }

    //internal class SessionResultExtPatch : ModulePatch
    //{
    //    protected override MethodBase GetTargetMethod()
    //    {
    //        return typeof(SessionResultExitStatus).GetMethod("Show", BindingFlags.Public | BindingFlags.Instance);
    //    }

    //    public static void PatchPreFix(Profile activeProfile, GClass1917 lastPlayerState, ESideType side, ref ExitStatus exitStatus, TimeSpan raidTime, ISession session, bool isOnline)
    //    {
    //        // Testing
    //        if (exitStatus == ExitStatus.Survived)
    //        {
    //            exitStatus = ExitStatus.Runner;
    //            Logger.LogInfo("Set exit status to runner (for testing)");

    //        }

    //        // Testing
    //        if (exitStatus == ExitStatus.Left)
    //        {
    //            exitStatus = ExitStatus.Survived;
    //        }

    //        // Set the exit status of the game to not be MIA
    //        if (exitStatus == ExitStatus.MissingInAction)
    //        {
    //            exitStatus = ExitStatus.Runner;
    //            Logger.LogInfo("Set MIA status to runner");
    //        }
    //    }
    //}
}
