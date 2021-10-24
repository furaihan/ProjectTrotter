using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;

namespace BarbarianCall.Freemode
{
    public class PedCombatProperty
    {
        private readonly Ped _owner;
        internal PedCombatProperty(Ped owner)
        {
            _owner = owner;
        }
        #region SET_​PED_​COMBAT_​ATTRIBUTES
        public bool CanUseCover { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 0, value); }
        public bool CanUseVehicles { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 1, value); }
        public bool CanDoDrivebys { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 2, value); }
        public bool CanLeaveVehicle { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 3, value); }
        public bool CanUseDynamicStrafeDecisions { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 4, value); }
        public bool AlwaysFight { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 5, value); }
        public bool BlindFireWhenInCover { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 12, value); }
        public bool Aggressive { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 13, value); }
        public bool CanInvestigate { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 14, value); }
        public bool HasRadio { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 15, value); }
        public bool AlwaysFlee { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 17, value); }
        public bool CanTauntInVehicle { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 20, value); }
        public bool CanChaseTargetOnFoot { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 21, value); }
        public bool WillDragInjuredPedsToSafety { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 22, value); }
        public bool UseProximityFiringRate { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 24, value); }
        public bool PerfectAccuracy { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 27, value); }
        public bool CanUseFrustratedAdvance { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 28, value); }
        public bool MaintainMinDistanceToTarget { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 31, value); }
        public bool CanUsePeekingVariations { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 34, value); }
        public bool DisableBulletReactions { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 38, value); }
        public bool CanBust { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 39, value); }
        public bool CanCommandeerVehicles { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 41, value); }
        public bool CanFlank { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 42, value); }
        public bool SwitchToAdvanceIfCantFindCover { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 43, value); }
        public bool SwitchToDefensiveIfInCover { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 44, value); }
        public bool CanFightArmedPedsWhenNotArmed { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 46, value); }
        public bool UseEnemyAccuracyScaling { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 49, value); }
        public bool CanCharge { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 50, value); }
        public bool AlwaysEquipBestWeapon { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 54, value); }
        public bool CanSeeUnderwaterPeds { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 55, value); }
        public bool DisableFleeFromCombat { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 58, value); }
        public bool CanThrowSmokeGrenade { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 60, value); }
        public bool NonMissionPedsFleeFromThisPedUnlessArmed { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 61, value); }
        public bool FleesFromInvincibleOpponents { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 63, value); }
        public bool DisableBlockFromPursueDuringVehicleChase { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 64, value); }
        public bool DisableSpinOutDuringVehicleChase { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 65, value); }
        public bool DisableCruiseInFrontDuringBlockDuringVehicleChase { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 66, value); }
        public bool DisableReactToBuddyShot { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 68, value); }
        public bool PermitChargeBeyondDefensiveArea { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 71, value); }
        public bool SetDisableShoutTargetPositionOnCombatStart { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 76, value); }
        public bool DisableRespondedToThreatBroadcast { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 77, value); }
        public bool AllowDogFighting { set => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(_owner, 86, value); }
        #endregion

        public float SeeingRange { set => NativeFunction.Natives.SET_​PED_​SEEING_​RANGE(_owner, value); }
        public float HearingRange { set => NativeFunction.Natives.SET_​PED_​HEARING_​RANGE(_owner, value); }

        #region SET_​COMBAT_​FLOAT
        public float BlindFireChance { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 0, value); }
        public float BurstDurationInCover { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 1, value); }
        public float MaxShootingDistance { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 2, value); }
        public float TimeBetweenBurstsInCover { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 3, value); }
        public float TimeBetweenPeeks { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 4, value); }
        public float StrafeWhenMovingChance { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 5, value); }
        public float WeaponAccuracy { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 6, value); }
        public float WalkWhenStrafingChance { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 7, value); }
        public float HeliSpeedModifier { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 8, value); }
        public float HeliSensesRange { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 9, value); }
        public float AttackWindowDistanceForCover { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 10, value); }
        public float TimeToInvalidateInjuredTarget { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 11, value); }
        public float MinimumDistanceToTarget { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 12, value); }
        public float BulletImpactDetectionRange { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 13, value); }
        public float AimTurnThreshold { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 14, value); }
        public float OptimalCoverDistance { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 15, value); }
        public float AutomobileSpeedModifier { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 16, value); }
        public float SpeedToFleeInVehicle { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 17, value); }
        public float TriggerChargeTime_Far { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 18, value); }
        public float TriggerChargeTime_Near { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 19, value); }
        public float MaxDistanceToHearEvents { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 20, value); }
        public float MaxDistanceToHearEventsUsingLOS { set => NativeFunction.Natives.SET_​COMBAT_​FLOAT(_owner, 21, value); }
        #endregion
        public CombatMovement CombatMovement
        {
            get => (CombatMovement)NativeFunction.Natives.GET_PED_COMBAT_MOVEMENT<int>(_owner);
            set => NativeFunction.Natives.SET_​PED_​COMBAT_​MOVEMENT(_owner, (int)value);
        }
        public CombatRange CombatRange
        {
            get => (CombatRange)NativeFunction.Natives.GET_​PED_​COMBAT_​RANGE<int>(_owner);
            set => NativeFunction.Natives.SET_​PED_​COMBAT_​RANGE(_owner, (int)value);
        }
        public CombatAbility CombatAbility
        {
            set => NativeFunction.Natives.SET_PED_COMBAT_ABILITY(_owner, (int)value);
        }
    }
}
