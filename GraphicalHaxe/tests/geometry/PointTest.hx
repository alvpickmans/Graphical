import haxe.unit.TestCase;
import src.graphical.geometry.core.Point;

class PointTest extends haxe.unit.TestCase{
    public function testConstructor(){
        var point = new Point(0, 1, 2);

        assertEquals(point.X, 0);
        assertEquals(point.Y, 1);
        assertEquals(point.Z, 2);
    }
}