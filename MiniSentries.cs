using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppSystem;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using MelonLoader;

namespace SentryTower
{
    internal class MiniSentries
    {
        
        public class LaserSentry : ModTower
        {
            protected override int Order => 1;
            public override TowerSet TowerSet => TowerSet.Support;
            public override string BaseTower => TowerType.SentryParagonChild;
            public override int Cost => 0;
            public override int TopPathUpgrades => 0;
            public override int MiddlePathUpgrades => 0;
            public override int BottomPathUpgrades => 0;

            public override string Name => "Laser Sentry";
            public override bool DontAddToShop => true;
            public override string Description => "A sentry deployed by a large sentry";
            public override string Icon => VanillaSprites.SentryParagonChildPortrait;

            public override string Portrait => VanillaSprites.SentryParagonChildPortrait;

            public override void ModifyBaseTowerModel(TowerModel towerModel)
            {
                towerModel.GetAttackModel().weapons[0].rate = .2f;
                towerModel.displayScale = 0.7f;
            }
        }
        public class SuperSentry : ModTower
        {
            protected override int Order => 2;
            public override TowerSet TowerSet => TowerSet.Support;
            public override string BaseTower => TowerType.SentryParagonChild;
            public override int Cost => 0;
            public override int TopPathUpgrades => 0;
            public override int MiddlePathUpgrades => 0;
            public override int BottomPathUpgrades => 0;

            public override bool DontAddToShop => true;
            public override string Description => "A sentry deployed by a large sentry";
            public override string Icon => VanillaSprites.SentryParagonChildPortrait;

            public override string Portrait => VanillaSprites.SentryParagonChildPortrait;

            public override void ModifyBaseTowerModel(TowerModel towerModel)
            {
                towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(Game.instance.model.GetTower(TowerType.BombShooter).GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>());
                //towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(Game.instance.model.GetTower(TowerType.BombShooter).GetAttackModel().weapons[0].projectile.GetBehavior<CreateEffectOnContactModel>());

                towerModel.display = new() { guidRef = "af6bc5f76310fa84eae188f2f5381dc6" };

                towerModel.GetBehavior<TowerExpireModel>().lifespan *= 2;
            }
        }
    }
}
