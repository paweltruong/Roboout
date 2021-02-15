using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum EncounterDifficulty
{
    D00_DiveInThePark = 0,
    D01_VeryEasy = 1, //[Introduces Light Enemy Warriors]
    D02_Easy = 2,
    D03_Medium =3, //[Introduces Medium Sized Enemies]
    D04_Challenging = 4,//[Introduces Heavy Enemies]
    D05_VeryChallenging = 5,//[Increases Number of enemies]
    D06_Hard = 6,
    D07_VeryHard =7,// - [Introduces Elite Enemies]
    D08_HardAsHell = 8,// - [Increases Number of Elites]
    D09_SuicideMission = 9,
    D10_Impossible = 10,// - [Reduces amount of light enemies]
    D11_Helldive = 11,// - [Highest difficulty before the 2018 A New Hell free content update]
    D12_AnExerciseInFutility = 12,// - [Introduces new enemy units]
    D13_TheDefinitionOfInsanity = 13,// - [Reduces amount of light enemies]
    D14_TheInnerCircleOfHell = 14,// - [Reduces amount of light enemies, Increases Number of Elites]
}
