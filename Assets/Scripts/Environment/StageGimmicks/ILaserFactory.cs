using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public interface ILaserTargetFactory
{
    LaserTarget GetTargetByName(string name);
    LaserTarget GetTargetByIndex(int index);
    IReadOnlyList<LaserTarget> GetAllTargets();
}

public interface ILaserFactory
{
    Laser GetLaserByName(string name);
    Laser GetLaserByIndex(int index);
    IReadOnlyList<Laser> GetAllLasers();
}

public class LaserTargetFactory : ILaserTargetFactory
{
    private readonly IReadOnlyList<LaserTarget> targets;

    public LaserTargetFactory(IReadOnlyList<LaserTarget> targets)
    {
        this.targets = targets;
    }

    public LaserTarget GetTargetByName(string name)
    {
        return targets.FirstOrDefault(t => t.name == name);
    }

    public LaserTarget GetTargetByIndex(int index)
    {
        return index >= 0 && index < targets.Count ? targets[index] : null;
    }

    public IReadOnlyList<LaserTarget> GetAllTargets()
    {
        return targets;
    }
}

public class LaserFactory : ILaserFactory
{
    private readonly IReadOnlyList<Laser> lasers;

    public LaserFactory(IReadOnlyList<Laser> lasers)
    {
        this.lasers = lasers;
    }

    public Laser GetLaserByName(string name)
    {
        return lasers.FirstOrDefault(l => l.name == name);
    }

    public Laser GetLaserByIndex(int index)
    {
        return index >= 0 && index < lasers.Count ? lasers[index] : null;
    }

    public IReadOnlyList<Laser> GetAllLasers()
    {
        return lasers;
    }
}
