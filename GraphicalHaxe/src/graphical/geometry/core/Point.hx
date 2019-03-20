package graphical.geometry.core;

import graphical.geometry.core.Base;
import graphical.geometry.core.IGeometry;

class Point extends Base implements IGeometry<Point>{
    
    // Properties
    public var X:Float;
    public var Y:Float;
    public var Z:Float;

    // Private Constructor
    private function new(x:Float, y:Float, z:Float){
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    // Static Constructors
    public function ByCoordinates(x:Float, y:Float, z:Float = 0):Point{
        return new Point(x, y, z);
    }

    public function ByCoordinatesArray(coordinates:Array<Int>):Point{
        return new Point(coordinates[0], coordinates[1], coordinates[2]);
    }

}