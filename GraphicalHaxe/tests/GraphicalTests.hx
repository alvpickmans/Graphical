import geometry.PointTest;

class GraphicalTests {
  static function main() {
    var r = new haxe.unit.TestRunner();
    r.add(new PointTest());
    // add other TestCases here

    // finally, run the tests
    r.run();
  }
}