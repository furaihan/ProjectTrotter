using Rage;
using Rage.Native;

namespace ProjectTrotter.MyPed
{
    public class PedCombatProperty
    {
        private readonly Ped _owner;
        internal PedCombatProperty(Ped owner)
        {
            _owner = owner;
        }
        #region SET_​PED_​COMBAT_​ATTRIBUTES
        public bool CanUseCover { set =>Att(_owner, 0, value); }
        public bool CanUseVehicles { set =>Att(_owner, 1, value); }
        public bool CanDoDrivebys { set =>Att(_owner, 2, value); }
        public bool CanLeaveVehicle { set =>Att(_owner, 3, value); }
        public bool CanUseDynamicStrafeDecisions { set =>Att(_owner, 4, value); }
        public bool AlwaysFight { set =>Att(_owner, 5, value); }
        public bool BlindFireWhenInCover { set =>Att(_owner, 12, value); }
        public bool Aggressive { set =>Att(_owner, 13, value); }
        public bool CanInvestigate { set =>Att(_owner, 14, value); }
        public bool HasRadio { set =>Att(_owner, 15, value); }
        public bool AlwaysFlee { set =>Att(_owner, 17, value); }
        public bool CanTauntInVehicle { set =>Att(_owner, 20, value); }
        public bool CanChaseTargetOnFoot { set =>Att(_owner, 21, value); }
        public bool WillDragInjuredPedsToSafety { set =>Att(_owner, 22, value); }
        public bool UseProximityFiringRate { set =>Att(_owner, 24, value); }
        public bool PerfectAccuracy { set =>Att(_owner, 27, value); }
        public bool CanUseFrustratedAdvance { set =>Att(_owner, 28, value); }
        public bool MaintainMinDistanceToTarget { set =>Att(_owner, 31, value); }
        public bool CanUsePeekingVariations { set =>Att(_owner, 34, value); }
        public bool DisableBulletReactions { set =>Att(_owner, 38, value); }
        public bool CanBust { set =>Att(_owner, 39, value); }
        public bool CanCommandeerVehicles { set =>Att(_owner, 41, value); }
        public bool CanFlank { set =>Att(_owner, 42, value); }
        public bool SwitchToAdvanceIfCantFindCover { set =>Att(_owner, 43, value); }
        public bool SwitchToDefensiveIfInCover { set =>Att(_owner, 44, value); }
        public bool CanFightArmedPedsWhenNotArmed { set =>Att(_owner, 46, value); }
        public bool UseEnemyAccuracyScaling { set =>Att(_owner, 49, value); }
        public bool CanCharge { set =>Att(_owner, 50, value); }
        public bool AlwaysEquipBestWeapon { set =>Att(_owner, 54, value); }
        public bool CanSeeUnderwaterPeds { set =>Att(_owner, 55, value); }
        public bool DisableFleeFromCombat { set =>Att(_owner, 58, value); }
        public bool CanThrowSmokeGrenade { set =>Att(_owner, 60, value); }
        public bool NonMissionPedsFleeFromThisPedUnlessArmed { set =>Att(_owner, 61, value); }
        public bool FleesFromInvincibleOpponents { set =>Att(_owner, 63, value); }
        public bool DisableBlockFromPursueDuringVehicleChase { set =>Att(_owner, 64, value); }
        public bool DisableSpinOutDuringVehicleChase { set =>Att(_owner, 65, value); }
        public bool DisableCruiseInFrontDuringBlockDuringVehicleChase { set =>Att(_owner, 66, value); }
        public bool DisableReactToBuddyShot { set =>Att(_owner, 68, value); }
        public bool PermitChargeBeyondDefensiveArea { set =>Att(_owner, 71, value); }
        public bool SetDisableShoutTargetPositionOnCombatStart { set =>Att(_owner, 76, value); }
        public bool DisableRespondedToThreatBroadcast { set =>Att(_owner, 77, value); }
        public bool AllowDogFighting { set =>Att(_owner, 86, value); }
        #endregion

        public float SeeingRange { set => NativeFunction.Natives.SET_​PED_​SEEING_​RANGE(_owner, value); }
        public float HearingRange { set => NativeFunction.Natives.SET_​PED_​HEARING_​RANGE(_owner, value); }

        #region SET_​COMBAT_​FLOAT
        public float BlindFireChance { set => Flo(_owner, 0, value); }
        public float BurstDurationInCover { set => Flo(_owner, 1, value); }
        public float MaxShootingDistance { set => Flo(_owner, 2, value); }
        public float TimeBetweenBurstsInCover { set => Flo(_owner, 3, value); }
        public float TimeBetweenPeeks { set => Flo(_owner, 4, value); }
        public float StrafeWhenMovingChance { set => Flo(_owner, 5, value); }
        public float WeaponAccuracy { set => Flo(_owner, 6, value); }
        public float WalkWhenStrafingChance { set => Flo(_owner, 7, value); }
        public float HeliSpeedModifier { set => Flo(_owner, 8, value); }
        public float HeliSensesRange { set => Flo(_owner, 9, value); }
        public float AttackWindowDistanceForCover { set => Flo(_owner, 10, value); }
        public float TimeToInvalidateInjuredTarget { set => Flo(_owner, 11, value); }
        public float MinimumDistanceToTarget { set => Flo(_owner, 12, value); }
        public float BulletImpactDetectionRange { set => Flo(_owner, 13, value); }
        public float AimTurnThreshold { set => Flo(_owner, 14, value); }
        public float OptimalCoverDistance { set => Flo(_owner, 15, value); }
        public float AutomobileSpeedModifier { set => Flo(_owner, 16, value); }
        public float SpeedToFleeInVehicle { set => Flo(_owner, 17, value); }
        public float TriggerChargeTime_Far { set => Flo(_owner, 18, value); }
        public float TriggerChargeTime_Near { set => Flo(_owner, 19, value); }
        public float MaxDistanceToHearEvents { set => Flo(_owner, 20, value); }
        public float MaxDistanceToHearEventsUsingLOS { set => Flo(_owner, 21, value); }
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
        private static void Att(Ped ped, int index, bool value) => NativeFunction.Natives.SET_​PED_​COMBAT_​ATTRIBUTES(ped, index, value);
        private static void Flo(Ped ped, int index, float value) => NativeFunction.Natives.SET_​COMBAT_​FLOAT(ped, index, value);
    }
}
