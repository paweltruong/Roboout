using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// skin/theme
/// </summary>
public enum PaintJob
{
    Default = 0,
    Khaki = 1,
    Orange = 2,
    Guard = 3,
    Prisoner = 4,
    Dark = 5,
    Red = 6,
    Blue = 7,
    Green = 8,
    Pink = 9
}

/// <summary>
/// configuration within paintjob
/// </summary>
public enum PaintJobElement
{
    BodyMain = 0,
    BodySecondary = 1,
    Detail1 = 2,
    Detail2 = 3,
    Detail3 = 4,
    Plugs = 5,
}

