using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    None = 0,
    Forward,
    Back,
    Left,
    Right,
}

public static class RemoteInput
{
    private static bool confirm;
    private static bool[] directions = new[] { false, false, false, false };

    public static bool GetConfirm()
    {
        return confirm;
    }

    public static void SetConfirm()
    {
        confirm = true;
    }

    public static void ConsumeConfirm()
    {
        confirm = false;
    }


    public static Direction GetDirection()
    {
        for (int i = 0; i < directions.Length; i++)
        {
            if (directions[i] == true)
            {
                return (Direction)(i + 1);
            }
        }
        return Direction.None;
    }

    public static void SetDirection(Direction direction)
    {
        if (direction == Direction.None)
        {
            for (int i = 0; i < directions.Length; i ++)
            {
                directions[i] = false;
            }
            return;
        }
        directions[(int)direction - 1] = true;
    }

    // public static void ConsumeDirection(Direction direction)
    // {
    //     if (direction == Direction.None)
    //     { 
    //         return; 
    //     }
    //     directions[(int)direction - 1] = false;
    // }

}
