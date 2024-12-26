using AmongUs.GameOptions;
using TownOfHost.Roles.Core;
using UnityEngine;

namespace TownOfHost.Roles.Imposter
{
    public sealed class Absorber : RoleBase
    {
        public static readonly SimpleRoleInfo RoleInfo =
            SimpleRoleInfo.Create(
                typeof(Absorber),
                player => new Absorber(player),
                CustomRoles.Absorber,
                () => RoleTypes.Imposter,
                CustomRoleTypes.Imposter,
                20000,
                null,
                "ab",
                "#800080"
            );
        
        public Absorber(PlayerControl player)
        : base(
            RoleInfo,
            player
        )
        { }

        public override void OnMurderPlayer(MurderInfo info)
        {
            var (killer, target) = info.AttemptTuple;
            if (killer.Is(CustomRoles.Absorber) && !info.IsSuicide)
            {
                AbsorbRole(target);
            }
        }

        private void AbsorbRole(PlayerControl target)
        {
            // Logic to absorb target player's role abilities
            RoleData targetRole = target.GetRole();
            bool isUsefulRole = true;

            // Useless roles for impostors (including crewmate, madmate, neutral, and specified vanilla roles)
            if (targetRole == CustomRoles.Doctor || targetRole == CustomRoles.Sheriff || targetRole == CustomRoles.Snitch ||
                targetRole == CustomRoles.TimeManager || targetRole == CustomRoles.Trapper ||
                targetRole == CustomRoles.Madmate || targetRole == CustomRoles.MadSnitch || targetRole == CustomRoles.MadGuardian ||
                targetRole == CustomRoles.Arsonist || targetRole == CustomRoles.Egoist || targetRole == CustomRoles.Executioner ||
                targetRole == CustomRoles.Jackal || targetRole == CustomRoles.Jester || targetRole == CustomRoles.Opportunist ||
                targetRole == CustomRoles.PlagueDoctor || targetRole == CustomRoles.SchrodingerCat || targetRole == CustomRoles.Terrorist ||
                targetRole == RoleTypes.Crewmate || targetRole == RoleTypes.GuardianAngel || targetRole == RoleTypes.Scientist || 
                targetRole == RoleTypes.Noisemaker ||
                // Prevent absorbing imposter roles
                targetRole == RoleTypes.Impostor || targetRole == CustomRoles.BountyHunter || targetRole == CustomRoles.Penguin ||
                targetRole == CustomRoles.Sniper || targetRole == CustomRoles.Vampire || targetRole == CustomRoles.Witch)
            {
                isUsefulRole = false;
            }

            if (isUsefulRole)
            {
                // Absorb useful role abilities
                if (targetRole == CustomRoles.Bait)
                {
                    this.Player.AddAbility("Report Body");
                    Debug.Log("Absorber has gained Bait's reporting ability.");
                }
                else if (targetRole == CustomRoles.Dictator)
                {
                    this.Player.AddAbility("Modify Vote");
                    Debug.Log("Absorber has gained Dictator's voting power.");
                }
                else if (targetRole == CustomRoles.Lighter)
                {
                    this.Player.AddAbility("Enhanced Vision");
                    Debug.Log("Absorber has gained Lighter's vision abilities.");
                }
                else if (targetRole == CustomRoles.Mayor)
                {
                    this.Player.AddAbility("Additional Vote");
                    this.Player.AddAbility("Portable Button");
                    Debug.Log("Absorber has gained Mayor's additional vote power and abilities.");
                }
                else if (targetRole == CustomRoles.SabotageMaster)
                {
                    this.Player.AddAbility("Fix Sabotages");
                    Debug.Log("Absorber has gained SabotageMaster's sabotage fixing abilities.");
                }
                else if (targetRole == CustomRoles.Seer)
                {
                    this.Player.AddAbility("See Kill Flashes");
                    Debug.Log("Absorber has gained Seer's kill flash seeing ability.");
                }
                else if (targetRole == CustomRoles.SpeedBooster)
                {
                    this.Player.AddAbility("Speed Boost");
                    Debug.Log("Absorber has gained SpeedBooster's speed boosting ability.");
                }
                else if (targetRole == RoleTypes.Tracker)
                {
                    this.Player.AddAbility("Track Players");
                    Debug.Log("Absorber has gained Tracker's tracking ability.");
                }
            }
            else
            {
                // Display "Useless Role!" message
                ShowMessage("Useless Role!", new Color(0.8f, 0.1f, 0.1f)); // reddish color
            }
        }

        private void ShowMessage(string message, Color color)
        {
            // Logic to display message only to Absorber
            this.Player.RpcSetNameColor(color);
            Debug.Log(message);
        }
    }
}
