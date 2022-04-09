using UnityEngine;

public enum CommandType : int
{
    Focus,
    SetTarget,
    Spawn_0,
    Skill_0,
    Follow,
    Income,
    Damage
}

public struct Command
{
    public CommandType  type;
    public Vector3      vec3;
    public int          id;
    public int          cnt;
    public int[]        ids;
    public Command(CommandType _type, Vector3 _vec3, int _id, int _cnt, int[] _ids)
    {
        type = _type;
        vec3 = _vec3;
        id = _id;
        cnt = _cnt;
        ids = _ids;
    }
}
