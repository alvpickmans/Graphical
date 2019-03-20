package graphical.geometry.core;

import graphical.geometry.core.Point;

/**
 * A Vector object.
 */
class Vector extends Point {
    //Public properties

    /**
     * Magnitud of Vector
     */
    public var Magnitud:Float;

    // Private Constructor
    @:overload private function new(x:Float, y:Float, z:Float){
        super(x, y, z);
        this.Magnitud = Math.sqrt(
                Math.pow(this.X, 2) 
                + Math.pow(this.Y, 2)
                + Math.pow(this.Z, 2)
                );
    }

    @:overload private function new(x:Float, y:Float, z:Float, magnitud:Float){
        super(x, y, z);
        this.Magnitud = magnitud;
    }

    // Public Constructors
    /**
     * Creates Vector instance with the given coordinates.
     * @param x Coordinate X
     * @param y Coordinate Y
     * @param z Coordinate Z - Default 0;
     * @return Vector
     */
    public function ByCoorindates(x:Float, y:Float, z:Float = 0):Vector{
        return new Vector(x, y, z);
    }

    /**
     * Creates a Vector instance from two Points
     * @param start 
     * @param end 
     * @return Vector
     */
    public function ByTwoPoints(start:Point, end:Point):Vector{
        return new Vector(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
    }

    /**
     * Returns canonical X Vector
     * @return Vector
     */
    public function XAxis():Vector{
        return new Vector(1, 0, 0);
    }

    /**
     * Returns canonical Y Vector
     * @return Vector
     */
    public function YAxis():Vector{
        return new Vector(0, 1, 0);
    }

    /**
     * Returns canonical Z Vector
     * @return Vector
     */
    public function ZAxis():Vector{
        return new Vector(0, 0, 1);
    }


    /**
     * Public Methods
     */

    /**
     * Dot product with another Vector
     * @param vector 
     * @return Float
     */
    public function Dot(vector:Vector):Float{
        return this.X * vector.X
            + this.Y * vector.Y
            + this.Z * vector.Z;
    }

    public function Cross(vector:Vector):Vector{
        var x = (this.Y * vector.Z) - (this.Z * vector.Y);
        var y = (this.Z * vector.X) - (this.X * vector.Z);
        var z = (this.X * vector.Y) - (this.Y * vector.X);
        var angle = this.Angle(vector);
        var magnitud = this.Magnitud * vector.Magnitud * Math.sin(angle);
        return new Vector(x, y, z, magnitud);
    }

    /**
     * Returns the Angle in Radians with another Vector.
     * @param vector 
     * @return Float - In Radians
     */
    public function Angle(vector:Vector):Float{
        var dot = this.Dot(vector);
        var cos = dot / (this.Magnitud * vector.Magnitud);
        if(cos > -1 && cos < 1){
            return Math.acos(cos);
        }else{
            return cos <= -1 ? Math.acos(-1) : Math.acos(1);
        }

    }


}