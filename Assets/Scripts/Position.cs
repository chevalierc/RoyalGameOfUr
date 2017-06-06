using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position {
    public int x;
    public int y;

    public Position(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Position(Position p) {
        this.x = p.x;
        this.y = p.y;
    }

    public Position(Vector3 p) {
        this.x = (int)(p.x);
        this.y = (int)(p.y);
    }

    public Position(Vector2 p) {
        this.x = (int)(p.x);
        this.y = (int)(p.y);
    }

    public Vector2 toVector2() {
        return new Vector2(this.x, this.y);
    }

    public Vector3 toVector3() {
        return new Vector3(this.x, this.y, 0);
    }

    public int manhattanDistanceTo(Position p2) {
        return Mathf.Abs(p2.x - this.x) + Mathf.Abs(p2.y - this.y);
    }

    //overloads
    public override string ToString() {
        return "(" + this.x + ", " + this.y + ")";
    }

    public static Position operator +(Position p1, Position p2) {
        p1.x += p2.x;
        p1.y += p2.y;
        return p1;
    }

    public static Position operator *(Position p1, int s) {
        p1.x *= s;
        p1.y *= s;
        return p1;
    }

    public static Position operator *(Position p1, Position p2) {
        p1.x *= p2.x;
        p1.y *= p2.y;
        return p1;
    }

    //equality overloads

    public static bool operator !=(Position a, Position b) {
        return !(a == b);
    }

    public static bool operator ==(Position a, Position b) {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b)) {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null)) {
            return false;
        }

        // Return true if the fields match:
        return a.x == b.x && a.y == b.y;
    }

    public override bool Equals(System.Object obj) {
        // If parameter is null return false.
        if (obj == null) {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Position p = new Position((Position)obj);
        if ((System.Object)p == null) {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y);
    }

    public bool Equals(Position p) {
        // If parameter is null return false:
        if ((object)p == null) {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y);
    }

    public override int GetHashCode() {
        return x ^ y;
    }
}