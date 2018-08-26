using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Pos
{
    public float X;
    public float Y;
    public float Z;

    public void SetPos(float x, float y, float z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public bool IsZero()
    {
        return this.X == 0 && this.Y == 0 && this.Z == 0;
    }
}

public struct Face
{
    public float X;
    public float Y;
    public float Z;

    public void SetFace(float x, float y, float z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public bool IsZero()
    {
        return this.X == 0 && this.Y == 0 && this.Z == 0;
    }
}
