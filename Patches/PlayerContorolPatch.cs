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
            if(target.Data.Role.Role == RoleTypes.Scientist && main.currentScientist == ScientistRole.Bait && AmongUsClient.Instance.AmHost
            && __instance.PlayerId != target.PlayerId) {
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
            if(!AmongUsClient.Instance.AmHost) return false;
            if(main.isSidekick(__instance)) {
                var ImpostorCount = 0;
                foreach(var pc in PlayerControl.AllPlayerControls) {
                    if(pc.Data.Role.Role == RoleTypes.Impostor &&
                       !pc.Data.IsDead) ImpostorCount++;
                }
                if(ImpostorCount > 0) return false;
            }
            if(main.isVampire(__instance)) { //キルキャンセル&自爆処理
                __instance.RpcProtectPlayer(__instance, 0);
                __instance.RpcMurderPlayer(__instance);
                main.BitPlayers.Add(target.PlayerId, (__instance.PlayerId, 0f));
                return false;
            }

            __instance.RpcMurderPlayer(target);
            if(main.isFixedCooldown) {
                __instance.RpcProtectPlayer(target,0);
                __instance.RpcMurderPlayer(target);
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportDeadBody))]
    class ReportDeadBodyPatch {
        public static bool Prefix(PlayerControl __instance) {
            if(main.IsHideAndSeek) return false;
            if(AmongUsClient.Instance.AmHost) {
                foreach(var bp in main.BitPlayers) {
                    foreach(var pc in PlayerControl.AllPlayerControls) {
                        if(bp.Key == pc.PlayerId)
                            pc.RpcMurderPlayer(pc);
                    }
                    main.PlaySoundRPC(bp.Value.Item1, Sounds.KillSound);
                }
            }
            main.BitPlayers = new Dictionary<byte, (byte, float)>();
            return true;
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    class FixedUpdatePatch {
        public static void Postfix(PlayerControl __instance) {
            if(AmongUsClient.Instance.AmHost) {
                if(main.BitPlayers.ContainsKey(__instance.PlayerId)) {
                    if(main.BitPlayers[__instance.PlayerId].Item2 >= 10) {
                        __instance.RpcMurderPlayer(__instance);
                        main.PlaySoundRPC(main.BitPlayers[__instance.PlayerId].Item1,Sounds.KillSound);
                        if(main.isBait(__instance)) {
                            foreach(var vampire in PlayerControl.AllPlayerControls) {
                                vampire.CmdReportDeadBody(__instance.Data);
                            }
                        }
                        main.BitPlayers.Remove(__instance.PlayerId);
                    } else {
                        main.BitPlayers[__instance.PlayerId] = 
                        (main.BitPlayers[__instance.PlayerId].Item1, main.BitPlayers[__instance.PlayerId].Item2 + Time.fixedDeltaTime);
                    }
                }
            }
        }
    }
}