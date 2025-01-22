using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;
using UnityEngine;

/*
 *  enum ExitStatus
    {
        Survived = 0,
        Killed = 1,
        Left = 2,
        Runner = 3,
        MissingInAction = 4,
        Transit = 5
    }
 * 
 */


namespace ConfigurableEndRaidStatus
{
    [BepInEx.BepInPlugin("com.crocodilejonesy.configraidstatus", "Configurable Raid Status", "0.9.8")]
    public class EndRaidPlugin : BaseUnityPlugin
    {
        // Options for each of the relevant exit statuses (Except transit)
        internal static ConfigEntry<ExitStatus> survivedStatus, killedStatus, leftStatus, runnerStatus, miaStatus;

        // Const strings for ease of use
        private const string m_CategoryRaidSucess = "Sucessful Raid Status Options (NOT IMPLEMENTED)";
        private const string m_CategoryRaidFailed = "Failed Raid Status Options";

        private void Awake()
        {
            // TODO: Survived
            survivedStatus = Config.Bind(m_CategoryRaidSucess, "Survived Raid Status", ExitStatus.Survived, "Sets the raid status when you extract");
            runnerStatus = Config.Bind(m_CategoryRaidSucess, "Runner Raid Status", ExitStatus.Runner, "Sets the raid status when you get a \"Run Through\" (Extract too early)");
            // TODO: runner

            killedStatus = Config.Bind(m_CategoryRaidFailed, "Killed Raid Status", ExitStatus.Killed, "Sets the raid status when the character is killed");
            leftStatus = Config.Bind(m_CategoryRaidFailed, "Left The Action Raid Status", ExitStatus.Left, "Sets the raid status when you quit through the menu");
            miaStatus = Config.Bind(m_CategoryRaidFailed, "Missing in Action Raid Status", ExitStatus.MissingInAction, "Sets the raid status when the raid timer runs out");

            //// Initialise harmony and begin patching, otherwise patches won't work
            //var harmony = new Harmony("com.crocodilejonesy.configraidstatus");
            //harmony.PatchAll();

            // Enable patching, otherwise code won't run, this is essentially thc calling of the patching code
            // NOTE: Make sure this is run AFTER the rest of the awake code otherwise no changes will apply
            new EndRaidPatch().Enable();
        }

    }
}

