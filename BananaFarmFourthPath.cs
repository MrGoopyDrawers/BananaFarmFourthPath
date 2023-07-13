using MelonLoader;
using BTD_Mod_Helper;
using PathsPlusPlus;
using Il2CppAssets.Scripts.Models.Towers;
using BTD_Mod_Helper.Api.Enums;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using JetBrains.Annotations;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppSystem.IO;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using Il2CppAssets.Scripts.Models.TowerSets;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using UnityEngine;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using System.Runtime.CompilerServices;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using BananaFarmFourthPath;

[assembly: MelonInfo(typeof(BananaFarmFourthPath.BananaFarmFourthPath), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BananaFarmFourthPath;

public class BananaFarmFourthPath : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<BananaFarmFourthPath>("BananaFarmFourthPath loaded!");
    }
    public class FourthPath2 : PathPlusPlus
    {
        public override string Tower => TowerType.BananaFarm;
        public override int UpgradeCount => 5;

    }
    public class BananaAssist : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 750;
        public override int Tier => 1;
        public override string Icon => VanillaSprites.MonkeyTownUpgradeIcon;

        public override string Description => "Harvests it's own bananas and others in it's radius";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.AddBehavior(new CollectCashZoneModel("CollectCashZoneModel_", towerModel.range, 19, 3, "", true, true, true, false));
        }
    }
    public class BananaGuards : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 1650;
        public override int Tier => 2;
        public override string Icon => VanillaSprites.DartMonkeyIcon;

        public override string Description => "Places down nearby banana protectors that throw bananas at bloons";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var attackmodel = towerModel.GetAttackModel();
            AttackModel[] Avatarspawner = { Game.instance.model.GetTowerFromId("EngineerMonkey-200").GetAttackModels().First(a => a.name == "AttackModel_Spawner_").Duplicate() };
            Avatarspawner[0].weapons[0].rate = 15;
            Avatarspawner[0].name = "Guards";
            Avatarspawner[0].weapons[0].projectile.RemoveBehavior<CreateTowerModel>();
            Avatarspawner[0].weapons[0].projectile.AddBehavior(new CreateTowerModel("CreateTower", GetTowerModel<NinjaSlicer>(), 0, false, false, false, false, false));
            Avatarspawner[0].weapons[0].projectile.display = new() { guidRef = "" };
            towerModel.AddBehavior(Avatarspawner[0]);
        }
    }
    public class MiniFarms : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 6000;
        public override int Tier => 3;
        public override string Icon => VanillaSprites.BananaFarm020;

        public override string Description => "Places down mini farms that produce temporary cash.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var attackmodel = towerModel.GetAttackModel();
            AttackModel[] Avatarspawner = { Game.instance.model.GetTowerFromId("EngineerMonkey-200").GetAttackModels().First(a => a.name == "AttackModel_Spawner_").Duplicate() };
            Avatarspawner[0].weapons[0].AddBehavior(new EmissionsPerRoundFilterModel("idk",1));
            Avatarspawner[0].name = "miniFarms";
            Avatarspawner[0].weapons[0].projectile.RemoveBehavior<CreateTowerModel>();
            Avatarspawner[0].weapons[0].projectile.AddBehavior(new CreateTowerModel("CreateTower", GetTowerModel<MiniBank>(), 0, false, false, false, false, false));
            Avatarspawner[0].weapons[0].projectile.display = new() { guidRef = "" };
            towerModel.AddBehavior(Avatarspawner[0]);

            towerModel.range *= 1.3f;
            towerModel.GetBehavior<CollectCashZoneModel>().collectRange *= 1.5f;

        }
    }
    public class BananaTown : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 40000;
        public override int Tier => 4;
        public override string Icon => VanillaSprites.MonkeyVillage005;

        public override string Description => "More guards and better farms.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var attackModel in towerModel.GetAttackModels())
            {
                if (attackModel.name.Contains("Guards"))
                {
                    attackModel.range *= 5;
                    attackModel.weapons[0].rate *= 0.8f;
                    attackModel.weapons[0].projectile.GetBehavior<CreateTowerModel>().tower = GetTowerModel<NinjaSlicer2>();
                }
                else if (attackModel.name.Contains("miniFarms"))
                {
                    attackModel.range *= 5;
                    attackModel.weapons[0].rate *= 0.8f;
                    attackModel.weapons[0].GetBehavior<EmissionsPerRoundFilterModel>().count += 3;
                    attackModel.weapons[0].projectile.GetBehavior<CreateTowerModel>().tower = GetTowerModel<MiniBank2>();
                }
            }
            towerModel.range *= 1.3f;
            towerModel.GetBehavior<CollectCashZoneModel>().collectRange *= 1.5f;
        }
    }
    public class Bananaopolis : UpgradePlusPlus<FourthPath2>
    {
        public override int Cost => 300000;
        public override int Tier => 5;
        public override string Icon => VanillaSprites.MerchantmanUpgradeIcon;

        public override string Description => "Destroys bloons, makes a bunch of money, and offers buff!";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var attackModel in towerModel.GetAttackModels())
            {
                attackModel.range *= 5;
                if (attackModel.name.Contains("Guards"))
                {
                    attackModel.range *= 5;
                    attackModel.weapons[0].rate *= 0.3f;
                    attackModel.weapons[0].projectile.RemoveBehavior<CreateTowerModel>();
                    attackModel.weapons[0].projectile.AddBehavior(new CreateTowerModel("CreateTower", GetTowerModel<NinjaSlicer3>(), 0, false, false, false, false, false));
                }
                else if (attackModel.name.Contains("miniFarms"))
                {
                    attackModel.range *= 5;
                    attackModel.weapons[0].rate *= 0.2f;
                    attackModel.weapons[0].GetBehavior<EmissionsPerRoundFilterModel>().count += 3;
                    attackModel.weapons[0].projectile.GetBehavior<CreateTowerModel>().tower = GetTowerModel<MiniBank3>();
                }
            }
            towerModel.range *= 1.6f;
            towerModel.GetBehavior<CollectCashZoneModel>().collectRange *= 1.5f;
            towerModel.AddBehavior(new MonkeyCityIncomeSupportModel("_MonkeyCityIncomeSupport", true, 3f, null, "MonkeyCityBuff", "BuffIconVillagexx4"));
        }
    }
    public class NinjaSlicer : ModTower
    {
        protected override int Order => 1;
        public override TowerSet TowerSet => TowerSet.Primary;
        public override string BaseTower => "DartMonkey-002";
        public override int Cost => 0;
        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override string Name => "Peasent";
        public override bool DontAddToShop => true;
        public override string Description => "oof";

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.isSubTower = true;
            towerModel.icon = towerModel.portrait = Game.instance.model.GetTowerFromId("DartMonkey-010").portrait;
            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("Marine").GetBehavior<TowerExpireModel>().Duplicate());
            towerModel.GetBehavior<TowerExpireModel>().lifespan = 25;
            var attackModel = towerModel.GetAttackModel();
            attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("dmgMod", "Moabs", 2f, 20f, false, false));
            attackModel.weapons[0].projectile.AddBehavior(Game.instance.model.GetTowerFromId("Adora 20").GetAttackModel().weapons[0].projectile.GetBehavior<AdoraTrackTargetModel>().Duplicate());
            attackModel.weapons[0].projectile.hasDamageModifiers = true;
            towerModel.range *= 0.6f;
            attackModel.weapons[0].projectile.pierce += 10;
            attackModel.weapons[0].projectile.display = Game.instance.model.GetTowerFromId("BananaFarm").GetWeapon().projectile.display;
        }
    }
    public class NinjaSlicer2 : ModTower
    {
        protected override int Order => 1;
        public override TowerSet TowerSet => TowerSet.Primary;
        public override string BaseTower => "DartMonkey-030";
        public override int Cost => 0;
        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override string Name => "Banana Guard";
        public override bool DontAddToShop => true;
        public override string Description => "oof";

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.isSubTower = true;
            towerModel.icon = towerModel.portrait = Game.instance.model.GetTowerFromId("DartMonkey-030").portrait;
            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("Marine").GetBehavior<TowerExpireModel>().Duplicate());
            towerModel.GetBehavior<TowerExpireModel>().lifespan = 25;
            var attackModel = towerModel.GetAttackModel();
            attackModel.weapons[0].projectile.AddBehavior(Game.instance.model.GetTowerFromId("Adora 20").GetAttackModel().weapons[0].projectile.GetBehavior<AdoraTrackTargetModel>().Duplicate());
            
            attackModel.weapons[0].projectile.display = Game.instance.model.GetTowerFromId("BananaFarm").GetWeapon().projectile.display;
            attackModel.weapons[0].projectile.pierce *= 5;
            attackModel.weapons[0].rate *= 0.15f;
            AttackModel[] Avatarspawner = { Game.instance.model.GetTowerFromId("EngineerMonkey-200").GetAttackModels().First(a => a.name == "AttackModel_Spawner_").Duplicate() };
            Avatarspawner[0].weapons[0].rate = 15f;
            Avatarspawner[0].name = "miniFarms2332";
            Avatarspawner[0].weapons[0].projectile.RemoveBehavior<CreateTowerModel>();
            Avatarspawner[0].weapons[0].projectile.AddBehavior(new CreateTowerModel("CreateTower", GetTowerModel<NinjaSlicer>(), 0, false, false, false, false, false));
            Avatarspawner[0].weapons[0].projectile.display = new() { guidRef = "" };
            towerModel.AddBehavior(Avatarspawner[0]);
        }
    }
    public class NinjaSlicer3 : ModTower
    {
        protected override int Order => 1;
        public override TowerSet TowerSet => TowerSet.Primary;
        public override string BaseTower => "SuperMonkey-002";
        public override int Cost => 0;
        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override string Name => "Banana Protection Agent";
        public override bool DontAddToShop => true;
        public override string Description => "oof";

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.isSubTower = true;
            towerModel.icon = towerModel.portrait = Game.instance.model.GetTowerFromId("SuperMonkey-002").portrait;
            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("Marine").GetBehavior<TowerExpireModel>().Duplicate());
            towerModel.GetBehavior<TowerExpireModel>().lifespan = 45;
            var attackModel = towerModel.GetAttackModel();
            attackModel.weapons[0].projectile.display = Game.instance.model.GetTowerFromId("BananaFarm-010").GetWeapon().projectile.display;
            attackModel.weapons[0].projectile.pierce += 55;
            attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("dmgMod","Moabs",5f,100f,false,false));
            attackModel.weapons[0].projectile.AddBehavior(Game.instance.model.GetTowerFromId("Adora 20").GetAttackModel().weapons[0].projectile.GetBehavior<AdoraTrackTargetModel>().Duplicate());
            attackModel.weapons[0].projectile.hasDamageModifiers = true;
            attackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
            AttackModel[] Avatarspawner = { Game.instance.model.GetTowerFromId("EngineerMonkey-200").GetAttackModels().First(a => a.name == "AttackModel_Spawner_").Duplicate() };
            Avatarspawner[0].weapons[0].rate = 5f;
            Avatarspawner[0].name = "miniFarms";
            Avatarspawner[0].weapons[0].projectile.RemoveBehavior<CreateTowerModel>();
            Avatarspawner[0].weapons[0].projectile.AddBehavior(new CreateTowerModel("CreateTower", GetTowerModel<NinjaSlicer4>(), 0, false, false, false, false, false));
            Avatarspawner[0].weapons[0].projectile.display = new() { guidRef = "" };
            towerModel.AddBehavior(Avatarspawner[0]);
        }
    }
    public class NinjaSlicer4 : ModTower
    {
        protected override int Order => 1;
        public override TowerSet TowerSet => TowerSet.Primary;
        public override string BaseTower => "DartMonkey-030";
        public override int Cost => 0;
        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override string Name => "Banana Guard";
        public override bool DontAddToShop => true;
        public override string Description => "oof";

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.isSubTower = true;
            towerModel.icon = towerModel.portrait = Game.instance.model.GetTowerFromId("DartMonkey-030").portrait;
            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("Marine").GetBehavior<TowerExpireModel>().Duplicate());
            towerModel.GetBehavior<TowerExpireModel>().lifespan = 25;
            var attackModel = towerModel.GetAttackModel();
            attackModel.weapons[0].projectile.display = Game.instance.model.GetTowerFromId("BananaFarm").GetWeapon().projectile.display;
            attackModel.weapons[0].projectile.pierce += 65;
            attackModel.weapons[0].rate *= 0.15f;
            attackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("dmgMod", "Moabs", 3f, 30f, false, false));
            attackModel.weapons[0].projectile.AddBehavior(Game.instance.model.GetTowerFromId("Adora 20").GetAttackModel().weapons[0].projectile.GetBehavior<AdoraTrackTargetModel>().Duplicate());
            attackModel.weapons[0].projectile.hasDamageModifiers = true;
            AttackModel[] Avatarspawner = { Game.instance.model.GetTowerFromId("EngineerMonkey-200").GetAttackModels().First(a => a.name == "AttackModel_Spawner_").Duplicate() };
            Avatarspawner[0].weapons[0].rate = 5f;
            Avatarspawner[0].name = "miniFarms2332";
            Avatarspawner[0].weapons[0].projectile.RemoveBehavior<CreateTowerModel>();
            Avatarspawner[0].weapons[0].projectile.AddBehavior(new CreateTowerModel("CreateTower", GetTowerModel<NinjaSlicer>(), 0, false, false, false, false, false));
            Avatarspawner[0].weapons[0].projectile.display = new() { guidRef = "" };
            towerModel.AddBehavior(Avatarspawner[0]);
        }
    }
    public class MiniBank : ModTower
    {
        protected override int Order => 1;
        public override TowerSet TowerSet => TowerSet.Primary;
        public override string BaseTower => "BananaFarm-220";
        public override int Cost => 0;
        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override string Name => "Mini Farm";
        public override bool DontAddToShop => true;
        public override string Description => "oof";

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.isSubTower = true;
            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("Marine").GetBehavior<TowerExpireModel>().Duplicate());
            towerModel.GetBehavior<TowerExpireModel>().lifespan = 120;
            towerModel.displayScale *= 0.5f;
            towerModel.radius *= 0.5f;
            towerModel.footprint = Game.instance.model.GetTowerFromId("DartMonkey").footprint;
            towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<CashModel>().maximum += 10;
            towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<CashModel>().minimum += 10;
        }
    }
    public class MiniBank2 : ModTower
    {
        protected override int Order => 1;
        public override TowerSet TowerSet => TowerSet.Primary;
        public override string BaseTower => "BananaFarm-320";
        public override int Cost => 0;
        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override string Name => "Mini Farm 2";
        public override bool DontAddToShop => true;
        public override string Description => "oof";

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.isSubTower = true;
            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("Marine").GetBehavior<TowerExpireModel>().Duplicate());
            towerModel.GetBehavior<TowerExpireModel>().lifespan = 140;
            towerModel.displayScale *= 0.5f;
            towerModel.radius *= 0.5f;
            towerModel.footprint = Game.instance.model.GetTowerFromId("DartMonkey").footprint;
            towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<CashModel>().maximum += 20;
            towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<CashModel>().minimum += 20;
        }
    }
    public class MiniBank3 : ModTower
    {
        protected override int Order => 1;
        public override TowerSet TowerSet => TowerSet.Primary;
        public override string BaseTower => "BananaFarm-204";
        public override int Cost => 0;
        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override string Name => "Mini Farm 4";
        public override bool DontAddToShop => true;
        public override string Description => "oof";

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.isSubTower = true;
            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("Marine").GetBehavior<TowerExpireModel>().Duplicate());
            towerModel.GetBehavior<TowerExpireModel>().lifespan = 200;
            towerModel.displayScale *= 0.5f;
            towerModel.radius *= 0.5f;
            towerModel.footprint = Game.instance.model.GetTowerFromId("DartMonkey").footprint;
            towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<CashModel>().maximum += 530;
            towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<CashModel>().minimum += 530;
            AttackModel[] Avatarspawner = { Game.instance.model.GetTowerFromId("EngineerMonkey-200").GetAttackModels().First(a => a.name == "AttackModel_Spawner_").Duplicate() };
            Avatarspawner[0].weapons[0].AddBehavior(new EmissionsPerRoundFilterModel("idk", 3));
            Avatarspawner[0].name = "miniFarms";
            Avatarspawner[0].weapons[0].projectile.RemoveBehavior<CreateTowerModel>();
            Avatarspawner[0].weapons[0].projectile.AddBehavior(new CreateTowerModel("CreateTower", GetTowerModel<MiniBank4>(), 0, false, false, false, false, false));
            Avatarspawner[0].weapons[0].projectile.display = new() { guidRef = "" };
            towerModel.AddBehavior(Avatarspawner[0]);
        }
    }
    public class MiniBank4 : ModTower
    {
        protected override int Order => 1;
        public override TowerSet TowerSet => TowerSet.Primary;
        public override string BaseTower => "BananaFarm-320";
        public override int Cost => 0;
        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override string Name => "Mini Farm 2";
        public override bool DontAddToShop => true;
        public override string Description => "oof";

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.isSubTower = true;
            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("Marine").GetBehavior<TowerExpireModel>().Duplicate());
            towerModel.GetBehavior<TowerExpireModel>().lifespan = 140;
            towerModel.displayScale *= 0.5f;
            towerModel.radius *= 0.5f;
            towerModel.footprint = Game.instance.model.GetTowerFromId("DartMonkey").footprint;
            AttackModel[] Avatarspawner = { Game.instance.model.GetTowerFromId("EngineerMonkey-200").GetAttackModels().First(a => a.name == "AttackModel_Spawner_").Duplicate() };
            Avatarspawner[0].weapons[0].AddBehavior(new EmissionsPerRoundFilterModel("idk", 3));
            Avatarspawner[0].name = "miniFarms5453";
            Avatarspawner[0].weapons[0].projectile.RemoveBehavior<CreateTowerModel>();
            Avatarspawner[0].weapons[0].projectile.AddBehavior(new CreateTowerModel("CreateTower", GetTowerModel<MiniBank>(), 0, false, false, false, false, false));
            Avatarspawner[0].weapons[0].projectile.display = new() { guidRef = "" };
            towerModel.AddBehavior(Avatarspawner[0]);
        }
    }
}