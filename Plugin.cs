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

/* EFT existstatus enum as reference
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
    // Exit result enum but without the transit entry, since that is for a different mechanic
    // This should match the order and int assignements for the origional ExitStatus enum, this makes it easier to cast and will also cause problems otherwise
    public enum ExitStatusFilter
    {
        Survived = 0,
        Killed = 1,
        Left = 2,
        Runner = 3,
        MissingInAction = 4
    }

    // Define the plugin info, this is necessary for bepinex plugins
    [BepInEx.BepInPlugin("com.crocodilejonesy.configraidstatus", "Configurable Raid Status", "1.0.0")]
    public class EndRaidPlugin : BaseUnityPlugin
    {
        // bool to allow control over the setting for the survived end raid status, as a safety precaution
        internal static ConfigEntry<bool> m_survivedEnabled;

        // Options for each of the relevant exit statuses (Except transit)
        internal static ConfigEntry<ExitStatusFilter> survivedStatus, killedStatus, leftStatus, runnerStatus, miaStatus;

        // Const strings for ease of use, numbers are added since the plugin order the categories based on alphabet
        private const string m_CategoryRaidSucess = "1.Sucessful Raid Status Options";
        private const string m_CategoryRaidFailed = "2.Failed Raid Status Options";

        private void Awake()
        {
            // NOTE: Order property values go in descending order, so largest order values get put first and lower gets put later
            // Misc options
            m_survivedEnabled = Config.Bind(m_CategoryRaidSucess, "Enable survived status setting", false,
                new ConfigDescription("Applies the survived status setting when extracting, this is mainly a safety feature to prevent accidental mia after extracting or something", null, new ConfigurationManagerAttributes { Order = 6 }));

            // Options for when extracting
            survivedStatus = Config.Bind(m_CategoryRaidSucess, "Survived Raid Status", ExitStatusFilter.Survived,
                new ConfigDescription("Sets the raid status when you extract", null, new ConfigurationManagerAttributes { Order = 5 }));
            runnerStatus = Config.Bind(m_CategoryRaidSucess, "Runner Raid Status", ExitStatusFilter.Runner,
                new ConfigDescription("Sets the raid status when you get a \"Run Through\" (Extract too early)", null, new ConfigurationManagerAttributes { Order = 4 }));

            // Options for when not extracted
            killedStatus = Config.Bind(m_CategoryRaidFailed, "Killed Raid Status", ExitStatusFilter.Killed, 
                new ConfigDescription("Sets the raid status when the character is killed", null, new ConfigurationManagerAttributes { Order = 3 }));
            leftStatus = Config.Bind(m_CategoryRaidFailed, "Left The Action Raid Status", ExitStatusFilter.Left, 
                new ConfigDescription("Sets the raid status when you quit through the menu", null, new ConfigurationManagerAttributes { Order = 2 }));
            miaStatus = Config.Bind(m_CategoryRaidFailed, "Missing in Action Raid Status", ExitStatusFilter.MissingInAction, 
                new ConfigDescription("Sets the raid status when the raid timer runs out", null, new ConfigurationManagerAttributes { Order = 1 }));

            //// Initialise harmony and begin patching, otherwise patches won't work, this would make sense with several patch classes but we only have one so no need
            //var harmony = new Harmony("com.crocodilejonesy.configraidstatus");
            //harmony.PatchAll();

            // Enable patching, otherwise code won't run, this is essentially thc calling of the patching code
            // NOTE: Make sure this is run AFTER the rest of the awake code otherwise no changes will apply
            new EndRaidPatch().Enable();
        }

    }
}

