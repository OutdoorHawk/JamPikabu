namespace Code.Infrastructure.Data
{
    public enum PhysicalBodyPartType
    {
        None = 0,
        Leg = 1,
        Arm = 2,
        Hips = 3,
        Head = 4,
        Spine = 5,
        UpperLeg = 6,
        Hand = 7,
        Shoulder = 8,
    }

    public enum LaserAnimationState
    {
        None = 0,
        Active,
        Cooldown
    }

    public enum CharacterID
    {
        Buster = 0,
        Evelone = 1,
        DmitryLixxx = 2
    }

    public enum SoundType
    {
        Music = 0,
        SFX_1 = 1,
        SFX_2 = 2,
        SFX_3 = 3,
        Scream = 4
    }

    public enum AbilityType
    {
        DoubleJump = 0,
        Dash = 1,
        Defence = 2
    }

    public enum LocalizationTables
    {
        TUTORIAL = 0,
    }
    
    public enum Identity
    {
        General
    }

    public enum DecalType
    {
        Environment = 0,
        Character = 1,
    }

    public enum WeaponID
    {
        Rifle_1 = 0,
        Rifle_2 = 1,
    }
    
    public enum WeaponType
    {
        Rifle = 0,
        Pistol = 1,
        Shotgun = 2,
    }
}