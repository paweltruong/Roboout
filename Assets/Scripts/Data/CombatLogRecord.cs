using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CombatLogRecord
{
    public string RobotName;
    public DateTime date;
    public string Message;

    public CombatLogRecord(string roboName, DateTime dateTime, string message)
    {
        this.RobotName = roboName;
        date = dateTime;
        this.Message = message;
    }

    public override string ToString()
    {
        return $"[{date.ToString("yyyy-MM-dd HH:mm:ss")} - {RobotName}] {Message}";
    }
}
