using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.Main.DifficultySelect;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentryTower
{
    [HarmonyPatch(typeof(RangeSupport.MutatorTower), nameof(RangeSupport.MutatorTower.Mutate))]
    internal static class RangeSupport_MutatorTower_Mutate
    {
        [HarmonyPrefix]
        private static bool Prefix(RangeSupport.MutatorTower __instance, Model model)
        {
            if (__instance.id == "SentrySharedRange")
            {
                model.GetDescendants<AttackModel>().ForEach(am =>
                {
                    am.AddBehavior(new TargetCloseSharedRangeModel("SharedRangeClose", false, true, model.Cast<TowerModel>().isSubTower, true));
                    am.AddBehavior(new TargetStrongSharedRangeModel("SharedRangeStrong", false, true, model.Cast<TowerModel>().isSubTower, true));
                    am.AddBehavior(new TargetLastSharedRangeModel("SharedRangeLast", false, true, model.Cast<TowerModel>().isSubTower, true));
                    am.AddBehavior(new TargetFirstSharedRangeModel("SharedRangeFirst", false, true, model.Cast<TowerModel>().isSubTower, true));
                });
            }
            return true;
        }
    }
}
