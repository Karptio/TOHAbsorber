using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using Hazel;
using System;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnhollowerBaseLib;
using TownOfHost;
using System.Threading.Tasks;
using System.Threading;

namespace TownOfHost {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    class MurderPlayerPatch {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)]PlayerControl target) {
            if(!target.Data.IsDead)
                return;
            //When Bait is killed
            if(target.Data.Role.Role == RoleTypes.Scientist && main.currentScientist == ScientistRole.Bait && AmongUsClient.Instance.AmHost) {
                Thread.Sleep(150);
                __instance.CmdReportDeadBody(target.Data);
            } else
            //Terrorist
            if(main.isTerrorist(target)) {
                main.CheckTerroristWin(target.Data);
            }
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckMurder))]
    class CheckMurderPatch {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)]PlayerControl target) {
            __instance.RpcMurderPlayer(target);
            return false;
        }
    }
}