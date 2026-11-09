using System;

//Used to store Project wide ENUMS/Identifiers

[Serializable]
public enum ENUM_RotationDirection
{
    None,
    Left,
    Right
}

[Serializable]
public enum ENUM_WeaponType
{
    None,
    Cannon,
    Coil,
    Scatter,
    Plasma
}

[Serializable]
public enum ENUM_CursorType
{
    None,
    Movement,
    Combat
}

[Serializable]
public enum ENUM_CursorSprite
{
    None,
    Standard,
    Valid,
    Invalid,
    Aiming
}

[Serializable]
public enum ENUM_TileType
{
    None,
    Asteroid,
    Debris,
    Gas,
    Nebulae
}

[Serializable]
public enum ENUM_Tokens
{
    None,
    Asteroids,
    Debris,
    Gas,
    Nebulae,
    Squadron
}