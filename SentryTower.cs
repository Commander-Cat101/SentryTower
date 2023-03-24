using MelonLoader;
using BTD_Mod_Helper;
using SentryTower;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using System.Linq;
using Harmony;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Simulation.Behaviors;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using static SentryTower.MiniSentries;
using Il2CppSystem.Runtime.InteropServices;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Utils;
using Il2CppAssets.Scripts.Unity.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Unity.Towers.Filters;
using BTD_Mod_Helper.Api.Helpers;

[assembly: MelonInfo(typeof(SentryTower.SentryTower), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace SentryTower;

public class SentryTower : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<SentryTower>("SentryTower loaded!");
    }
}
public class SharedRangeIcon : ModBuffIcon
{
    public override string Icon => VanillaSprites.SentryPortrait;
    public override int MaxStackSize => 1;
    protected override int Order => 1;
}
public class Sentry : ModTower
{
    protected override int Order => 10;
    public override TowerSet TowerSet => TowerSet.Support;
    public override string BaseTower => TowerType.Sentry;
    public override int Cost => 650;
    public override int TopPathUpgrades => 5;
    public override int MiddlePathUpgrades => 5;
    public override int BottomPathUpgrades => 5;
    public override string Description => "The Engineer Monkeys Sentry";
    public override string Icon => VanillaSprites.SentryPortrait;

    public override string Portrait => VanillaSprites.SentryPortrait;

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        towerModel.isSubTower = false;

        towerModel.RemoveBehavior<TowerExpireModel>();
        towerModel.RemoveBehavior<SavedSubTowerModel>();

        towerModel.GetAttackModel().GetBehavior<TargetFirstModel>().isOnSubTower = false;
        towerModel.GetAttackModel().GetBehavior<TargetStrongModel>().isOnSubTower = false;


        towerModel.GetAttackModel().RemoveBehavior<TargetCloseModel>();
        towerModel.GetAttackModel().RemoveBehavior<TargetLastModel>();

        towerModel.footprint.doesntBlockTowerPlacement = false;
        towerModel.GetDescendant<CircleFootprintModel>().radius = 2;
    }
}
public class InfraredTargeting : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;
    public override int Path => TOP;
    public override int Tier => 1;
    public override int Cost => 250;


    public override string Description => "Allows the sentry to target camo bloons, also adds camo priority targeting";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.AddBehavior(new OverrideCamoDetectionModel("CamoDetect", true));

        towerModel.GetAttackModel().AddBehavior(new TargetStrongPrioCamoModel("StrongPrioCamo", true, false));
        towerModel.GetAttackModel().AddBehavior(new TargetFirstPrioCamoModel("FirstPrioCamo", true, false));

        towerModel.towerSelectionMenuThemeId = "Camo";

        towerModel.GetAttackModel().weapons[0].projectile.pierce += 1;
    }
}
public class FasterIdentification : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;
    public override int Path => TOP;
    public override int Tier => 2;
    public override int Cost => 500;


    public override string Description => "Faster cameras allow the sentry to shoot at quicker speeds";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetAttackModel().weapons[0].rate *= 0.6f;
    }
}

public class TrackingNails : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;
    public override int Path => TOP;
    public override int Tier => 3;
    public override int Cost => 1100;


    public override string Description => "Nails slightly home in on bloons";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(Game.instance.model.GetTower(TowerType.MonkeySub).GetAttackModel().weapons[0].projectile.GetBehavior<TrackTargetModel>());
        towerModel.GetAttackModel().weapons[0].projectile.pierce += 1;
    }
}

public class SmartTargeting : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryEnergyPortrait;
    public override int Path => TOP;
    public override int Tier => 4;
    public override int Cost => 2200;


    public override string Description => "Sentry can now shoot bloons that are visible by other monkeys, also increases damage and pierce";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetAttackModel().AddBehavior(new TargetStrongSharedRangeModel("TargetStrongShared", false, true, false, true));
        towerModel.GetAttackModel().AddBehavior(new TargetFirstSharedRangeModel("TargetFirstShared", false, true, false, true));

        towerModel.GetAttackModel().weapons[0].projectile.pierce += 3;
        towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 1;

        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().Lifespan *= 2;

        towerModel.display = new() { guidRef = "af6bc5f76310fa84eae188f2f5381dc6" };
        towerModel.GetAttackModel().behaviors.First(a => a.name == "DisplayModel_AttackDisplay").Cast<DisplayModel>().display = new() { guidRef = "5bb9342d838c0d848ab4ccb4f078114f" };
    }
}

public class SharedCameras : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryEnergyPortrait;
    public override int Path => TOP;
    public override int Tier => 5;
    public override int Cost => 6500;


    public override string Description => "All nearby towers gain shared targeting, also main sentry gains global range";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var boost = new RangeSupportModel("RangeSupportShared", true, 1.1f, 0, "SentrySharedRange", null, false, null, null);
        boost.ApplyBuffIcon<SharedRangeIcon>();
        towerModel.AddBehavior(boost);

        towerModel.isGlobalRange = true;
        towerModel.GetAttackModel().range = 999;

        towerModel.GetAttackModel().weapons[0].projectile.pierce += 5;
        towerModel.GetAttackModel().weapons[0].rate *= .3f;
        towerModel.GetAttackModel().range *= 1.3f;
        towerModel.display = new() { guidRef = "af6bc5f76310fa84eae188f2f5381dc6" };
        towerModel.GetAttackModel().behaviors.First(a => a.name == "DisplayModel_AttackDisplay").Cast<DisplayModel>().display = new() { guidRef = "4caf2f2c8975c94419872e1d66fbbc35" };
    }
}
public class ShredingNails : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;
    public override int Path => MIDDLE;
    public override int Tier => 1;
    public override int Cost => 400;


    public override string Description => "Sentry gains damage, aswell as the ability to pop frozen bloons";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 1;
        towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = (Il2Cpp.BloonProperties)1;
    }
}
public class HeatedNails : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;
    public override int Path => MIDDLE;
    public override int Tier => 2;
    public override int Cost => 600;


    public override string Description => "Heated nails give more damage and deal five times more damage to ceramics";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 1;
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("CeramicBloonDamageMultiplier", "Ceramic", 5, 0, false, false));
        towerModel.GetAttackModel().weapons[0].projectile.display = Game.instance.model.GetTower(TowerType.TackShooter, 3, 0, 0).GetAttackModel().weapons[0].projectile.display;
    }
}
public class ExplosiveTipped : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;

    public override int Path => MIDDLE;
    public override int Tier => 3;
    public override int Cost => 1800;


    public override string Description => "Projectiles now explode on impact";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var boom = Game.instance.model.GetTower(TowerType.BombShooter).GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().Duplicate();
        boom.projectile.GetDamageModel().damage = 4;
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(boom);  
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(Game.instance.model.GetTower(TowerType.BombShooter).GetAttackModel().weapons[0].projectile.GetBehavior<CreateEffectOnContactModel>());
        towerModel.GetAttackModel().weapons[0].rate *= 0.5f;

        towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = Il2Cpp.BloonProperties.Purple;
    }
}

public class SupportDrop : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;

    public override int Path => MIDDLE;
    public override int Tier => 4;
    public override int Cost => 7500;


    public override string Description => "Support Drop: Drops support sentrys";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var SupportDrop = Game.instance.model.GetTower(TowerType.BombShooter, 0, 4, 0).GetAbilities()[0];
        

        AttackModel[] sentry = { Game.instance.model.GetTowerFromId("EngineerMonkey-200").GetAttackModels().First(a => a.name == "AttackModel_Spawner_").Duplicate() };
        sentry[0].range = 25;
        sentry[0].name = "Sentry_Weapon";
        sentry[0].weapons[0].Rate = 0.5f;
        sentry[0].weapons[0].projectile.RemoveBehavior<CreateTowerModel>();
        sentry[0].weapons[0].projectile.AddBehavior(new CreateTowerModel("CreateTowerInAbility", GetTowerModel<LaserSentry>(), 0, false, false, true, false, false));
        
        sentry[0].weapons[0].projectile.display = new() { guidRef = "22bf6a11c89ffc8448485efa9480a816" };

        SupportDrop.GetBehavior<ActivateAttackModel>().attacks = sentry;
        SupportDrop.GetBehavior<ActivateAttackModel>().lifespan = 1.5f;
        SupportDrop.GetBehavior<ActivateAttackModel>().Lifespan = 1.5f;
        SupportDrop.cooldown *= 1.1f;
        SupportDrop.GetBehavior<ActivateAttackModel>().isOneShot = false;
        SupportDrop.name = "SupportDrop";

        SupportDrop.icon = GetSpriteReference("PlasmaSentry");

        towerModel.AddBehavior(SupportDrop);

        towerModel.display = new() { guidRef = "af6bc5f76310fa84eae188f2f5381dc6" };
        towerModel.GetAttackModel().behaviors.First(a => a.name == "DisplayModel_AttackDisplay").Cast<DisplayModel>().display = new() { guidRef = "9e4bd88882a62fc4596ed8a89a085c69" };
    }
}

public class SuperDrop : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;

    public override int Path => MIDDLE;
    public override int Tier => 5;
    public override int Cost => 35000;


    public override string Description => "Super Drop: Drops super sentrys that quickly mow through any bloons";

    public override void ApplyUpgrade(TowerModel towerModel)
    {

        towerModel.display = new() { guidRef = "af6bc5f76310fa84eae188f2f5381dc6" };
        towerModel.GetAttackModel().behaviors.First(a => a.name == "DisplayModel_AttackDisplay").Cast<DisplayModel>().display = new() { guidRef = "5c9c1ff2098e42944938381a0d8d45e3" };
        towerModel.GetAttackModel().weapons[0].rate *= 0.2f;
        var newability = towerModel.GetAbility().Duplicate();
        towerModel.RemoveBehavior<AbilityModel>();
        newability.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateTowerModel>().tower = GetTowerModel<SuperSentry>().Duplicate();
        towerModel.AddBehavior(newability);

        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("MoabDamageModifier", "Moabs", 5, 0, false, true));
        towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage *= 3f;
        towerModel.displayScale = 0.8f;
    }
}
public class ShatteringNails : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait; 
    public override int Path => BOTTOM;
    public override int Tier => 1;
    public override int Cost => 250;


    public override string Description => "Nails strip fortification of all non moab bloons";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetAttackModel().weapons[0].projectile.collisionPasses = new[] { -1, 0 };
        var removefort = Game.instance.model.GetTower(TowerType.Alchemist, 0, 2, 0).GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetBehavior<RemoveBloonModifiersModel>().Duplicate();
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(removefort);
    }
}

public class AirLeak : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;
    public override int Path => BOTTOM;
    public override int Tier => 2;
    public override int Cost => 800;


    public override string Description => "Nails make bloons leak air, slowly taking damage over time";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetAttackModel().weapons[0].projectile.collisionPasses = new[] { -1, 0, 1 };
        var airleak = Game.instance.model.GetTowerFromId("Sauda 9").GetAttackModel().weapons[0].projectile.behaviors.First(a => a.name == "AddBehaviorToBloonModel_BleedNonMoab").Duplicate();
        airleak.Cast<AddBehaviorToBloonModel>().filter = new FilterMoabModel("LeakNonMoabs", true);
        airleak.name = "AirLeakNonMoab";
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(airleak);
    }
}

public class BiggerLeaks : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;
    public override int Path => BOTTOM;
    public override int Tier => 3;
    public override int Cost => 1800;


    public override string Description => "Air leaks deal 4x more damage to bloons";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>().interval = .25f;

        towerModel.display = new() { guidRef = "af6bc5f76310fa84eae188f2f5381dc6" };
        towerModel.GetAttackModel().behaviors.First(a => a.name == "DisplayModel_AttackDisplay").Cast<DisplayModel>().display = new() { guidRef = "95f0e98e9602cab4fb4f1dcc6d01653a" };

    }
}
public class MoabRips : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;
    public override int Path => BOTTOM;
    public override int Tier => 4;
    public override int Cost => 6000;


    public override string Description => "Leaks now affect moabs and do massive damage";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().filter = null;

        var rip = towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().Duplicate();
        rip.name = "AirLeakMoab";
        rip.GetBehavior<DamageOverTimeModel>().damage = 49;
        rip.filter = new FilterMoabModel("MoabsOnly", false);
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(rip);
    }
}
public class TheBigRip : ModUpgrade<Sentry>
{
    public override string Portrait => VanillaSprites.SentryPortrait;
    public override int Path => BOTTOM;
    public override int Tier => 5;
    public override int Cost => 31000;


    public override string Description => "No more bloon lol";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
        var moableak = towerModel.GetAttackModel().weapons[0].projectile.behaviors.First(a => a.name == "AirLeakMoab").Cast<AddBehaviorToBloonModel>();
        moableak.GetBehavior<DamageOverTimeModel>().damage = 150;
        moableak.GetBehavior<DamageOverTimeModel>().interval = 0.01f;

        towerModel.display = new() { guidRef = "af6bc5f76310fa84eae188f2f5381dc6" };
        towerModel.GetAttackModel().behaviors.First(a => a.name == "DisplayModel_AttackDisplay").Cast<DisplayModel>().display = new() { guidRef = "02712b0bdfcd72b4f8bebf853ab75d70" };
        towerModel.displayScale = 1.1f;
    }
}