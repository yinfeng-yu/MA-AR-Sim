using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandType
{
    Grab,
    Displace,
    Patrol,
}

public enum Handness
{
    Left,
    Right,
}

// Unity serialization does not support derived classes.
// As the result, the base class has to contain everything.
[Serializable]
public class Command
{
    public CommandType type;

    // Grab command
    public Handness handness;

    // Displace command
    public Vector2 targetLocation;

    // Patrol waypoints
    public Vector2[] waypoints;

    public Command(CommandType a_commandType)
    {
        type = a_commandType;
    }
}

// [Serializable]
// public class GrabCommand : Command
// {
//     public Handness handness;
// 
//     public GrabCommand(Handness a_handness)
//     {
//         type = CommandType.Grab;
//         handness = a_handness;
//     }
// 
// }
// 
// [Serializable]
// public class DisplaceCommand : Command
// {
//     public Vector2 targetLocation;
// 
//     public DisplaceCommand(Vector2 a_targetLocation)
//     {
//         type = CommandType.Displace;
//         targetLocation = a_targetLocation;
//     }
// 
// }